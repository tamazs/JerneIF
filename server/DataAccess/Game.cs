using System;
using System.Collections.Generic;
using Sieve.Attributes;

namespace DataAccess;

public partial class Game
{
    public string GameId { get; set; } = null!;
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime StartDate { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime EndDate { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public string Status { get; set; } = null!;

    public DateTime? PublishedAt { get; set; }

    public string? PublishedByUserId { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Board> BoardGames { get; set; } = new List<Board>();

    public virtual ICollection<Board> BoardRepeatingUntilGames { get; set; } = new List<Board>();

    public virtual GameWinningNumber? GameWinningNumber { get; set; }

    public virtual User? PublishedByUser { get; set; }
}
