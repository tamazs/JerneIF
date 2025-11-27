using Api.DTOs;
using Api.DTOs.Request;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace Api.Controllers;

[ApiController]
public class BoardController : BaseController
{
    private readonly IBoardService _boardService;

    public BoardController(IBoardService boardService)
    {
        _boardService = boardService;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost(nameof(GetAllBoards))]
    public async Task<List<BoardDto>> GetAllBoards([FromBody]SieveModel sieveModel){
        return await _boardService.GetAllBoards(sieveModel);
    }

    [Authorize]
    [HttpPost(nameof(CreateBoard))]
    public async Task<BoardDto> CreateBoard(AddBoardRequestDto dto)
    {
        return await _boardService.CreateBoard(CurrentUserId, dto);
    }

    [Authorize]
    [HttpGet(nameof(GetBalance))]
    public async Task<decimal> GetBalance()
    {
        return await _boardService.GetBalance(CurrentUserId);
    }

    [Authorize]
    [HttpPost(nameof(GetBoardsByUserId))]
    public async Task<List<BoardDto>> GetBoardsByUserId([FromBody] SieveModel sieveModel)
    {
        return await _boardService.GetBoardsByUserId(CurrentUserId, sieveModel);
    }
}