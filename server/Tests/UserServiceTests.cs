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
public class UserServiceTests
{
    private readonly IUserService _userService;
    private readonly JerneDbContext _dbContext;

    public UserServiceTests(IUserService userService, JerneDbContext dbContext)
    {
        _userService = userService;
        _dbContext = dbContext;
    }

    private async Task ClearDatabase()
    {
        _dbContext.ChangeTracker.Clear();
        await _dbContext.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE \"Users\" RESTART IDENTITY CASCADE;");
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnFilteredResults()
    {
        await ClearDatabase();

        await _dbContext.Users.AddRangeAsync(
            new User
            {
                UserId = Guid.NewGuid().ToString(),
                FullName = "Alice",
                Email = "a@test.com",
                Role = "Player",
                PhoneNumber = "12345678",
                IsActive = true,
                PasswordHash = new PasswordHasher<User>().HashPassword(new User(), "Password123!")
            },
            new User
            {
                UserId = Guid.NewGuid().ToString(),
                FullName = "Bob",
                Email = "b@test.com",
                Role = "Player",
                PhoneNumber = "12345678",
                IsActive = true,
                PasswordHash = new PasswordHasher<User>().HashPassword(new User(), "Password123!")
            }
        );
        await _dbContext.SaveChangesAsync();

        var sieve = new SieveModel { Filters = "FullName@=Alice" };
        var result = await _userService.GetAllUsers(sieve);

        Assert.NotEmpty(result);
        Assert.Single(result);
        Assert.Equal("Alice", result[0].FullName);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnEmpty_WhenNoUsersExist()
    {
        await ClearDatabase();

        var sieve = new SieveModel();
        var result = await _userService.GetAllUsers(sieve);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenExists()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Charlie",
            Email = "charlie@test.com",
            Role = "Player",
            PhoneNumber = "12345678",
            IsActive = true,
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!")
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var result = await _userService.GetUserById(user.UserId);

        Assert.NotNull(result);
        Assert.Equal("Charlie", result.FullName);
    }

    [Fact]
    public async Task GetUserById_ShouldThrow_WhenUserNotFound()
    {
        await ClearDatabase();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _userService.GetUserById("nonexistent-id"));
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateFields_WhenValid()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "David",
            Email = "david@test.com",
            PhoneNumber = "12345678",
            Role = "Player",
            IsActive = true,
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "StrongPass123!")
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var dto = new UpdateUserRequestDto
        {
            UserId = user.UserId,
            FullName = "David Updated",
            PhoneNumber = "87654321",
            Email = "updated@test.com",
            Role = "Admin",
            IsActive = true
        };

        var result = await _userService.UpdateUser(dto);

        Assert.Equal("David Updated", result.FullName);
        Assert.Equal("Admin", result.Role);
    }

    [Fact]
    public async Task UpdateUser_ShouldChangePassword_WhenValidCurrentPassword()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Emma",
            Email = "emma@test.com",
            Role = "Player",
            PhoneNumber = "12345678",
            PasswordHash = new PasswordHasher<User>().HashPassword(new User(), "OldPassword123!"),
            IsActive = true
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        _dbContext.ChangeTracker.Clear();
        var trackedUser = await _dbContext.Users.AsTracking()
            .FirstAsync(u => u.UserId == user.UserId);

        var dto = new UpdateUserRequestDto
        {
            UserId = trackedUser.UserId,
            FullName = trackedUser.FullName,
            Email = trackedUser.Email,
            PhoneNumber = trackedUser.PhoneNumber,
            Role = trackedUser.Role,
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword456!"
        };

        var result = await _userService.UpdateUser(dto);

        Assert.NotNull(result);
        Assert.Equal(trackedUser.Email, result.Email);

        var refreshed = await _dbContext.Users.FirstAsync(u => u.UserId == user.UserId);
        var passwordHasher = new PasswordHasher<User>();
        var verification = passwordHasher.VerifyHashedPassword(
            refreshed,
            refreshed.PasswordHash,
            "NewPassword456!");
        Assert.Equal(PasswordVerificationResult.Success, verification);
    }

    [Fact]
    public async Task UpdateUser_ShouldThrow_WhenWrongCurrentPassword()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Fred",
            Email = "fred@test.com",
            Role = "Player",
            PhoneNumber = "12345678",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "CorrectPass!"),
            IsActive = true
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var dto = new UpdateUserRequestDto
        {
            UserId = user.UserId,
            FullName = "Fred",
            Email = "fred@test.com",
            PhoneNumber = "12345678",
            Role = "Player",
            CurrentPassword = "WrongPass!",
            NewPassword = "NewPass!"
        };

        await Assert.ThrowsAsync<ValidationException>(() =>
            _userService.UpdateUser(dto));
    }

    [Fact]
    public async Task UpdateUser_ShouldThrow_WhenUserNotFound()
    {
        await ClearDatabase();

        var dto = new UpdateUserRequestDto
        {
            UserId = "missing-id",
            FullName = "Nobody",
            Email = "no@test.com",
            PhoneNumber = "00000000",
            Role = "Player"
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _userService.UpdateUser(dto));
    }

    [Fact]
    public async Task DeleteUser_ShouldMarkAsInactive()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Greg",
            Email = "greg@test.com",
            IsActive = true,
            PhoneNumber = "12345678",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!"),
            Role = "Player"
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var result = await _userService.DeleteUser(user.UserId);

        Assert.False(result.IsActive);

        var userFromDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == result.UserId);
        Assert.NotNull(userFromDb);
        Assert.NotNull(userFromDb.DeletedAt);
    }

    [Fact]
    public async Task DeleteUser_ShouldThrow_WhenUserNotFound()
    {
        await ClearDatabase();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _userService.DeleteUser("fake-id"));
    }
}
