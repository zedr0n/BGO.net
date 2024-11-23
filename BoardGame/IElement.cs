namespace BoardGame;

public interface IElement
{
    int X { get; set; }
    int Y { get; set; }
    int Z { get; set; }
    Item Item { get; set; }
    int Layer { get; set; }
    
    public (int x, int y, int z) GetOverlap(IElement element);
    public void Mutate();
}