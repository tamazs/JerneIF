using System.ComponentModel.DataAnnotations;
using Api.DTOs;
using Api.DTOs.Request;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Api.Services;

public class BoardService(JerneDbContext dbContext, ISieveProcessor sieveProcessor, BalanceHelper balanceHelper, GameQueryHelper gameQueryHelper, TimeProvider? timeProvider = null) : IBoardService
{
    public async Task<List<BoardDto>> GetAllBoards(SieveModel sieveModel)
    {
        IQueryable<Board> boards = dbContext.Boards.Include(b => b.BoardNumber);
        
        boards = sieveProcessor.Apply(sieveModel, boards);

        return await boards.Select(b => new BoardDto(b)).ToListAsync();
    }

    public async Task<List<BoardDto>> GetBoardsByUserId(string userId, SieveModel sieveModel)
    {
        IQueryable<Board> boards = dbContext.Boards.Where(b => b.UserId == userId).Include(b => b.BoardNumber);
        
        boards = sieveProcessor.Apply(sieveModel, boards);

        return await boards.Select(b => new BoardDto(b)).ToListAsync();
    }

    public async Task<BoardDto> CreateBoard(string userId, AddBoardRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        // Serialize board purchases per user. The balance check and the insert must see each other:
        // without this lock, two parallel requests could both pass "already has a board" and
        // "enough balance" and both be saved. The lock is released when the transaction ends.
        await using var dbTransaction = await dbContext.Database.BeginTransactionAsync();
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT 1 FROM \"Users\" WHERE \"UserId\" = {userId} FOR UPDATE");

        var activeGame = await gameQueryHelper.GetActiveGame();

        var userAlreadyHasBoard =
            await dbContext.Boards.AnyAsync(b => b.UserId == userId && b.GameId == activeGame.GameId);

        if (userAlreadyHasBoard) throw new ValidationException("You have already submitted a board for this game.");
        
        var danishTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((timeProvider ?? TimeProvider.System).GetUtcNow().UtcDateTime, "Europe/Copenhagen");
        
        if (danishTime.DayOfWeek == DayOfWeek.Saturday && danishTime.Hour >= 17)
            throw new ValidationException("Boards cannot be submitted after Saturday 17:00.");

        if (danishTime.DayOfWeek == DayOfWeek.Sunday)
            throw new ValidationException("Boards cannot be submitted on Sunday.");
   
        var boardNumbers = new BoardNumber
        {
            BoardNumbersId = Guid.NewGuid().ToString(),
            BoardNumbers = dto.BoardNumbers
        };
        
        var balance = await balanceHelper.GetBalance(userId);

        var numberCount = boardNumbers.BoardNumbers.Count;
        var price = balanceHelper.CalculatePrice(numberCount);

        if (balance < price)
        {
            throw new ValidationException("Insufficient balance.");
        }

        var board = new Board
        {
            BoardId = Guid.NewGuid().ToString(),
            GameId = activeGame.GameId,
            UserId = userId,
            NumberCount = numberCount,
            IsRepeating = dto.IsRepeating,
            RepeatCount = dto.RepeatCount,
            Price = price,
            PurchasedAt = DateTime.UtcNow,
            BoardNumber = boardNumbers
        };
        
        await dbContext.Boards.AddAsync(board);
        await dbContext.SaveChangesAsync();
        await dbTransaction.CommitAsync();
        return new BoardDto(board);
    }
}