using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request;

public class ApproveTransactionRequestDto
{
    [MinLength(3)] [Required]
    public string UserId { get; set; } = null!;
    [MinLength(3)] [Required]
    public string TransactionId { get; set; } = null!;
}