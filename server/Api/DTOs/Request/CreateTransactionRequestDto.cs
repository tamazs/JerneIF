using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request;

public class CreateTransactionRequestDto
{
    [MinLength(3)] [Required]
    public string UserId { get; set; } = null!;
    [MinLength(3)] [Required]
    public string? MobilePayReference { get; set; }
    [Range(0.01, double.MaxValue)] [Required]
    public decimal Amount { get; set; }
}