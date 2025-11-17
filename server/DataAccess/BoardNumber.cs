using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class BoardNumber
{
    public string BoardNumbersId { get; set; } = null!;

    public string BoardId { get; set; } = null!;

    public List<int> BoardNumbers { get; set; } = null!;

    public virtual Board Board { get; set; } = null!;
}
