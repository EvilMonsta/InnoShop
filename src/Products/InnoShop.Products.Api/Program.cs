using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using InnoShop.Products.Application.Validation;
using InnoShop.Products.Infrastructure.DI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

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
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        };
    });

builder.Services.AddAuthorization();

var conn = builder.Configuration["SqlStrCon"]
           ?? builder.Configuration["SqlStrConProducts"]
           ?? builder.Configuration.GetConnectionString("Default")
           ?? throw new InvalidOperationException("SqlStrCon is not set");

builder.Services.AddProductsInfrastructure(conn);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
