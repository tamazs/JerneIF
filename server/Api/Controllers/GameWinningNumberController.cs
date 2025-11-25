using Api.DTOs;
using Api.DTOs.Request;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class GameWinningNumberController : ControllerBase
{
    private readonly IGameWinningNumberService _gameWinningNumberService;

    public GameWinningNumberController(IGameWinningNumberService gameWinningNumberService)
    {
        _gameWinningNumberService = gameWinningNumberService;
    }

    [HttpPost(nameof(GetGameWinningNumbersForGame))]
    public async Task<GameWinningNumbersDto> GetGameWinningNumbersForGame([FromBody] string gameId)
    {
        return await _gameWinningNumberService.GetGameWinningNumbersForGame(gameId);
    }

    [HttpPost(nameof(AddGameWinningNumbers))]
    public async Task<GameWinningNumbersDto> AddGameWinningNumbers([FromBody]AddGameWinningNumbersDto dto)
    {
        return await _gameWinningNumberService.AddGameWinningNumbers(dto);
    }
}