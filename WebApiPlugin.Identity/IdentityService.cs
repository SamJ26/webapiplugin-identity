using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiPlugin.Identity.Dtos;
using WebApiPlugin.Identity.Options;

namespace WebApiPlugin.Identity
{
    public class IdentityService<TUser> : IIdentityService
        where TUser : IdentityUser<Guid>, new()
    {
        private readonly UserManager<TUser> userManager;
        private readonly SignInManager<TUser> signInManager;
        private readonly IUserStore<TUser> userStore;
        private readonly JwtOptions jwtOptions;
        private readonly Options.IdentityOptions identityOptions;

        public IdentityService(
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IUserStore<TUser> userStore,
            IOptions<JwtOptions> jwtOptions,
            IOptions<Options.IdentityOptions> identityOptions)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.userStore = userStore;
            this.jwtOptions = jwtOptions.Value;
            this.identityOptions = identityOptions.Value;

            if (string.IsNullOrWhiteSpace(this.identityOptions.InitialRole))
            {
                throw new InvalidDataException($"{nameof(this.identityOptions.InitialRole)} can not be null, empty or whitespace");
            }
        }

        public async Task<IdentityResult> CreateUserAsync(RegisterRequestDto registerRequest, CancellationToken cancellationToken)
        {
            var user = new TUser();

            await userStore.SetUserNameAsync(user, registerRequest.UserName, cancellationToken);

            var creationResult = await userManager.CreateAsync(user, registerRequest.Password);
            if (creationResult.Succeeded)
            {
                return await userManager.AddToRoleAsync(user, identityOptions.InitialRole);
            }
            return creationResult;
        }

        public async Task<string> GenerateJwtTokenAsync(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>()
            {
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.UserName)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var token = GenerateJwtToken(claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<SignInResult> SignInAsync(string userName, string password)
        {
            return await signInManager.PasswordSignInAsync(userName, password, false, false);
        }

        public async Task SignOutAsync()
        {
            await signInManager.SignOutAsync();
        }

        private JwtSecurityToken GenerateJwtToken(List<Claim> claims)
        {
            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));
            var signingCredentials = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: jwtOptions.ValidIssuer,
                audience: jwtOptions.ValidAudience,
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: signingCredentials);
        }
    }
}
