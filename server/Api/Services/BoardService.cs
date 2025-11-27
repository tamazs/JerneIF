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
        IQueryable<Board> boards = dbContext.Boards;
        
        boards = sieveProcessor.Apply(sieveModel, boards);

        return await boards.Select(b => new BoardDto(b)).ToListAsync();
    }

    public async Task<BoardDto> CreateBoard(AddBoardRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);
        
        var userAlreadyHasBoard = await dbContext.Boards.AnyAsync(b => b.UserId == dto.UserId && b.GameId == dto.GameId);

        if (userAlreadyHasBoard) throw new ValidationException("You have already submitted a board for this game.");
        
        var danishTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Europe/Copenhagen");
        
        if (danishTime.DayOfWeek == DayOfWeek.Saturday && danishTime.Hour >= 17)
            throw new ValidationException("Boards cannot be submitted after Saturday 17:00.");
        
        if (danishTime.DayOfWeek == DayOfWeek.Sunday)
            throw new ValidationException("Boards cannot be submitted after on Sunday.");

        var boardNumbers = new BoardNumber
        {
            BoardNumbersId = Guid.NewGuid().ToString(),
            BoardNumbers = dto.BoardNumbers
        };

        var numberCount = boardNumbers.BoardNumbers.Count;
        var price = CalculatePrice(numberCount);

        var board = new Board
        {
            BoardId = Guid.NewGuid().ToString(),
            GameId = dto.GameId,
            UserId = dto.UserId,
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