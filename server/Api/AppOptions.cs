using System.ComponentModel.DataAnnotations;

namespace Api;

public class AppOptions
{
    [Required] [MinLength(1)] public string DbConnectionString { get; set; }
}