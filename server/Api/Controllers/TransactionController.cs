using Api.DTOs;
using Api.DTOs.Request;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace Api.Controllers;

[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost(nameof(CreateTransaction))]
    public async Task<TransactionDto> CreateTransaction([FromBody]CreateTransactionRequestDto dto)
    {
        return await _transactionService.CreateTransaction(dto);
    }
    
    [HttpPost(nameof(GetTransactions))]
    public async Task<List<TransactionDto>> GetTransactions([FromBody] SieveModel sieveModel)
    {
        return await _transactionService.GetTransactions(sieveModel);
    }
    
    [HttpPut(nameof(ApproveTransaction))]
    public async Task<TransactionDto> ApproveTransaction([FromBody]ApproveTransactionRequestDto dto) {
        return await _transactionService.ApproveTransaction(dto);
    }
    
    [HttpPut(nameof(DenyTransaction))]
    public async Task<TransactionDto> DenyTransaction([FromBody]ApproveTransactionRequestDto dto) {
        return await _transactionService.DenyTransaction(dto);
    }

    [HttpPost(nameof(GetTransactionsByUserId))]
    public async Task<List<TransactionDto>> GetTransactionsByUserId([FromQuery] string userId,
        [FromBody] SieveModel sieveModel)
    {
        return await _transactionService.GetTransactionsByUserId(userId, sieveModel);
    }
}