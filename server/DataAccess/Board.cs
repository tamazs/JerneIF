using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Board
{
    public string BoardId { get; set; } = null!;

    public string GameId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int NumberCount { get; set; }

    public bool IsRepeating { get; set; }

    public string? RepeatingUntilGameId { get; set; }

    public decimal Price { get; set; }

    public DateTime PurchasedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual BoardNumber? BoardNumber { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Game? RepeatingUntilGame { get; set; }

    public virtual User User { get; set; } = null!;
}
