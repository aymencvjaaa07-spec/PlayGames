namespace PlayGames.Models;

public class GameStoreViewModel
{
    public List<Game> Games { get; set; } = new();

    public HashSet<int> OwnedGameIds { get; set; } = new();

    public int? AutoInstallId { get; set; }
}
