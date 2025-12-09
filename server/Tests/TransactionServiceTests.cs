using System.ComponentModel.DataAnnotations;
using Api.DTOs.Request;
using Api.Services;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Xunit;

namespace Tests;

[Collection("IntegrationTests")]
public class TransactionServiceTests
{
    private readonly ITransactionService _transactionService;
    private readonly JerneDbContext _dbContext;

    public TransactionServiceTests(ITransactionService transactionService, JerneDbContext dbContext)
    {
        _transactionService = transactionService;
        _dbContext = dbContext;
    }

    private async Task ClearDatabase()
    {
        _dbContext.ChangeTracker.Clear();
        await _dbContext.Database.ExecuteSqlRawAsync(@"
            TRUNCATE TABLE ""Transactions"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""Users"" RESTART IDENTITY CASCADE;
        ");
    }

    [Fact]
    public async Task GetTransactions_ShouldReturnList()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Alice",
            Email = "alice@test.com",
            PhoneNumber = "12345678",
            Role = "Player",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!"),
            IsActive = true
        };
        await _dbContext.Users.AddAsync(user);

        await _dbContext.Transactions.AddRangeAsync(
            new Transaction { TransactionId = Guid.NewGuid().ToString(), UserId = user.UserId, Amount = 100, MobilePayReference = "REF1", Status = "Pending", CreatedAt = DateTime.UtcNow },
            new Transaction { TransactionId = Guid.NewGuid().ToString(), UserId = user.UserId, Amount = 200, MobilePayReference = "REF2", Status = "Approved", CreatedAt = DateTime.UtcNow }
        );
        await _dbContext.SaveChangesAsync();

        var sieve = new SieveModel();
        var result = await _transactionService.GetTransactions(sieve);

        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetTransactions_ShouldReturnEmpty_WhenNoneExist()
    {
        await ClearDatabase();

        var sieve = new SieveModel();
        var result = await _transactionService.GetTransactions(sieve);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTransactionsByUserId_ShouldReturnUserTransactions()
    {
        await ClearDatabase();

        var user1 = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Bob",
            Email = "bob@test.com",
            PhoneNumber = "12345678",
            Role = "Player",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!"),
            IsActive = true
        };
        var user2 = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Charlie",
            Email = "charlie@test.com",
            PhoneNumber = "87654321",
            Role = "Player",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!"),
            IsActive = true
        };

        await _dbContext.Users.AddRangeAsync(user1, user2);

        await _dbContext.Transactions.AddRangeAsync(
            new Transaction { TransactionId = Guid.NewGuid().ToString(), UserId = user1.UserId, Amount = 50, MobilePayReference = "TX1", Status = "Pending", CreatedAt = DateTime.UtcNow },
            new Transaction { TransactionId = Guid.NewGuid().ToString(), UserId = user2.UserId, Amount = 150, MobilePayReference = "TX2", Status = "Pending", CreatedAt = DateTime.UtcNow }
        );
        await _dbContext.SaveChangesAsync();

        var sieve = new SieveModel();
        var result = await _transactionService.GetTransactionsByUserId(user1.UserId, sieve);

        Assert.Single(result);
        Assert.Equal(user1.UserId, result.First().UserId);
    }

    [Fact]
    public async Task CreateTransaction_ShouldCreateTransactionSuccessfully()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Daisy",
            Email = "daisy@test.com",
            PhoneNumber = "55555555",
            Role = "Player",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!"),
            IsActive = true
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var dto = new CreateTransactionRequestDto
        {
            MobilePayReference = "MP123",
            Amount = 250
        };

        var result = await _transactionService.CreateTransaction(user.UserId, dto);

        Assert.NotNull(result);
        Assert.Equal("Pending", result.Status);
        Assert.Equal(dto.Amount, result.Amount);
        Assert.Equal(user.UserId, result.UserId);

        var saved = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.TransactionId == result.TransactionId);
        Assert.NotNull(saved);
    }

    [Fact]
    public async Task CreateTransaction_ShouldThrow_WhenValidationFails()
    {
        await ClearDatabase();

        var userId = Guid.NewGuid().ToString();
        var dto = new CreateTransactionRequestDto
        {
            MobilePayReference = "",
            Amount = -50
        };

        await Assert.ThrowsAsync<ValidationException>(() =>
            _transactionService.CreateTransaction(userId, dto));
    }

    [Fact]
    public async Task ApproveTransaction_ShouldSetApprovedStatus()
    {
        await ClearDatabase();

        var admin = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Admin",
            Email = "admin@test.com",
            PhoneNumber = "11111111",
            Role = "Admin",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!"),
            IsActive = true
        };

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Edward",
            Email = "ed@test.com",
            PhoneNumber = "22222222",
            Role = "Player",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!"),
            IsActive = true
        };

        await _dbContext.Users.AddRangeAsync(admin, user);

        var transaction = new Transaction
        {
            TransactionId = Guid.NewGuid().ToString(),
            UserId = user.UserId,
            Amount = 500,
            MobilePayReference = "APPROVE1",
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();

        var result = await _transactionService.ApproveTransaction(admin.UserId, transaction.TransactionId);

        Assert.Equal("Approved", result.Status);
        Assert.Equal(admin.UserId, result.ApprovedByUserId);
        Assert.NotNull(result.ApprovedAt);
    }

    [Fact]
    public async Task ApproveTransaction_ShouldThrow_WhenNotFound()
    {
        await ClearDatabase();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _transactionService.ApproveTransaction("admin-id", "fake-transaction-id"));
    }

    [Fact]
    public async Task DenyTransaction_ShouldSetRejectedStatus()
    {
        await ClearDatabase();

        var admin = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Admin",
            Email = "admin@test.com",
            PhoneNumber = "11111111",
            Role = "Admin",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!"),
            IsActive = true
        };

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Fiona",
            Email = "fiona@test.com",
            PhoneNumber = "99999999",
            Role = "Player",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!"),
            IsActive = true
        };

        await _dbContext.Users.AddRangeAsync(admin, user);

        var transaction = new Transaction
        {
            TransactionId = Guid.NewGuid().ToString(),
            UserId = user.UserId,
            Amount = 300,
            MobilePayReference = "REJECT1",
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();

        var result = await _transactionService.DenyTransaction(admin.UserId, transaction.TransactionId);

        Assert.Equal("Rejected", result.Status);
        Assert.Equal(admin.UserId, result.ApprovedByUserId);
        Assert.NotNull(result.DeletedAt);
    }

    [Fact]
    public async Task DenyTransaction_ShouldThrow_WhenNotFound()
    {
        await ClearDatabase();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _transactionService.DenyTransaction("admin-id", "fake-transaction-id"));
    }
}
