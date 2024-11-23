namespace BoardGame;

/// <summary>
/// Board game card item
/// </summary>
/// <param name="Length">Length of the card</param>
/// <param name="Width">Width of the card</param>
/// <param name="Height">Thickness of the card</param>
/// <param name="Orientation">Upwards or downwards orientation</param>
public record Card(int Length, int Width, int Height, ItemOrientation Orientation = ItemOrientation.VERTICAL, Plane Plane = Plane.XZ) 
    : Item(Length, Width, Height, Orientation, Plane)
{
}