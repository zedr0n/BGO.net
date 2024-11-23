namespace BoardGame;

/// <summary>
/// Board game item
/// </summary>
public record Item(int Length, int Width, int Height, ItemOrientation Orientation = ItemOrientation.VERTICAL, Plane Plane = Plane.XZ)
{
    /// <summary>
    /// Gets the bounding box for the item
    /// </summary>
    public (int x, int y, int z) BoundingBox => ApplyOrientation(Width, Length, Height);

    private (int x, int y, int z) ApplyOrientation(int x, int y, int z)
    {
        if ((Orientation & ItemOrientation.DOWN) != 0)
            z *= -1;
        if ((Orientation & ItemOrientation.HORIZONTAL) != 0)
            (x, y) = (y, x);

        switch (Plane)
        {
            case Plane.XZ:
                (x, y, z) = (x, z, y);
                break;
            case Plane.YZ:
                (x, y, z) = (z, x, y);
                break;
            default:
                break; 
        }
        return (x, y, z);
    }
}