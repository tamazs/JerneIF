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