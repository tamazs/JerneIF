using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Transaction
{
    public string TransactionId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? MobilePayReference { get; set; }

    public decimal Amount { get; set; }
    
    public TransactionStatus Status { get; set; }

    public string? ApprovedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User? ApprovedByUser { get; set; }

    public virtual User User { get; set; } = null!;
}
