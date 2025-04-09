using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NoteApp.Application.Interfaces.Services;
using NoteApp.Application.Settings;
using NoteApp.Infrastructure.Services;
using System.Text;
using NoteApp.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace NoteApp.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT yapılandırmasını ekle
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        // HTTP Context Accessor ekle
        services.AddHttpContextAccessor();
        
        // Servis implementasyonlarını kaydet
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        // JWT kimlik doğrulama yapılandırması
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings!.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }
}