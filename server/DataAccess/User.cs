using System;
using System.Collections.Generic;
using Sieve.Attributes;

namespace DataAccess;

public partial class User
{
    public string UserId { get; set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    [Sieve(CanFilter = true, CanSort = true)]
    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
    [Sieve(CanFilter = true, CanSort = true)]
    public string Role { get; set; } = null!;
    [Sieve(CanFilter = true, CanSort = true)]
    public bool IsActive { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime CreatedAt { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime? UpdatedAt { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime? DeletedAt { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiresAt { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public virtual ICollection<Transaction> TransactionApprovedByUsers { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionUsers { get; set; } = new List<Transaction>();
    
    public virtual ICollection<GameWinner> GameWinners { get; set; } = new List<GameWinner>();

}
