using Api.DTOs;
using Api.DTOs.Request;
using Sieve.Models;

namespace Api.Services;

public interface ITransactionService
{
    Task<List<TransactionDto>> GetTransactions(SieveModel sieveModel);
    Task<TransactionDto> CreateTransaction(CreateTransactionRequestDto dto);
    Task<TransactionDto> ApproveTransaction(ApproveTransactionRequestDto dto);
    Task<TransactionDto> DenyTransaction(ApproveTransactionRequestDto dto);
}