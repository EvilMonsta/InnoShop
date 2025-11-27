using System.Security.Claims;
using InnoShop.Products.Application.Abstractions;
using InnoShop.Products.Contracts.Requests;
using InnoShop.Products.Contracts.Responses;
using InnoShop.Products.Domain.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Products.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _products;
    public ProductsController(IProductRepository products) => _products = products;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id, CancellationToken ct)
    {
        var p = await _products.GetByIdAsync(id, ct);
        if (p is null) return NotFound();
        return new ProductResponse(
            p.Id, p.OwnerUserId, p.Name, p.Description, p.Price, p.IsAvailable,
            p.CreatedAt.ToString("O"));
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductResponse>>> Search(
        [FromQuery] string? q,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? onlyAvailable,
        [FromQuery] Guid? ownerUserId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var query = _products.Query();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var ql = q.ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(ql) ||
                (x.Description ?? string.Empty).ToLower().Contains(ql));
        }
        if (minPrice.HasValue) query = query.Where(x => x.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(x => x.Price <= maxPrice.Value);
        if (onlyAvailable == true) query = query.Where(x => x.IsAvailable);
        if (ownerUserId.HasValue) query = query.Where(x => x.OwnerUserId == ownerUserId.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductResponse(
                p.Id, p.OwnerUserId, p.Name, p.Description, p.Price, p.IsAvailable,
                p.CreatedAt.ToString("O")))
            .ToListAsync(ct);

        return new PagedResult<ProductResponse>(items, total, page, pageSize);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductRequest req, CancellationToken ct)
    {
        var ownerId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (ownerId is null) return Forbid();

        var p = new Product(Guid.Parse(ownerId), req.Name, req.Description, req.Price);
        if (!req.IsAvailable)
            p.Update(p.Name, p.Description, p.Price, isAvailable: false);

        await _products.AddAsync(p, ct);
        await _products.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = p.Id }, new ProductResponse(
            p.Id, p.OwnerUserId, p.Name, p.Description, p.Price, p.IsAvailable,
            p.CreatedAt.ToString("O")));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest req, CancellationToken ct)
    {
        var callerId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callerId is null) return Forbid();

        var p = await _products.GetByIdAsync(id, ct);
        if (p is null) return NotFound();
        if (p.OwnerUserId.ToString() != callerId) return Forbid();

        p.Update(req.Name, req.Description, req.Price, req.IsAvailable);
        await _products.UpdateAsync(p, ct);
        await _products.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var callerId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callerId is null) return Forbid();

        var p = await _products.GetByIdAsync(id, ct);
        if (p is null) return NotFound();
        if (p.OwnerUserId.ToString() != callerId) return Forbid();

        p.SoftDelete();
        await _products.UpdateAsync(p, ct);
        await _products.SaveChangesAsync(ct);
        return NoContent();
    }
}
