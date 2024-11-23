using BoardGame;

namespace Genetic;

/// <summary>
/// Each Gene represents a set of element's configuration: dimensions (x, y, z), position (x, y, z), and layer index 
/// </summary>
public class Gene
{
    private readonly int _maxNumberOfElements;
    public List<IElement> Data { get; }
    public bool CanAddMoreElements => Data.Count < _maxNumberOfElements;
    
    public Gene(int maxNumberOfElements = 1)
    {
        _maxNumberOfElements = maxNumberOfElements;
        Data = [];
    }

    public override string ToString()
    {
        return string.Join(", ", Data);
    }
}