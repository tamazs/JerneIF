using DataAccess;

namespace Api.DTOs;

public class GameWinningNumbersDto
{
    public GameWinningNumbersDto(GameWinningNumber gameWinningNumber)
    {
        GameWinningNumbersId = gameWinningNumber.GameWinningNumbersId;
        GameId = gameWinningNumber.GameId;
        GameWinningNumbers = gameWinningNumber.GameWinningNumbers;
    }
    public string GameWinningNumbersId { get; set; } = null!;

    public string GameId { get; set; } = null!;

    public List<int> GameWinningNumbers { get; set; } = null!;
}