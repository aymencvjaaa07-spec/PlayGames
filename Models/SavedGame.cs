using System.ComponentModel.DataAnnotations;

namespace PlayGames.Models;

public class SavedGame
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = "";

    public int GameId { get; set; }

    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}
