using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request;

public class AddBoardRequestDto
{
    [Required]
    public bool IsRepeating { get; set; }

    public int RepeatCount { get; set; }
    
    [Required]
    [MinLength(5)]
    [MaxLength(8)]
    public List<int> BoardNumbers { get; set; } = null!;
}