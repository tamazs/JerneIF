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
        GameWinningNumber = game.GameWinningNumber?.GameWinningNumbers;
        PublishedByUser = game.PublishedByUser == null
            ? null
            : new UserDto
            {
                UserId = game.PublishedByUser.UserId,
                FullName = game.PublishedByUser.FullName
            };

        Winners = game.GameWinners
            .Select(w => new GameWinnerDto
            {
                FullName = w.User.FullName,                 // FULL NAME → done
                MatchedNumbers = w.MatchedNumbers.ToList()      // whatever column you have
            })
            .ToList();
    }
    
    public string GameId { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? PublishedAt { get; set; }

    public string? PublishedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    
    public List<int>? GameWinningNumber { get; set; }

    public UserDto? PublishedByUser { get; set; }
    
    public List<GameWinnerDto>? Winners { get; set; } = new();
}

public class GameWinnerDto
{
    public string FullName { get; set; }
    public List<int> MatchedNumbers { get; set; }
}
