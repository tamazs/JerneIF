using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request;

public class AddBoardRequestDto
{
    [MinLength(3)] [Required]
    public string GameId { get; set; } = null!;
    
    [Required]
    public bool IsRepeating { get; set; }

    public int RepeatCount { get; set; }
    
    [Required]
    [MinLength(5)]
    [MaxLength(8)]
    public List<int> BoardNumbers { get; set; } = null!;
}