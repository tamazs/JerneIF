using Api.DTOs;
using Api.DTOs.Request;
using Sieve.Models;

namespace Api.Services;

public interface IGameService
{
    Task<List<GameDto>> GetAllGames(SieveModel sieveModel);
    Task<GameDto> CreateGame();
}