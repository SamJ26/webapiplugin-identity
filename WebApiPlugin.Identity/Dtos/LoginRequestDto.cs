using System.ComponentModel.DataAnnotations;

namespace WebApiPlugin.Identity.Dtos
{
    public record LoginRequestDto
    {
        [Required]
        public string UserName { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
