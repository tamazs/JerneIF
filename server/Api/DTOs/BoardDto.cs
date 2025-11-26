using DataAccess;

namespace Api.DTOs;

public class BoardDto
{
    public BoardDto(Board board)
    {
        BoardId = board.BoardId;
        GameId = board.GameId;
        UserId = board.UserId;
        NumberCount = board.NumberCount;
        IsRepeating = board.IsRepeating;
        RepeatingUntil = board.RepeatingUntil;
        Price = board.Price;
        PurchasedAt = board.PurchasedAt;
        BoardNumbers = board.BoardNumber?.BoardNumbers ?? new List<int>();
    }
    public string BoardId { get; set; } = null!;

    public string GameId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int NumberCount { get; set; }

    public bool IsRepeating { get; set; }

    public DateTime? RepeatingUntil { get; set; }

    public decimal Price { get; set; }

    public DateTime PurchasedAt { get; set; }
    public List<int> BoardNumbers { get; set; } = new();
}