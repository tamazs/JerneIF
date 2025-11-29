using Api.DTOs;
using Api.DTOs.Request;
using Sieve.Models;

namespace Api.Services;

public interface IBoardService
{
    Task<List<BoardDto>> GetAllBoards(SieveModel sieveModel);
    Task<List<BoardDto>> GetBoardsByUserId(string userId, SieveModel sieveModel);
    Task<decimal> GetBalance(string userId);
    Task<BoardDto> CreateBoard(string userId, AddBoardRequestDto dto);
}