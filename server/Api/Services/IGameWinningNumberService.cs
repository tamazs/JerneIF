using Api.DTOs;
using Api.DTOs.Request;

namespace Api.Services;

public interface IGameWinningNumberService
{
    Task<GameWinningNumbersDto> GetGameWinningNumbersForGame(string gameId);
    Task<GameWinningNumbersDto> AddGameWinningNumbers(string userId, AddGameWinningNumbersDto dto);
}