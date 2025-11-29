using System.ComponentModel.DataAnnotations;
using Api.DTOs;
using Api.DTOs.Request;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Api.Services;

public class BoardService(JerneDbContext dbContext, ISieveProcessor sieveProcessor) : IBoardService
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

    public async Task<decimal> GetBalance(string userId)
    {
        var totalDeposits = await dbContext.Transactions
            .Where(t => t.UserId == userId && t.Status == TransactionStatus.Approved.ToString())
            .SumAsync(t => (decimal?)t.Amount) ?? 0;
        
        var totalSpent = await dbContext.Boards
            .Where(b => b.UserId == userId)
            .SumAsync(b => (decimal?)b.Price) ?? 0;
        
        return totalDeposits - totalSpent;
    }

    public async Task<BoardDto> CreateBoard(string userId, AddBoardRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);
        
        var userAlreadyHasBoard = await dbContext.Boards.AnyAsync(b => b.UserId == userId && b.GameId == dto.GameId);

        if (userAlreadyHasBoard) throw new ValidationException("You have already submitted a board for this game.");
        
        var danishTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Europe/Copenhagen");
        
        if (danishTime.DayOfWeek == DayOfWeek.Saturday && danishTime.Hour >= 17)
            throw new ValidationException("Boards cannot be submitted after Saturday 17:00.");
        
        if (danishTime.DayOfWeek == DayOfWeek.Sunday)
            throw new ValidationException("Boards cannot be submitted on Sunday.");

        var boardNumbers = new BoardNumber
        {
            BoardNumbersId = Guid.NewGuid().ToString(),
            BoardNumbers = dto.BoardNumbers
        };
        
        var balance = await GetBalance(userId);

        var numberCount = boardNumbers.BoardNumbers.Count;
        var price = CalculatePrice(numberCount);

        if (balance < price)
        {
            throw new ValidationException("Insufficient balance.");
        }

        var board = new Board
        {
            BoardId = Guid.NewGuid().ToString(),
            GameId = dto.GameId,
            UserId = userId,
            NumberCount = numberCount,
            IsRepeating = dto.IsRepeating,
            RepeatingUntil = dto.RepeatingUntil,
            Price = price,
            PurchasedAt = DateTime.UtcNow,
            BoardNumber = boardNumbers
        };
        
        await dbContext.Boards.AddAsync(board);
        await dbContext.SaveChangesAsync();
        return new BoardDto(board);
    }
    
    private int CalculatePrice(int count)
    {
        return count switch
        {
            5 => 20,
            6 => 40,
            7 => 80,
            8 => 160,
            _ => throw new ArgumentOutOfRangeException(nameof(count), "Board must contain 5–8 numbers.")
        };
    }
}