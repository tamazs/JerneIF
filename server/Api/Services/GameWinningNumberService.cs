using System.ComponentModel.DataAnnotations;
using Api.DTOs;
using Api.DTOs.Request;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class GameWinningNumberService(JerneDbContext dbContext, IGameService gameService) : IGameWinningNumberService
{
    public async Task<GameWinningNumbersDto> GetGameWinningNumbersForGame(string gameId)
    {
        var gameWinningNumbers = await dbContext.GameWinningNumbers.FirstOrDefaultAsync(g => g.GameId == gameId);
        
        if (gameWinningNumbers == null) throw new KeyNotFoundException("Game winning numbers not found.");
        
        return new GameWinningNumbersDto(gameWinningNumbers);
    }

    public async Task<GameWinningNumbersDto> AddGameWinningNumbers(AddGameWinningNumbersDto dto)
{
    Validator.ValidateObject(dto, new ValidationContext(dto), true);
    
    var game = await dbContext.Games
        .Include(g => g.Boards)
            .ThenInclude(b => b.BoardNumber)
        .FirstOrDefaultAsync(g => g.GameId == dto.GameId);

    if (game == null)
        throw new KeyNotFoundException("Game not found.");

    if (await dbContext.GameWinningNumbers.AnyAsync(x => x.GameId == dto.GameId))
        throw new ValidationException("Winning numbers already exist for this game.");

    if (dto.GameWinningNumbers == null || dto.GameWinningNumbers.Count != 3 || dto.GameWinningNumbers.Any(n => n < 1 || n > 16))
        throw new ValidationException("Exactly 3 numbers between 1 and 16 are required.");
    
    await using var tx = await dbContext.Database.BeginTransactionAsync();

    try
    {
        await CloseGame(game, dto.UserId);

        var gameWinningNumbers = new GameWinningNumber
        {
            GameWinningNumbersId = Guid.NewGuid().ToString(),
            GameId = dto.GameId,
            GameWinningNumbers = dto.GameWinningNumbers
        };
        
        var winners = new List<GameWinner>();

        foreach (var board in game.Boards)
        {
            if (board.BoardNumber == null || board.BoardNumber.BoardNumbers == null) 
                continue;
            
            var matched = board.BoardNumber.BoardNumbers
                .Intersect(dto.GameWinningNumbers)
                .ToList();

            if (matched.Count <= 0) 
                continue;

            winners.Add(new GameWinner
            {
                WinnerId = Guid.NewGuid().ToString(),
                GameId = game.GameId,
                UserId = board.UserId,
                BoardId = board.BoardId,
                MatchedNumbers = matched,
                WonAt = DateTime.UtcNow
            });
        }
        
        await dbContext.GameWinningNumbers.AddAsync(gameWinningNumbers);

        if (winners.Count > 0)
            await dbContext.GameWinners.AddRangeAsync(winners);

        await dbContext.SaveChangesAsync();
        
        await tx.CommitAsync();
        
        await gameService.CreateGame();

        return new GameWinningNumbersDto(gameWinningNumbers);
    }
    catch
    {
        await tx.RollbackAsync();
        throw;
    }
}
    
    private async Task CloseGame(Game game, string publishedByUserId)
    {
        game.Status = GameStatus.Finished.ToString();
        game.PublishedAt = DateTime.UtcNow;
        game.PublishedByUserId = publishedByUserId;

        await dbContext.SaveChangesAsync();
    }

}