using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiresAt { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public virtual ICollection<Transaction> TransactionApprovedByUsers { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionUsers { get; set; } = new List<Transaction>();
}
