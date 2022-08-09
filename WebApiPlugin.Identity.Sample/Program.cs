using Microsoft.EntityFrameworkCore;
using WebApiPlugin.Identity.Extensions;

namespace WebApiPlugin.Identity.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Added
            builder.Services.AddDbContext<AppDbContext>(options
                => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Added
            builder.Services.ConfigureIdentity<AppDbContext>(builder.Configuration, (identityOptions) =>
            {
                identityOptions.InitialRole = "Admin";
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Added
            app.UseAuthentication();
            app.UseAuthorization();

            // Added
            app.MapEndpointsForIdentity();

            app.MapGet("api/anonymous/data", () => Results.Ok(new List<string>() { "1", "2", "3" }))
                .AllowAnonymous();

            app.MapGet("api/authorized/data", () => Results.Ok(new List<string>() { "4", "5", "6" }))
                .RequireAuthorization();

            app.Run();
        }
    }
}