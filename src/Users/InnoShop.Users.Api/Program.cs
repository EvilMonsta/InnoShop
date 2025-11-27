using FluentValidation;
using FluentValidation.AspNetCore;
using InnoShop.Users.Api.Middlewares;
using InnoShop.Users.Infrastructure.DI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();