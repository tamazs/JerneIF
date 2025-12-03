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
        IQueryable<Transaction> transactions = dbContext.Transactions.Include(t => t.User);
        
        transactions = sieveProcessor.Apply(sieveModel, transactions);
        
        return await transactions.Select(t => new TransactionDto(t)).ToListAsync();
    }

    public async Task<List<TransactionDto>> GetTransactionsByUserId(string userId, SieveModel sieveModel)
    {
        IQueryable<Transaction> transactions = dbContext.Transactions.Where(t => t.UserId == userId);
        
        transactions = sieveProcessor.Apply(sieveModel, transactions);
        
        return await transactions.Select(t => new TransactionDto(t)).ToListAsync();
    }

    public async Task<TransactionDto> CreateTransaction(string userId, CreateTransactionRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var transaction = new Transaction
        {
            TransactionId = Guid.NewGuid().ToString(),
            UserId = userId,
            MobilePayReference = dto.MobilePayReference,
            Amount = dto.Amount,
            Status = TransactionStatus.Pending.ToString(),
            CreatedAt = DateTime.UtcNow,
        };
        
        await dbContext.Transactions.AddAsync(transaction);
        await dbContext.SaveChangesAsync();
        
        //reloading to fix  Object reference not set to an instance of an object.
        //because of fullName of user attached for search from client
        var loaded = await dbContext.Transactions
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TransactionId == transaction.TransactionId);

        return new TransactionDto(loaded);
    }

    public async Task<TransactionDto> ApproveTransaction(string userId, string transactionId)
    {
        var transaction = await dbContext.Transactions.Include(t => t.User).FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        if (transaction == null)  throw new KeyNotFoundException("Transaction not found.");
        transaction.Status = TransactionStatus.Approved.ToString();
        transaction.ApprovedByUserId = userId;
        transaction.ApprovedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return new TransactionDto(transaction);
    }

    public async Task<TransactionDto> DenyTransaction(string userId, string transactionId)
    {
        var transaction = await dbContext.Transactions.Include(t => t.User).FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        if (transaction == null)  throw new KeyNotFoundException("Transaction not found.");
        transaction.Status = TransactionStatus.Rejected.ToString();
        transaction.ApprovedByUserId = userId;
        transaction.DeletedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return new TransactionDto(transaction);
    }
}