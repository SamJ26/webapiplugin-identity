using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiPlugin.Identity.Dtos;

namespace WebApiPlugin.Identity
{
    public class IdentityEndpoints
    {
        public static async Task<IResult> RegisterAsync(
            [FromBody] RegisterRequestDto registerRequest,
            [FromServices] IIdentityService identityService,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await identityService.CreateUserAsync(registerRequest, cancellationToken);
                if (result.Succeeded)
                {
                    var token = await identityService.GenerateJwtTokenAsync(registerRequest.UserName);
                    return Results.Ok(new AuthenticationResponseDto(token));
                }

                return Results.Problem(
                    title: "Internal Server Error",
                    statusCode: 500,
                    detail: "Unable to register new user",
                    extensions: new Dictionary<string, object?>() { { "errors", result.Errors.Select(e => e.Description).ToList() } });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Internal Server Error",
                    statusCode: 500,
                    detail: "Something went wrong in registration process",
                    extensions: new Dictionary<string, object?>() { { "errors", ex.Message } });
            }
        }

        public static async Task<IResult> LoginAsync(
            [FromBody] LoginRequestDto loginRequest,
            [FromServices] IIdentityService identityService)
        {
            try
            {
                var signInResult = await identityService.SignInAsync(loginRequest.UserName, loginRequest.Password);
                if (signInResult.Succeeded is false)
                {
                    return Results.BadRequest();
                }
                var token = await identityService.GenerateJwtTokenAsync(loginRequest.UserName);
                return Results.Ok(new AuthenticationResponseDto(token));
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Internal Server Error",
                    statusCode: 500,
                    detail: "Something went wrong in sign in process",
                    extensions: new Dictionary<string, object?>() { { "errors", ex.Message } });
            }
        }

        public static async Task<IResult> LogoutAsync([FromServices] IIdentityService identityService)
        {
            await identityService.SignOutAsync();
            return Results.Ok();
        }
    }
}