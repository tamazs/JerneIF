using Api.DTOs;
using Api.DTOs.Request;
using Sieve.Models;

namespace Api.Services;

public interface IBoardService
{
    Task<List<BoardDto>> GetAllBoards(SieveModel sieveModel);
    Task<BoardDto> CreateBoard(AddBoardRequestDto dto);
}