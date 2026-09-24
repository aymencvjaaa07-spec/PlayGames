namespace PlayGames.Models;

public class GameKey
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public string Key { get; set; } = "";
    public bool IsSold { get; set; }
}
