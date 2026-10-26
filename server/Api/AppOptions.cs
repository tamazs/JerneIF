using System.ComponentModel.DataAnnotations;

namespace Api;

public class AppOptions
{
    [Required] [MinLength(1)] public string DbConnectionString { get; set; }
    [Required] [MinLength(1)] public string Token { get; set; }
    [Required] [MinLength(1)] public string Issuer { get; set; }
    [Required] [MinLength(1)] public string Audience { get; set; }

    /// <summary>Browser origins allowed by CORS. When empty, <see cref="Program.DefaultAllowedOrigins"/> is used.</summary>
    public string[]? AllowedOrigins { get; set; }
}