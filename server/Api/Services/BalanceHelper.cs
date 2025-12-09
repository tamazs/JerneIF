using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class BalanceHelper(JerneDbContext dbContext)
{
    public virtual async Task<decimal> GetBalance(string userId)
    {
        var totalDeposits = await dbContext.Transactions
            .Where(t => t.UserId == userId && t.Status == TransactionStatus.Approved.ToString())
            .SumAsync(t => (decimal?)t.Amount) ?? 0;
        
        var totalSpent = await dbContext.Boards
            .Where(b => b.UserId == userId)
            .SumAsync(b => (decimal?)b.Price) ?? 0;
        
        return totalDeposits - totalSpent;
    }
    
    public virtual int CalculatePrice(int count)
    {
        return count switch
        {
            5 => 20,
            6 => 40,
            7 => 80,
            8 => 160,
            _ => throw new ArgumentOutOfRangeException(nameof(count), "Board must contain 5–8 numbers.")
        };
    }
}