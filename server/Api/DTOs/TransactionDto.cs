using DataAccess;

namespace Api.DTOs;

public class TransactionDto
{
    public TransactionDto(Transaction transaction)
    {
        TransactionId = transaction.TransactionId;
        UserId = transaction.UserId;
        MobilePayReference = transaction.MobilePayReference;
        Amount = transaction.Amount;
        Status = transaction.Status;
        ApprovedByUserId = transaction.ApprovedByUserId;
        CreatedAt = transaction.CreatedAt;
        ApprovedAt = transaction.ApprovedAt;
        DeletedAt = transaction.DeletedAt;
    }
    
    public string TransactionId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? MobilePayReference { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public string? ApprovedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime? ApprovedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}