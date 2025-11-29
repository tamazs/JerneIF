using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace Api.Controllers;

[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [Authorize]
    [HttpPost(nameof(GetAllGames))]
    public async Task<List<GameDto>> GetAllGames([FromBody] SieveModel sieveModel)
    {
        return await _gameService.GetAllGames(sieveModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost(nameof(CreateGame))]
    public async Task<GameDto> CreateGame()
    {
        return await _gameService.CreateGame();
    }
}