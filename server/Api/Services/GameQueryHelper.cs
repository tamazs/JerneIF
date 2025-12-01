using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class GameQueryHelper(JerneDbContext dbContext)
{
    public async Task<Game?> GetActiveGame()
    {
        return await dbContext.Games
            .Where(g => g.Status == GameStatus.Active.ToString())
            .OrderByDescending(g => g.CreatedAt)
            .FirstOrDefaultAsync();
    }
}