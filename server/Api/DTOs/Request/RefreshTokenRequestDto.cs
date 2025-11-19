using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request;

public class RefreshTokenRequestDto
{
    [MinLength(3)] [Required]
    public string UserId { get; set; }
    [MinLength(3)] [Required]
    public string RefreshToken { get; set; }
}