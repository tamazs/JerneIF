using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request;

public class AddBoardRequestDto
{
    [MinLength(3)] [Required]
    public string GameId { get; set; } = null!;
    [MinLength(3)] [Required]
    public string UserId { get; set; } = null!;
    [Required]
    public bool IsRepeating { get; set; }

    public DateTime? RepeatingUntil { get; set; }
    
    [Required]
    [MinLength(5)]
    [MaxLength(8)]
    public List<int> BoardNumbers { get; set; } = null!;
}