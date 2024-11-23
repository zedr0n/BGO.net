namespace BoardGame;

/// <summary>
/// Board game card item
/// </summary>
/// <param name="Length">Length of the tile</param>
/// <param name="Width">Width of the tile</param>
/// <param name="Height">Thickness of the tile</param>
/// <param name="Orientation">Upwards or downwards orientation</param>
public record Tile(int Length, int Width, int Height, ItemOrientation Orientation = ItemOrientation.VERTICAL) 
    : Item(Length, Width, Height, Orientation)
{
}