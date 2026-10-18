using System.ComponentModel.DataAnnotations;
using Api;
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

        var result = await _userService.GetUserById(user.UserId, "admin-id", true);

        Assert.NotNull(result);
        Assert.Equal("Charlie", result.FullName);
    }

    [Fact]
    public async Task GetUserById_ShouldThrow_WhenUserNotFound()
    {
        await ClearDatabase();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _userService.GetUserById("nonexistent-id", "admin-id", true));
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

        var result = await _userService.UpdateUser(dto, "admin-id", true);

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

        var result = await _userService.UpdateUser(dto, trackedUser.UserId, false);

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
            _userService.UpdateUser(dto, user.UserId, false));
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
            _userService.UpdateUser(dto, "admin-id", true));
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

    private async Task<User> AddUser(string name, string role = "Player")
    {
        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = name,
            Email = $"{name.ToLower()}@test.com",
            PhoneNumber = "12345678",
            Role = role,
            IsActive = true,
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!")
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    private static UpdateUserRequestDto UpdateDtoFor(User user, string? role = null) => new()
    {
        UserId = user.UserId,
        FullName = user.FullName,
        PhoneNumber = user.PhoneNumber,
        Email = user.Email,
        Role = role ?? user.Role,
        IsActive = true
    };

    [Fact]
    public async Task UpdateUser_Player_CannotPromoteSelfToAdmin()
    {
        await ClearDatabase();
        var player = await AddUser("Pia");

        var result = await _userService.UpdateUser(UpdateDtoFor(player, "Admin"), player.UserId, false);

        Assert.Equal("Player", result.Role);
        var fromDb = await _dbContext.Users.AsNoTracking().FirstAsync(u => u.UserId == player.UserId);
        Assert.Equal("Player", fromDb.Role);
    }

    [Fact]
    public async Task UpdateUser_Player_CanUpdateOwnProfileFields()
    {
        await ClearDatabase();
        var player = await AddUser("Paul");
        var dto = UpdateDtoFor(player);
        dto.FullName = "Paul Renamed";

        var result = await _userService.UpdateUser(dto, player.UserId, false);

        Assert.Equal("Paul Renamed", result.FullName);
    }

    [Fact]
    public async Task UpdateUser_Player_CannotReactivateSelf()
    {
        await ClearDatabase();
        var player = await AddUser("Quinn");
        player.IsActive = false;
        await _dbContext.SaveChangesAsync();

        await _userService.UpdateUser(UpdateDtoFor(player), player.UserId, false);

        var fromDb = await _dbContext.Users.AsNoTracking().FirstAsync(u => u.UserId == player.UserId);
        Assert.False(fromDb.IsActive);
    }

    [Fact]
    public async Task UpdateUser_Player_CannotUpdateAnotherUser()
    {
        await ClearDatabase();
        var attacker = await AddUser("Mallory");
        var victim = await AddUser("Victor");
        var dto = UpdateDtoFor(victim);
        dto.FullName = "Hacked";

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _userService.UpdateUser(dto, attacker.UserId, false));

        var fromDb = await _dbContext.Users.AsNoTracking().FirstAsync(u => u.UserId == victim.UserId);
        Assert.Equal("Victor", fromDb.FullName);
    }

    [Fact]
    public async Task UpdateUser_Admin_CanUpdateAnotherUserAndChangeRole()
    {
        await ClearDatabase();
        var admin = await AddUser("Adam", "Admin");
        var player = await AddUser("Penny");

        var result = await _userService.UpdateUser(UpdateDtoFor(player, "Admin"), admin.UserId, true);

        Assert.Equal("Admin", result.Role);
    }

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("admin")]
    [InlineData("0")]
    public async Task UpdateUser_Admin_RejectsInvalidRole(string badRole)
    {
        await ClearDatabase();
        var admin = await AddUser("Alice", "Admin");
        var player = await AddUser("Pete");

        await Assert.ThrowsAsync<ValidationException>(() =>
            _userService.UpdateUser(UpdateDtoFor(player, badRole), admin.UserId, true));

        var fromDb = await _dbContext.Users.AsNoTracking().FirstAsync(u => u.UserId == player.UserId);
        Assert.Equal("Player", fromDb.Role);
    }

    [Fact]
    public async Task UpdateUser_Admin_WithoutRole_KeepsExistingRole()
    {
        await ClearDatabase();
        var admin = await AddUser("Anna", "Admin");
        var player = await AddUser("Pam");
        var dto = UpdateDtoFor(player);
        dto.Role = null;

        var result = await _userService.UpdateUser(dto, admin.UserId, true);

        Assert.Equal("Player", result.Role);
    }

    [Fact]
    public async Task GetUserById_Player_CanReadSelf()
    {
        await ClearDatabase();
        var player = await AddUser("Pablo");

        var result = await _userService.GetUserById(player.UserId, player.UserId, false);

        Assert.Equal("Pablo", result.FullName);
    }

    [Fact]
    public async Task GetUserById_Player_CannotReadAnotherUser()
    {
        await ClearDatabase();
        var attacker = await AddUser("Eve");
        var victim = await AddUser("Bob");

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _userService.GetUserById(victim.UserId, attacker.UserId, false));
    }

    [Fact]
    public async Task GetUserById_Admin_CanReadAnyUser()
    {
        await ClearDatabase();
        var admin = await AddUser("Alex", "Admin");
        var player = await AddUser("Pat");

        var result = await _userService.GetUserById(player.UserId, admin.UserId, true);

        Assert.Equal("Pat", result.FullName);
    }
}
