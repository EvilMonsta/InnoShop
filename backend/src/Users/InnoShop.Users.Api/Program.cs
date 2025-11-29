using FluentValidation;
using FluentValidation.AspNetCore;
using InnoShop.Users.Api.Middlewares;
using InnoShop.Users.Infrastructure.DI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var allowed = builder.Configuration["Cors:AllowedOrigins"]?.Split(';', StringSplitOptions.RemoveEmptyEntries)
              ?? new[] { "http://localhost:5173", "https://localhost:5173" };

builder.Services.AddCors(o =>
{
    o.AddPolicy("Frontend", p => p
        .WithOrigins(allowed)
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = null;
});


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<InnoShop.Users.Api.Validation.RegisterUserRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<InnoShop.Users.Application.Validation.RegisterUserValidator>(); 
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



builder.Services.AddAuthorization();

var conn = builder.Configuration["SqlStrConUsers"]
           ?? builder.Configuration.GetConnectionString("Default")
           ?? throw new InvalidOperationException("SqlStrCon is not set");


builder.Services.AddUsersInfrastructure(conn);


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/__whoami", (HttpContext ctx) =>
{
    var claims = ctx.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
    return Results.Json(new
    {
        id = ctx.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
             ?? ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
             ?? "(no sub)",
        roleClaims = ctx.User.Claims.Where(c => c.Type is "role" or ClaimTypes.Role)
                                    .Select(c => c.Value)
    });
}).RequireAuthorization();
app.MapControllers();
app.Run();