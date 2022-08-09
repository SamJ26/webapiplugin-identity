using System.ComponentModel.DataAnnotations;

namespace WebApiPlugin.Identity.Dtos
{
    public record RegisterRequestDto
    {
        [Required]
        public string UserName { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        public string? ConfirmPassword { get; set; }
    }
}
