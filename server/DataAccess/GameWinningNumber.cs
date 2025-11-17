using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class GameWinningNumber
{
    public string GameWinningNumbersId { get; set; } = null!;

    public string GameId { get; set; } = null!;

    public List<int> GameWinningNumbers { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;
}
