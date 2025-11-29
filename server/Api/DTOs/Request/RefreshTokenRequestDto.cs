using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request;

public class RefreshTokenRequestDto
{
    [MinLength(3)] [Required]
    public string RefreshToken { get; set; }
}