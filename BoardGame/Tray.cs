using System.Drawing;

namespace BoardGame;

public class Tray : IElement
{
    private Random _random = new();
    private readonly int _parameterCount = 7; 
    
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    
    public Item Item { get; set; }
    public int Layer { get; set; }
    
    public (int x, int y, int z) BoundingBox => Item.BoundingBox;

    public (int x, int y, int z) GetOverlap(IElement element)
    {
        if (element is not Tray other)
            return (0,0,0);
        // calculate overlap in each dimension
        var overlapX = Get1DOverlap(X, BoundingBox.x, other.X, other.BoundingBox.x);
        var overlapY = Get1DOverlap(Y, BoundingBox.y, other.Y, other.BoundingBox.y);
        var overlapZ = Get1DOverlap(Z, BoundingBox.z, other.Z, other.BoundingBox.z);
        return (overlapX,overlapY,overlapZ); 
    }

    public void Mutate()
    {
        var random = new Random();
        var choice = random.Next(_parameterCount);
        var d = random.Next(-15, 15);
        switch (choice)
        {
            case 0:
                X = Math.Max(0, X + d);
                break;
            case 1:
                Y = Math.Max(0, Y + d);
                break;
            case 2:
                Z = Math.Max(0, Z + d);
                break;
            case 3:
                Item = new Item(Math.Max(0, Item.Length + d), Item.Width, Item.Height);
                break;
            case 4:
                Item = new Item(Item.Length, Math.Max(0, Item.Width + d), Item.Height);
                break;
            case 5:
                Item = new Item(Item.Length, Item.Width, Math.Max(0, Item.Height + d));
                break;
        }
    }

    public override string ToString()
    {
        return $"({X},{BoundingBox.x})|({Y},{BoundingBox.y})|({Z},{BoundingBox.z})|{Layer}";
    }

    public static Tray FromGame(Game game)
    {
        var layerSize = game.GetLayerSize();
        var minSize = Math.Min(layerSize.x, Math.Min(layerSize.y, layerSize.z));
        var random = new Random();
        var tray = new Tray
        {
            X = random.Next(0, layerSize.x),
            Y = random.Next(0, layerSize.y),
            Z = random.Next(0, layerSize.z),
            Item = new Item(random.Next(1, minSize), random.Next(1, minSize), random.Next(1, minSize)),
            Layer = 0
        };
        return tray;
    }

    private int Get1DOverlap(int x, int dx, int y, int dy)
    {
        var start1 = Math.Min(x, x + dx);
        var end1 = Math.Max(x, x + dx);
        var start2 = Math.Min(y, y + dy);
        var end2 = Math.Max(y, y + dy);

        var overlapStart = Math.Max(start1, start2);
        var overlapEnd = Math.Min(end1, end2);
        return Math.Max(0, overlapEnd - overlapStart);
    }
}