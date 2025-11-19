using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Game
{
    public string GameId { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? PublishedAt { get; set; }

    public string? PublishedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Board> BoardGames { get; set; } = new List<Board>();

    public virtual ICollection<Board> BoardRepeatingUntilGames { get; set; } = new List<Board>();

    public virtual GameWinningNumber? GameWinningNumber { get; set; }

    public virtual User? PublishedByUser { get; set; }
}
