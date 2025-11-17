using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request;

public class LoginRequestDto
{
    [MinLength(3)] [Required]
    public string Email { get; set; } = string.Empty;
    
    [MinLength(3)] [Required]
    public string Password { get; set; } = string.Empty;
}