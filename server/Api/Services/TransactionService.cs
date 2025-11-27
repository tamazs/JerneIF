using System.ComponentModel.DataAnnotations;
using Api.DTOs;
using Api.DTOs.Request;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Api.Services;

public class TransactionService(JerneDbContext dbContext, ISieveProcessor sieveProcessor) : ITransactionService
{
    public async Task<List<TransactionDto>> GetTransactions(SieveModel sieveModel)
    {
        IQueryable<Transaction> transactions = dbContext.Transactions;
        
        transactions = sieveProcessor.Apply(sieveModel, transactions);
        
        return await transactions.Select(t => new TransactionDto(t)).ToListAsync();
    }

    public async Task<List<TransactionDto>> GetTransactionsByUserId(string userId, SieveModel sieveModel)
    {
        IQueryable<Transaction> transactions = dbContext.Transactions.Where(t => t.UserId == userId);
        
        transactions = sieveProcessor.Apply(sieveModel, transactions);
        
        return await transactions.Select(t => new TransactionDto(t)).ToListAsync();
    }

    public async Task<TransactionDto> CreateTransaction(CreateTransactionRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var transaction = new Transaction
        {
            TransactionId = Guid.NewGuid().ToString(),
            UserId = dto.UserId,
            MobilePayReference = dto.MobilePayReference,
            Amount = dto.Amount,
            Status = TransactionStatus.Pending.ToString(),
            CreatedAt = DateTime.UtcNow,
        };
        
        await dbContext.Transactions.AddAsync(transaction);
        await dbContext.SaveChangesAsync();
        return new TransactionDto(transaction);
    }

    public async Task<TransactionDto> ApproveTransaction(ApproveTransactionRequestDto dto)
    {
        var transaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.TransactionId == dto.TransactionId);
        if (transaction == null)  throw new KeyNotFoundException("Transaction not found.");
        transaction.Status = TransactionStatus.Approved.ToString();
        transaction.ApprovedByUserId = dto.UserId;
        transaction.ApprovedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return new TransactionDto(transaction);
    }

    public async Task<TransactionDto> DenyTransaction(ApproveTransactionRequestDto dto)
    {
        var transaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.TransactionId == dto.TransactionId);
        if (transaction == null)  throw new KeyNotFoundException("Transaction not found.");
        transaction.Status = TransactionStatus.Rejected.ToString();
        transaction.ApprovedByUserId = dto.UserId;
        transaction.DeletedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return new TransactionDto(transaction);
    }
}