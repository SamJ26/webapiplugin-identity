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

            app.MapGet("api/data", () => Results.Ok(new List<string>() { "1", "2", "3" }));

            // Added
            app.MapEndpointsForIdentity();

            app.Run();
        }
    }
}