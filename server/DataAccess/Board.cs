using System;
using System.Collections.Generic;
using Sieve.Attributes;

namespace DataAccess;

public partial class Board
{
    public string BoardId { get; set; } = null!;

    public string GameId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int NumberCount { get; set; }

    public bool IsRepeating { get; set; }

    public int RepeatCount { get; set; }

    public decimal Price { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime PurchasedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual BoardNumber? BoardNumber { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    
    public virtual ICollection<GameWinner> GameWinners { get; set; } = new List<GameWinner>();

}
