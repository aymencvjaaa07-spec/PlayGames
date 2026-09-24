namespace PlayGames.Models;

public class LibraryItem
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";

    public int GameId { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
