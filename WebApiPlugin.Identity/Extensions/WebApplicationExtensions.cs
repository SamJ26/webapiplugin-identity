using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiPlugin.Identity.Dtos;

namespace WebApiPlugin.Identity.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void MapEndpointsForIdentity(this WebApplication app)
        {
            app.MapPost($"api/identity/register", IdentityEndpoints.RegisterAsync)
                .AllowAnonymous()
                .Produces<AuthenticationResponseDto>(200)
                .Produces(400)
                .Produces<ProblemDetails>(500);

            app.MapPost($"api/identity/login", IdentityEndpoints.LoginAsync)
                .AllowAnonymous()
                .Produces<AuthenticationResponseDto>(200)
                .Produces(400)
                .Produces<ProblemDetails>(500);

            app.MapPost($"api/identity/logout", IdentityEndpoints.LogoutAsync)
                .RequireAuthorization()
                .Produces(200);
        }
    }
}
