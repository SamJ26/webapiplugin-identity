using Microsoft.AspNetCore.Identity;
using WebApiPlugin.Identity.Dtos;

namespace WebApiPlugin.Identity
{
    public interface IIdentityService
    {
        public Task<IdentityResult> CreateUserAsync(RegisterRequestDto registerRequest, CancellationToken cancellationToken);
        public Task<string> GenerateJwtTokenAsync(string userName);
        public Task<SignInResult> SignInAsync(string userName, string password);
        public Task SignOutAsync();
    }
}
