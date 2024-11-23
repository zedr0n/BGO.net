namespace BoardGame;

public class Game(int length, int width, int height)
{
    private readonly Dictionary<Item, int> _items = new();

    public (int x, int y, int z) LayerSize => new(width, height, length);
    
    public (int x, int y, int z) GetLayerSize(int numberOfLayers = 1) => new (width, length, height / numberOfLayers); 
    
    public List<int> TotalSpace(int layers = 1)
    {
        var volume = length * width * height;
        var totalSpace = new List<int>();
        for (var i = 0; i < layers; i++)
            totalSpace.Add(volume / layers);
        return totalSpace;
    }
    
    public void AddItems(Item item, int count)
    {
        if(!_items.TryAdd(item, count))
            _items[item] += count;
    }

    public List<ItemClass> GetItemClasses()
    {
        var itemClasses = new List<ItemClass>();
        foreach (var (item, count) in _items)
        {
            var itemClass = new ItemClass(item, count);
            var boundingBox = itemClass.BoundingBox;
            itemClasses.Add(itemClass);
        }

        return itemClasses;
    }
}