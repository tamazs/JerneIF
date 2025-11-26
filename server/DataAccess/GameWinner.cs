namespace DataAccess;

public partial class GameWinner
{
    public string WinnerId { get; set; } = null!;
    public string GameId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string BoardId { get; set; } = null!;
    public DateTime WonAt { get; set; }
    public List<int> MatchedNumbers { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual Board Board { get; set; } = null!;
}
