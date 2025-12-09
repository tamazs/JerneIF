using Api.Services;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Xunit;

namespace Tests;

[Collection("IntegrationTests")]
public class GameServiceTests
{
    private readonly JerneDbContext _dbContext;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly GameService _gameService;

    // Fake BalanceHelper to simulate user balances in tests
    public class FakeBalanceHelper : BalanceHelper
    {
        private readonly decimal _balance;
        private readonly int _price;

        public FakeBalanceHelper(decimal balance, int price) : base(null!)
        {
            _balance = balance;
            _price = price;
        }

        public override Task<decimal> GetBalance(string userId)
            => Task.FromResult(_balance);

        public override int CalculatePrice(int numberCount)
            => _price;
    }

    public GameServiceTests(JerneDbContext dbContext, ISieveProcessor sieveProcessor)
    {
        _dbContext = dbContext;
        _sieveProcessor = sieveProcessor;

        // Default balance helper (sufficient balance)
        var fakeHelper = new FakeBalanceHelper(1000, 100);
        _gameService = new GameService(_dbContext, _sieveProcessor, fakeHelper);
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

    // Helper to create a dummy game for FK relations
    private async Task<Game> CreateDummyGame()
    {
        var game = new Game
        {
            GameId = Guid.NewGuid().ToString(),
            Status = "Finished",
            StartDate = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();
        return game;
    }

    [Fact]
    public async Task GetAllGames_ShouldReturnGames()
    {
        await ClearDatabase();

        var game1 = new Game { GameId = Guid.NewGuid().ToString(), Status = "Active", StartDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow };
        var game2 = new Game { GameId = Guid.NewGuid().ToString(), Status = "Finished", StartDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow };

        await _dbContext.Games.AddRangeAsync(game1, game2);
        await _dbContext.SaveChangesAsync();

        var sieve = new SieveModel();
        var result = await _gameService.GetAllGames(sieve);

        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllGames_ShouldReturnEmpty_WhenNoGamesExist()
    {
        await ClearDatabase();

        var sieve = new SieveModel();
        var result = await _gameService.GetAllGames(sieve);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateGame_ShouldCreateActiveGame()
    {
        await ClearDatabase();

        var result = await _gameService.CreateGame();

        Assert.NotNull(result);
        Assert.Equal("Active", result.Status);
        Assert.NotEqual(default, result.GameId);

        var saved = await _dbContext.Games.FirstOrDefaultAsync(g => g.GameId == result.GameId);
        Assert.NotNull(saved);
        Assert.Equal("Active", saved.Status);
    }

    [Fact]
    public async Task CreateGame_ShouldReuseRepeatingBoards_WhenUserHasEnoughBalance()
    {
        await ClearDatabase();
        
        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Bob",
            Email = "bob@test.com",
            PhoneNumber = "12345678",
            Role = "Player",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!"),
            IsActive = true
        };
        await _dbContext.Users.AddAsync(user);

        // Create dummy game for FK
        var oldGame = await CreateDummyGame();

        // Arrange old repeating board
        var boardNumbers = new BoardNumber
        {
            BoardNumbersId = Guid.NewGuid().ToString(),
            BoardNumbers = new List<int> { 1, 2, 3, 4, 5 }
        };

        var board = new Board
        {
            BoardId = Guid.NewGuid().ToString(),
            GameId = oldGame.GameId,
            UserId = user.UserId,
            IsRepeating = true,
            RepeatCount = 2,
            PurchasedAt = DateTime.UtcNow.AddDays(-1),
            BoardNumber = boardNumbers,
            NumberCount = 5,
            Price = 100
        };

        await _dbContext.BoardNumbers.AddAsync(boardNumbers);
        await _dbContext.Boards.AddAsync(board);
        await _dbContext.SaveChangesAsync();

        // Balance helper with enough funds
        var richHelper = new FakeBalanceHelper(500, 100);
        var gameService = new GameService(_dbContext, _sieveProcessor, richHelper);
        
        var result = await gameService.CreateGame();
        
        Assert.NotNull(result);
        Assert.Equal("Active", result.Status);

        var boards = await _dbContext.Boards.Where(b => b.GameId == result.GameId).ToListAsync();
        Assert.Single(boards);
        Assert.Equal(user.UserId, boards.First().UserId);
        Assert.Equal(1, boards.First().RepeatCount);
    }

    [Fact]
    public async Task CreateGame_ShouldNotRepeat_WhenInsufficientBalance()
    {
        await ClearDatabase();
        
        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Alice",
            Email = "alice@test.com",
            PhoneNumber = "11111111",
            Role = "Player",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!"),
            IsActive = true
        };
        await _dbContext.Users.AddAsync(user);
        
        var oldGame = await CreateDummyGame();
        
        var boardNumbers = new BoardNumber
        {
            BoardNumbersId = Guid.NewGuid().ToString(),
            BoardNumbers = new List<int> { 6, 7, 8, 9, 10 }
        };

        var board = new Board
        {
            BoardId = Guid.NewGuid().ToString(),
            GameId = oldGame.GameId,
            UserId = user.UserId,
            IsRepeating = true,
            RepeatCount = 2,
            PurchasedAt = DateTime.UtcNow.AddDays(-1),
            BoardNumber = boardNumbers,
            NumberCount = 5,
            Price = 100
        };

        await _dbContext.BoardNumbers.AddAsync(boardNumbers);
        await _dbContext.Boards.AddAsync(board);
        await _dbContext.SaveChangesAsync();

        // Balance helper with too little balance
        var poorHelper = new FakeBalanceHelper(50, 100);
        var gameService = new GameService(_dbContext, _sieveProcessor, poorHelper);
        
        var result = await gameService.CreateGame();
        
        Assert.NotNull(result);
        Assert.Equal("Active", result.Status);

        var boards = await _dbContext.Boards.Where(b => b.GameId == result.GameId).ToListAsync();
        Assert.Empty(boards);

        var oldBoard = await _dbContext.Boards.Include(b => b.BoardNumber).FirstAsync(b => b.UserId == user.UserId);
        Assert.False(oldBoard.IsRepeating);
        Assert.Equal(0, oldBoard.RepeatCount);
    }
}
