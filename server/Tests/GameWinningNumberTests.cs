using System.ComponentModel.DataAnnotations;
using Api.DTOs;
using Api.DTOs.Request;
using Api.Services;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests;

[Collection("IntegrationTests")]
public class GameWinningNumberServiceTests
{
    private readonly JerneDbContext _dbContext;
    private readonly FakeGameService _fakeGameService;
    private readonly FakeGameQueryHelper _fakeGameQueryHelper;
    private readonly GameWinningNumberService _service;

    public GameWinningNumberServiceTests(JerneDbContext dbContext)
    {
        _dbContext = dbContext;
        _fakeGameService = new FakeGameService();
        _fakeGameQueryHelper = new FakeGameQueryHelper();
        _service = new GameWinningNumberService(_dbContext, _fakeGameService, _fakeGameQueryHelper);
    }

    private async Task ClearDatabase()
    {
        _dbContext.ChangeTracker.Clear();
        await _dbContext.Database.ExecuteSqlRawAsync(@"
            TRUNCATE TABLE ""Games"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""Boards"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""BoardNumbers"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""GameWinningNumbers"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""GameWinners"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""Users"" RESTART IDENTITY CASCADE;
        ");
    }
    
    private class FakeGameService : IGameService
    {
        public bool WasCalled { get; private set; }

        public Task<GameDto> CreateGame()
        {
            WasCalled = true;
            var dto = new GameDto(new Game
            {
                GameId = Guid.NewGuid().ToString(),
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            });
            return Task.FromResult(dto);
        }

        public Task<List<GameDto>> GetAllGames(Sieve.Models.SieveModel sieveModel) => Task.FromResult(new List<GameDto>());
    }

    private class FakeGameQueryHelper : GameQueryHelper
    {
        private Game? _activeGame;

        public FakeGameQueryHelper() : base(null!) { }

        public void SetActiveGame(Game game)
        {
            _activeGame = game;
        }

        public override Task<Game> GetActiveGame()
        {
            if (_activeGame == null)
                throw new InvalidOperationException("Active game not set in fake helper.");
            return Task.FromResult(_activeGame);
        }
    }

    [Fact]
    public async Task GetGameWinningNumbersForGame_ShouldReturnRecord()
    {
        await ClearDatabase();

        var game = new Game
        {
            GameId = Guid.NewGuid().ToString(),
            Status = "Finished",
            CreatedAt = DateTime.UtcNow
        };
        
        var gwn = new GameWinningNumber
        {
            GameWinningNumbersId = Guid.NewGuid().ToString(),
            GameId = game.GameId,
            GameWinningNumbers = new List<int> { 1, 2, 3 }
        };
        
        await _dbContext.Games.AddAsync(game);
        await _dbContext.GameWinningNumbers.AddAsync(gwn);
        await _dbContext.SaveChangesAsync();

        var result = await _service.GetGameWinningNumbersForGame(game.GameId);

        Assert.NotNull(result);
        Assert.Equal(game.GameId, result.GameId);
        Assert.Equal(3, result.GameWinningNumbers.Count);
    }

    [Fact]
    public async Task GetGameWinningNumbersForGame_ShouldThrow_WhenNotFound()
    {
        await ClearDatabase();
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetGameWinningNumbersForGame("nonexistent"));
    }

    [Fact]
    public async Task AddGameWinningNumbers_ShouldCreateRecord_AndCloseGame()
    {
        await ClearDatabase();

        var game = new Game
        {
            GameId = Guid.NewGuid().ToString(),
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };
        
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
        
        var boardNumbers = new BoardNumber
        {
            BoardNumbersId = Guid.NewGuid().ToString(),
            BoardNumbers = new List<int> { 1, 2, 3, 4, 5 }
        };
        
        var board = new Board
        {
            BoardId = Guid.NewGuid().ToString(),
            GameId = game.GameId,
            UserId = user.UserId,
            BoardNumber = boardNumbers,
            NumberCount = 5
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.Games.AddAsync(game);
        await _dbContext.BoardNumbers.AddAsync(boardNumbers);
        await _dbContext.Boards.AddAsync(board);
        await _dbContext.SaveChangesAsync();

        // Prepare fake active game
        _fakeGameQueryHelper.SetActiveGame(game);

        var dto = new AddGameWinningNumbersDto
        {
            GameWinningNumbers = new List<int> { 1, 2, 3 }
        };

        var result = await _service.AddGameWinningNumbers(user.UserId, dto);

        Assert.NotNull(result);
        Assert.Equal(game.GameId, result.GameId);
        Assert.Equal(3, result.GameWinningNumbers.Count);

        // Check game closed
        var updatedGame = await _dbContext.Games.FindAsync(game.GameId);
        Assert.Equal(GameStatus.Finished.ToString(), updatedGame.Status);

        // Winner should exist
        var winners = await _dbContext.GameWinners.Where(gw => gw.GameId == game.GameId).ToListAsync();
        Assert.Single(winners);
        Assert.Equal(user.UserId, winners.First().UserId);

        // Next game should be created (verified by flag)
        Assert.True(_fakeGameService.WasCalled);
    }

    [Fact]
    public async Task AddGameWinningNumbers_ShouldThrow_WhenAlreadyExists()
    {
        await ClearDatabase();

        var game = new Game { GameId = Guid.NewGuid().ToString(), Status = "Active", CreatedAt = DateTime.UtcNow };
        
        var existing = new GameWinningNumber
        {
            GameWinningNumbersId = Guid.NewGuid().ToString(),
            GameId = game.GameId,
            GameWinningNumbers = new List<int> { 1, 2, 3 }
        };
        
        await _dbContext.Games.AddAsync(game);
        await _dbContext.GameWinningNumbers.AddAsync(existing);
        await _dbContext.SaveChangesAsync();

        _fakeGameQueryHelper.SetActiveGame(game);

        var dto = new AddGameWinningNumbersDto { GameWinningNumbers = new List<int> { 4, 5, 6 } };

        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.AddGameWinningNumbers("user", dto));
    }

    [Theory]
    [InlineData(2)] // less than 3 numbers
    [InlineData(4)] // more than 3 numbers
    public async Task AddGameWinningNumbers_ShouldThrow_WhenInvalidCount(int count)
    {
        await ClearDatabase();

        var game = new Game { GameId = Guid.NewGuid().ToString(), Status = "Active", CreatedAt = DateTime.UtcNow };
        
        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();

        _fakeGameQueryHelper.SetActiveGame(game);

        var dto = new AddGameWinningNumbersDto { GameWinningNumbers = Enumerable.Range(1, count).ToList() };

        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.AddGameWinningNumbers("user", dto));
    }

    [Fact]
    public async Task AddGameWinningNumbers_ShouldThrow_WhenOutOfRange()
    {
        await ClearDatabase();

        var game = new Game { GameId = Guid.NewGuid().ToString(), Status = "Active", CreatedAt = DateTime.UtcNow };
        
        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();

        _fakeGameQueryHelper.SetActiveGame(game);

        var dto = new AddGameWinningNumbersDto { GameWinningNumbers = new List<int> { 0, 17, 18 } };

        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.AddGameWinningNumbers("user", dto));
    }
}
