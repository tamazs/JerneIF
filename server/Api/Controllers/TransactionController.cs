using Api.DTOs;
using Api.DTOs.Request;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace Api.Controllers;

[ApiController]
public class TransactionController : BaseController
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [Authorize]
    [HttpPost(nameof(CreateTransaction))]
    public async Task<TransactionDto> CreateTransaction([FromBody]CreateTransactionRequestDto dto)
    {
        return await _transactionService.CreateTransaction(CurrentUserId, dto);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost(nameof(GetTransactions))]
    public async Task<List<TransactionDto>> GetTransactions([FromBody] SieveModel sieveModel)
    {
        return await _transactionService.GetTransactions(sieveModel);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut(nameof(ApproveTransaction))]
    public async Task<TransactionDto> ApproveTransaction([FromBody]ApproveTransactionRequestDto dto) {
        return await _transactionService.ApproveTransaction(CurrentUserId, dto);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut(nameof(DenyTransaction))]
    public async Task<TransactionDto> DenyTransaction([FromBody]ApproveTransactionRequestDto dto) {
        return await _transactionService.DenyTransaction(CurrentUserId, dto);
    }

    [Authorize]
    [HttpPost(nameof(GetTransactionsByUserId))]
    public async Task<List<TransactionDto>> GetTransactionsByUserId([FromBody] SieveModel sieveModel)
    {
        return await _transactionService.GetTransactionsByUserId(CurrentUserId, sieveModel);
    }
}