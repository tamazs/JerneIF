using Api.DTOs;
using Api.DTOs.Request;
using Sieve.Models;

namespace Api.Services;

public interface ITransactionService
{
    Task<List<TransactionDto>> GetTransactions(SieveModel sieveModel);
    Task<List<TransactionDto>> GetTransactionsByUserId(string userId, SieveModel sieveModel);
    Task<TransactionDto> CreateTransaction(string userId, CreateTransactionRequestDto dto);
    Task<TransactionDto> ApproveTransaction(string userId, ApproveTransactionRequestDto dto);
    Task<TransactionDto> DenyTransaction(string userId, ApproveTransactionRequestDto dto);
}