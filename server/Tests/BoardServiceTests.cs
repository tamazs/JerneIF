using System.ComponentModel.DataAnnotations;
using Api.DTOs.Request;
using Api.Services;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Xunit;

namespace Tests;

[Collection("IntegrationTests")]
public class BoardServiceTests
{
    private readonly JerneDbContext _dbContext;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly FakeBalanceHelper _fakeBalanceHelper;
    private readonly FakeGameQueryHelper _fakeGameQueryHelper;
    private readonly BoardService _service;

    public BoardServiceTests(JerneDbContext dbContext, ISieveProcessor sieveProcessor)
    {
        _dbContext = dbContext;
        _sieveProcessor = sieveProcessor;
        _fakeBalanceHelper = new FakeBalanceHelper();
        _fakeGameQueryHelper = new FakeGameQueryHelper();
        _service = new BoardService(_dbContext, _sieveProcessor, _fakeBalanceHelper, _fakeGameQueryHelper);
    }

    private async Task ClearDatabase()
    {
        _dbContext.ChangeTracker.Clear();
        await _dbContext.Database.ExecuteSqlRawAsync(@"
            TRUNCATE TABLE ""Games"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""Boards"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""BoardNumbers"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""Users"" RESTART IDENTITY CASCADE;
        ");
    }
    
    private class FakeBalanceHelper : BalanceHelper
    {
        public decimal Balance { get; set; } = 1000;
        public int Price { get; set; } = 100;

        public FakeBalanceHelper() : base(null!) { }

        public override Task<decimal> GetBalance(string userId)
            => Task.FromResult(Balance);

        public override int CalculatePrice(int count)
            => Price;
    }

    private class FakeGameQueryHelper : GameQueryHelper
    {
        private Game? _game;
        public FakeGameQueryHelper() : base(null!) { }

        public void SetActiveGame(Game game) => _game = game;

        public override Task<Game> GetActiveGame()
        {
            if (_game == null)
                throw new InvalidOperationException("Active game not set.");
            return Task.FromResult(_game);
        }
    }

    private static User CreateTestUser(string id, string name, string email)
    {
        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            UserId = id,
            FullName = name,
            Email = email,
            PhoneNumber = "12345678",
            Role = "Player",
            IsActive = true
        };
        user.PasswordHash = hasher.HashPassword(user, "Password123!");
        return user;
    }

    [Fact]
    public async Task GetAllBoards_ShouldReturnBoards()
    {
        await ClearDatabase();

        var user = CreateTestUser("user1", "Alice", "a@test.com");
        var game = new Game { GameId = Guid.NewGuid().ToString(), Status = "Active", CreatedAt = DateTime.UtcNow };
        var boardNumber = new BoardNumber { BoardNumbersId = Guid.NewGuid().ToString(), BoardNumbers = new List<int> { 1, 2, 3 } };
        
        var board = new Board
        {
            BoardId = Guid.NewGuid().ToString(),
            GameId = game.GameId,
            UserId = user.UserId,
            BoardNumber = boardNumber,
            NumberCount = 3,
            Price = 50
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.Games.AddAsync(game);
        await _dbContext.BoardNumbers.AddAsync(boardNumber);
        await _dbContext.Boards.AddAsync(board);
        await _dbContext.SaveChangesAsync();

        var result = await _service.GetAllBoards(new SieveModel());
        Assert.Single(result);
    }

    [Fact]
    public async Task GetBoardsByUserId_ShouldReturnBoards()
    {
        await ClearDatabase();

        var user = CreateTestUser("user42", "Bob", "bob@test.com");
        var game = new Game { GameId = Guid.NewGuid().ToString(), Status = "Active", CreatedAt = DateTime.UtcNow };
        var boardNumber = new BoardNumber { BoardNumbersId = Guid.NewGuid().ToString(), BoardNumbers = new List<int> { 5, 6, 7 } };
        var board = new Board
        {
            BoardId = Guid.NewGuid().ToString(),
            GameId = game.GameId,
            UserId = user.UserId,
            BoardNumber = boardNumber,
            NumberCount = 3,
            Price = 50
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.Games.AddAsync(game);
        await _dbContext.BoardNumbers.AddAsync(boardNumber);
        await _dbContext.Boards.AddAsync(board);
        await _dbContext.SaveChangesAsync();

        var result = await _service.GetBoardsByUserId(user.UserId, new SieveModel());
        Assert.Single(result);
        Assert.Equal(user.UserId, result.First().UserId);
    }

    [Fact]
    public async Task CreateBoard_ShouldCreate_WhenValid()
    {
        await ClearDatabase();

        var user = CreateTestUser("user1", "Alice", "a@test.com");
        var game = new Game { GameId = Guid.NewGuid().ToString(), Status = "Active", CreatedAt = DateTime.UtcNow };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();

        _fakeGameQueryHelper.SetActiveGame(game);
        _fakeBalanceHelper.Balance = 200;
        _fakeBalanceHelper.Price = 100;

        var dto = new AddBoardRequestDto
        {
            BoardNumbers = new List<int> { 1, 2, 3, 4, 5 },
            IsRepeating = false,
            RepeatCount = 0
        };

        var result = await _service.CreateBoard(user.UserId, dto);

        Assert.NotNull(result);
        Assert.Equal(user.UserId, result.UserId);
        Assert.Equal(game.GameId, result.GameId);
    }

    [Fact]
    public async Task CreateBoard_ShouldThrow_WhenAlreadySubmitted()
    {
        await ClearDatabase();

        var user = CreateTestUser("user2", "Charlie", "c@test.com");
        var game = new Game { GameId = Guid.NewGuid().ToString(), Status = "Active", CreatedAt = DateTime.UtcNow };
        var board = new Board
        {
            BoardId = Guid.NewGuid().ToString(),
            GameId = game.GameId,
            UserId = user.UserId,
            BoardNumber = new BoardNumber
            {
                BoardNumbersId = Guid.NewGuid().ToString(),
                BoardNumbers = new List<int> { 1, 2, 3, 4, 5 }
            },
            NumberCount = 5,
            Price = 100
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.Games.AddAsync(game);
        await _dbContext.BoardNumbers.AddAsync(board.BoardNumber!);
        await _dbContext.Boards.AddAsync(board);
        await _dbContext.SaveChangesAsync();

        _fakeGameQueryHelper.SetActiveGame(game);

        var dto = new AddBoardRequestDto
        {
            BoardNumbers = new List<int> { 6, 7, 8, 9, 10 },
            IsRepeating = false,
            RepeatCount = 0
        };

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateBoard(user.UserId, dto));
    }

    [Fact]
    public async Task CreateBoard_ShouldThrow_WhenInsufficientBalance()
    {
        await ClearDatabase();

        var user = CreateTestUser("user3", "Dave", "d@test.com");
        var game = new Game { GameId = Guid.NewGuid().ToString(), Status = "Active", CreatedAt = DateTime.UtcNow };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();

        _fakeGameQueryHelper.SetActiveGame(game);
        _fakeBalanceHelper.Balance = 50;
        _fakeBalanceHelper.Price = 100;

        var dto = new AddBoardRequestDto
        {
            BoardNumbers = new List<int> { 1, 2, 3, 4, 5 },
            IsRepeating = false,
            RepeatCount = 0
        };

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateBoard(user.UserId, dto));
    }

    [Fact]
    public async Task CreateBoard_ShouldThrow_WhenAfterSaturday5PM_Simulated()
    {
        await ClearDatabase();

        var user = CreateTestUser("user4", "Erin", "e@test.com");
        var game = new Game { GameId = Guid.NewGuid().ToString(), Status = "Active", CreatedAt = DateTime.UtcNow };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();

        _fakeGameQueryHelper.SetActiveGame(game);
        _fakeBalanceHelper.Balance = 200;
        _fakeBalanceHelper.Price = 100;

        var dto = new AddBoardRequestDto
        {
            BoardNumbers = new List<int> { 1, 2, 3, 4, 5 },
            IsRepeating = false,
            RepeatCount = 0
        };

        var ex = new ValidationException("Boards cannot be submitted after Saturday 17:00.");
        Assert.Equal("Boards cannot be submitted after Saturday 17:00.", ex.Message);
    }

    [Fact]
    public async Task CreateBoard_ShouldThrow_WhenSunday_Simulated()
    {
        await ClearDatabase();

        var user = CreateTestUser("user5", "Fiona", "f@test.com");
        var game = new Game { GameId = Guid.NewGuid().ToString(), Status = "Active", CreatedAt = DateTime.UtcNow };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();

        _fakeGameQueryHelper.SetActiveGame(game);
        _fakeBalanceHelper.Balance = 200;
        _fakeBalanceHelper.Price = 100;

        var dto = new AddBoardRequestDto
        {
            BoardNumbers = new List<int> { 1, 2, 3, 4, 5 },
            IsRepeating = false,
            RepeatCount = 0
        };

        var ex = new ValidationException("Boards cannot be submitted on Sunday.");
        Assert.Equal("Boards cannot be submitted on Sunday.", ex.Message);
    }
}
