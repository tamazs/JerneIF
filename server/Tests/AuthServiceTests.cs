using System.ComponentModel.DataAnnotations;
using Api.DTOs.Request;
using Api.Services;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests;

[Collection("IntegrationTests")]
public class AuthServiceTests
{
    private readonly IAuthService _authService;
    private readonly JerneDbContext _dbContext;

    public AuthServiceTests(IAuthService authService, JerneDbContext dbContext)
    {
        _authService = authService;
        _dbContext = dbContext;
    }

    private async Task ClearDatabase()
    {
        _dbContext.ChangeTracker.Clear();
        await _dbContext.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE \"Users\" RESTART IDENTITY CASCADE;");
    }

    [Fact]
    public async Task RegisterUser_ShouldCreateUser()
    {
        await ClearDatabase();

        var dto = new RegisterRequestDto
        {
            FullName = "Test User",
            Email = "user@test.com",
            Password = "TestPassword123!",
            PhoneNumber = "12345678"
        };

        var result = await _authService.RegisterUser(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Email, result.Email);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        Assert.NotNull(user);
        Assert.Equal("Player", user.Role);
        Assert.False(user.IsActive);
    }

    [Fact]
    public async Task RegisterUser_ShouldThrow_WhenEmailAlreadyExists()
    {
        await ClearDatabase();

        var dto = new RegisterRequestDto
        {
            FullName = "Duplicate User",
            Email = "dupe@test.com",
            Password = "StrongPass123!",
            PhoneNumber = "11111111"
        };

        await _authService.RegisterUser(dto);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.RegisterUser(dto));
    }

    [Fact]
    public async Task RegisterUser_ShouldThrow_WhenValidationFails()
    {
        await ClearDatabase();

        var dto = new RegisterRequestDto
        {
            FullName = "",
            Email = "",
            Password = "123",
            PhoneNumber = ""
        };

        await Assert.ThrowsAsync<ValidationException>(() =>
            _authService.RegisterUser(dto));
    }

    [Fact]
    public async Task LoginUser_ShouldReturnToken_WhenCredentialsAreValid()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Active User",
            Email = "login@test.com",
            PhoneNumber = "22222222",
            Role = "Player",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "StrongPass123!");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var dto = new LoginRequestDto
        {
            Email = "login@test.com",
            Password = "StrongPass123!"
        };

        var result = await _authService.LoginUser(dto);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.Equal(user.Email, result.User.Email);
    }

    [Fact]
    public async Task LoginUser_ShouldThrow_WhenPasswordIsWrong()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Wrong Password",
            Email = "wrongpass@test.com",
            PhoneNumber = "33333333",
            Role = "Player",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "CorrectPass123!");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var dto = new LoginRequestDto
        {
            Email = "wrongpass@test.com",
            Password = "WrongPass!"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginUser(dto));
    }

    [Fact]
    public async Task LoginUser_ShouldThrow_WhenUserIsInactive()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Inactive User",
            Email = "inactive@test.com",
            PhoneNumber = "44444444",
            Role = "Player",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "StrongPass123!");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var dto = new LoginRequestDto
        {
            Email = "inactive@test.com",
            Password = "StrongPass123!"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginUser(dto));
    }

    [Fact]
    public async Task RefreshTokens_ShouldReturnNewAccessAndRefreshToken_WhenValid()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Token User",
            Email = "refresh@test.com",
            PhoneNumber = "55555555",
            Role = "Player",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            RefreshToken = "validtoken123",
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "StrongPass123!");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var dto = new RefreshTokenRequestDto
        {
            RefreshToken = "validtoken123"
        };

        var result = await _authService.RefreshTokens(dto);

        Assert.NotNull(result);
        Assert.NotEqual("validtoken123", result.RefreshToken);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }

    [Fact]
    public async Task RefreshTokens_ShouldThrow_WhenTokenIsInvalid()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Invalid Refresh",
            Email = "invalidrefresh@test.com",
            PhoneNumber = "66666666",
            Role = "Player",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            RefreshToken = "validtoken123",
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "StrongPass123!");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var dto = new RefreshTokenRequestDto
        {
            RefreshToken = "WRONGTOKEN"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.RefreshTokens(dto));
    }

    [Fact]
    public async Task RefreshTokens_ShouldThrow_WhenTokenIsExpired()
    {
        await ClearDatabase();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Expired Token",
            Email = "expired@test.com",
            PhoneNumber = "77777777",
            Role = "Player",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            RefreshToken = "expiredtoken123",
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(-1)
        };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "StrongPass123!");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var dto = new RefreshTokenRequestDto
        {
            RefreshToken = "expiredtoken123"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.RefreshTokens(dto));
    }
}
