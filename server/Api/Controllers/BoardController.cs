using Api.DTOs;
using Api.DTOs.Request;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace Api.Controllers;

[ApiController]
public class BoardController : ControllerBase
{
    private readonly IBoardService _boardService;

    public BoardController(IBoardService boardService)
    {
        _boardService = boardService;
    }
    
    [HttpPost(nameof(GetAllBoards))]
    public async Task<List<BoardDto>> GetAllBoards([FromBody]SieveModel sieveModel){
        return await _boardService.GetAllBoards(sieveModel);
    }

    [HttpPost(nameof(CreateBoard))]
    public async Task<BoardDto> CreateBoard(AddBoardRequestDto dto)
    {
        return await _boardService.CreateBoard(dto);
    }

    [HttpGet(nameof(GetBalance))]
    public async Task<decimal> GetBalance([FromQuery] string userId)
    {
        return await _boardService.GetBalance(userId);
    }

    [HttpPost(nameof(GetBoardsByUserId))]
    public async Task<List<BoardDto>> GetBoardsByUserId([FromQuery] string userId, [FromBody] SieveModel sieveModel)
    {
        return await _boardService.GetBoardsByUserId(userId, sieveModel);
    }
}