using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request;

public class UpdateUserRequestDto
{
    [MinLength(3)] [Required]
    public string? UserId { get; set; }
    
    [MinLength(3)] [Required]
    public string? FullName { get; set; }
    
    [MinLength(3)] [Required]
    public string? PhoneNumber { get; set; }
    
    [MinLength(3)] [Required]
    public string? Email { get; set; }
    
    [MinLength(6)] [Required]
    public string? CurrentPassword { get; set; }
    
    [MinLength(6)] [Required]
    public string? NewPassword { get; set; }
}