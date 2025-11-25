using DataAccess;

namespace Api.DTOs;

public class GameDto
{
    public GameDto(Game game)
    {
        GameId = game.GameId;
        StartDate = game.StartDate;
        EndDate = game.EndDate;
        Status = game.Status;
        PublishedAt = game.PublishedAt;
        PublishedByUserId = game.PublishedByUserId;
        CreatedAt = game.CreatedAt;
        DeletedAt = game.DeletedAt;
    }
    
    public string GameId { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? PublishedAt { get; set; }

    public string? PublishedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}