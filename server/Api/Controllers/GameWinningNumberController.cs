using Api.DTOs;
using Api.DTOs.Request;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class GameWinningNumberController : BaseController
{
    private readonly IGameWinningNumberService _gameWinningNumberService;

    public GameWinningNumberController(IGameWinningNumberService gameWinningNumberService)
    {
        _gameWinningNumberService = gameWinningNumberService;
    }

    [Authorize]
    [HttpGet(nameof(GetGameWinningNumbersForGame))]
    public async Task<GameWinningNumbersDto> GetGameWinningNumbersForGame([FromQuery] string gameId)
    {
        return await _gameWinningNumberService.GetGameWinningNumbersForGame(gameId);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost(nameof(AddGameWinningNumbers))]
    public async Task<GameWinningNumbersDto> AddGameWinningNumbers([FromBody]AddGameWinningNumbersDto dto)
    {
        return await _gameWinningNumberService.AddGameWinningNumbers(CurrentUserId, dto);
    }
}