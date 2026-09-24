namespace PlayGames.Models;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public decimal DiscountPrice { get; set; }
    public string ImageUrl { get; set; } = "";
    public string Genre { get; set; } = "";
    public bool IsActive { get; set; } = true;
}
