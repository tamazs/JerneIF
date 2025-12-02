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
    
    public string? Role { get; set; }
    
    public bool? IsActive { get; set; }
    
    public string? CurrentPassword { get; set; }
    
    public string? NewPassword { get; set; }
}