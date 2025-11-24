using System;
using System.Collections.Generic;
using Sieve.Attributes;

namespace DataAccess;

public partial class Transaction
{
    public string TransactionId { get; set; } = null!;
    [Sieve(CanFilter = true, CanSort = true)]
    public string UserId { get; set; } = null!;
    [Sieve(CanFilter = true, CanSort = true)]
    public string? MobilePayReference { get; set; }

    public decimal Amount { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public string Status { get; set; } = null!;

    public string? ApprovedByUserId { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime CreatedAt { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime? ApprovedAt { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime? DeletedAt { get; set; }

    public virtual User? ApprovedByUser { get; set; }

    public virtual User User { get; set; } = null!;
}
