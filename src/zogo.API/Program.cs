using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Serilog;

using zogo.Application;
using zogo.Infrastructure;
using zogo.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Serilog
// ------------------------------------------------------------
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

// ------------------------------------------------------------
// Controllers
// ------------------------------------------------------------
builder.Services.AddControllers();

// ------------------------------------------------------------
// Application
// ------------------------------------------------------------
builder.Services.AddApplication();

// ------------------------------------------------------------
// Infrastructure
// ------------------------------------------------------------
builder.Services.AddInfrastructure(
    builder.Configuration);

// ------------------------------------------------------------
// JWT Configuration
// ------------------------------------------------------------
var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException(
        "JWT configuration is missing.");

if (string.IsNullOrWhiteSpace(jwtOptions.Secret))
{
    throw new InvalidOperationException(
        "JWT Secret is missing.");
}

if (jwtOptions.Secret.Length < 32)
{
    throw new InvalidOperationException(
        "JWT Secret must be at least 32 characters.");
}

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // --------------------------------------------
                // Token validation
                // --------------------------------------------
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                // --------------------------------------------
                // JWT issuer / audience
                // --------------------------------------------
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,

                // --------------------------------------------
                // Signing key
                // --------------------------------------------
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtOptions.Secret)),

                // --------------------------------------------
                // IMPORTANT:
                // Tell ASP.NET Core which JWT claim contains
                // roles and the user identifier.
                // --------------------------------------------
                RoleClaimType = ClaimTypes.Role,
                NameClaimType = ClaimTypes.NameIdentifier,

                // Small tolerance for server clock differences
                ClockSkew = TimeSpan.FromSeconds(30)
            };
    });

// ------------------------------------------------------------
// Authorization
// ------------------------------------------------------------
builder.Services.AddAuthorization();

// ------------------------------------------------------------
// Swagger
// ------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------------------------------------------------
// Build application
// ------------------------------------------------------------
var app = builder.Build();

// ------------------------------------------------------------
// Swagger
// ------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ------------------------------------------------------------
// HTTPS
// ------------------------------------------------------------
app.UseHttpsRedirection();

// ------------------------------------------------------------
// Authentication MUST come before Authorization
// ------------------------------------------------------------
app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------------------------
// Controllers
// ------------------------------------------------------------
app.MapControllers();

app.Run();

public partial class Program;