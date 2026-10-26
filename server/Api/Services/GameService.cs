using System.ComponentModel.DataAnnotations;
using Api.DTOs;
using Api.DTOs.Request;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Api.Services;

public class GameService(JerneDbContext dbContext, ISieveProcessor sieveProcessor, BalanceHelper balanceHelper) : IGameService
{
    public async Task<List<GameDto>> GetAllGames(SieveModel sieveModel)
    {
        IQueryable<Game> games = dbContext.Games
            .Include(g => g.PublishedByUser)
            .Include(g => g.GameWinningNumber)
            .Include(g => g.GameWinners)
            .ThenInclude(gw => gw.User);
        
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
        
        var repeatingBoardsFromDb = await dbContext.Boards
            .Include(b => b.BoardNumber)
            .Where(b => b.IsRepeating && b.RepeatCount > 0)
            .OrderByDescending(b => b.PurchasedAt)
            .ToListAsync();
        
        var repeatingBoards = repeatingBoardsFromDb
            .GroupBy(b => b.UserId)
            .Select(g => g.First())
            .ToList();
        
        foreach (var oldBoard in repeatingBoards)
        {
            if (!oldBoard.IsRepeating || oldBoard.RepeatCount <= 0)
                continue;

            var balance = await balanceHelper.GetBalance(oldBoard.UserId);

            var numberCount = oldBoard.BoardNumber.BoardNumbers.Count;
            var price = balanceHelper.CalculatePrice(numberCount);

            // The old board keeps the RepeatCount it was bought with. IsRepeating = false only means
            // "the repeat chain is no longer continued from this board". The copy in the new game carries
            // the remaining repeats, so exactly one board per user keeps repeating.
            oldBoard.IsRepeating = false;

            if (balance < price)
                continue;

            var remainingRepeats = oldBoard.RepeatCount - 1;

            var boardNumbers = new BoardNumber
            {
                BoardNumbersId = Guid.NewGuid().ToString(),
                BoardNumbers = oldBoard.BoardNumber.BoardNumbers,
            };

            var newBoard = new Board
            {
                BoardId = Guid.NewGuid().ToString(),
                GameId = game.GameId,
                UserId = oldBoard.UserId,
                NumberCount = numberCount,
                Price = price,
                PurchasedAt = DateTime.UtcNow,
                BoardNumber = boardNumbers,
                IsRepeating = remainingRepeats > 0,
                RepeatCount = remainingRepeats
            };

            await dbContext.Boards.AddAsync(newBoard);
        }
        
        await dbContext.SaveChangesAsync();
        
        return new GameDto(game);
    }
}