using FluentValidation;
using FluentValidation.AspNetCore;
using InnoShop.Products.Api.Security;
using InnoShop.Products.Application.Validation;
using InnoShop.Products.Infrastructure.DI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using InnoShop.Products.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var allowed = builder.Configuration["Cors:AllowedOrigins"]?.Split(';', StringSplitOptions.RemoveEmptyEntries)
              ?? new[] { "http://localhost:8080", "http://localhost:5173", "https://localhost:5173" };
Console.WriteLine("CORS allowed origins: " + string.Join(", ", allowed));

builder.Services.AddCors(o =>
{
    o.AddPolicy("Frontend", p =>
    {
        p.WithOrigins(allowed)
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials();
    });
});

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var issuer = builder.Configuration["Jwt:Issuer"] ?? "innoshop";
var audience = builder.Configuration["Jwt:Audience"] ?? "innoshop.clients";
var signingKey = builder.Configuration["JwtSigningKey"]
                 ?? builder.Configuration["Jwt:SigningKey"]
                 ?? throw new InvalidOperationException("Jwt signing key is not configured. Set env JwtSigningKey.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.MapInboundClaims = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = ClaimTypes.Role
        };
    });


builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("ActiveUser", p => p.Requirements.Add(new ActiveUserRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, ActiveUserHandler>();

var conn = builder.Configuration["SqlStrCon"]
           ?? builder.Configuration["SqlStrConProducts"]
           ?? builder.Configuration.GetConnectionString("Default")
           ?? throw new InvalidOperationException("SqlStrCon is not set");

builder.Services.AddProductsInfrastructure(conn);

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();

    if (scope.ServiceProvider.GetService<ProductsDbContext>() is { } pdb)
    {
        var hasPending = pdb.Database.GetPendingMigrations().Any();
        if (hasPending) pdb.Database.Migrate();
        else pdb.Database.EnsureCreated();
    }
});


app.MapGet("/health", () => Results.Ok(new { ok = true }));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");  
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
