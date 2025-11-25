using System.ComponentModel.DataAnnotations;
using Api.DTOs;
using Api.DTOs.Request;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Api.Services;

public class GameService(JerneDbContext dbContext, ISieveProcessor sieveProcessor) : IGameService
{
    public async Task<List<GameDto>> GetAllGames(SieveModel sieveModel)
    {
        IQueryable<Game> games = dbContext.Games;
        
        games = sieveProcessor.Apply(sieveModel, games);

        return await games.Select(g => new GameDto(g)).ToListAsync();
    }

    public async Task<GameDto> CreateGame()
    {
        var game = new Game
        {
            GameId = Guid.NewGuid().ToString(),
            StartDate = DateTime.UtcNow,
            Status = GameStatus.Active.ToString(),
            CreatedAt = DateTime.UtcNow
        };
            
        await dbContext.Games.AddAsync(game);
        await  dbContext.SaveChangesAsync();
        return new GameDto(game);
    }
}