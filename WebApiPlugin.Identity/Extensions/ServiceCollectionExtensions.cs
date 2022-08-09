using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApiPlugin.Identity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureIdentity<TDbContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<Options.IdentityOptions> configureIdentityOptions)
            where TDbContext : DbContext
        {
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<TDbContext>()
                .AddDefaultTokenProviders();

            var jwtOptions = configuration.GetSection("JwtOptions").Get<Options.JwtOptions>();

            services.AddSingleton<IOptions<Options.JwtOptions>>(_ =>
            {
                return Microsoft.Extensions.Options.Options.Create(jwtOptions);
            });

            services.AddSingleton<IOptions<Options.IdentityOptions>>(_ =>
            {
                var options = new Options.IdentityOptions();
                configureIdentityOptions?.Invoke(options);
                return Microsoft.Extensions.Options.Options.Create(options);
            });

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(bearerOptions =>
            {
                bearerOptions.SaveToken = true;
                bearerOptions.RequireHttpsMetadata = false;
                bearerOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = jwtOptions.ValidAudience,
                    ValidIssuer = jwtOptions.ValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };
            });

            services.AddAuthorization();

            services.AddScoped<IIdentityService, IdentityService>();

            return services;
        }
    }
}
