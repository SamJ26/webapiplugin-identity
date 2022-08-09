namespace WebApiPlugin.Identity.Options
{
    public class JwtOptions
    {
        public string ValidAudience { get; set; } = default!;
        public string ValidIssuer { get; set; } = default!;
        public string Secret { get; set; } = default!;
    }
}
