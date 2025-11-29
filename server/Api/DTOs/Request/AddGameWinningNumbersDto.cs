using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request;

public class AddGameWinningNumbersDto
{
    [MinLength(3)] [Required]
    public string GameId { get; set; } = null!;

    [MinLength(3)] [MaxLength(3)] [Required]
    public List<int> GameWinningNumbers { get; set; } = null!;
}