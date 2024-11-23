namespace Genetic;

/// <summary>
/// An individual is composed of multiple genes, each representing a set of elements for each item class
/// </summary>
public class Individual
{
    private readonly int _geneCount;
    private readonly Random _random = new();

    public double Fitness { get; set; } = double.NaN;
    public List<Gene> Genes { get; } = [];
    public int MaxNumberOfElements { get; }

    public Individual(int geneCount, int maxNumberOfElements)
    {
        _geneCount = geneCount;
        MaxNumberOfElements = maxNumberOfElements;

        for (var i = 0; i < _geneCount; i++)
            Genes.Add(new Gene(maxNumberOfElements));
    }

    public void Mutate(double mutationRate)
    {
        var gene = _random.GetItems(Genes.ToArray(), 1)[0];
        var rand = _random.NextDouble();
        if (rand < mutationRate)
        {
            var elementToMutate = _random.GetItems(gene.Data.ToArray(), 1)[0];
            elementToMutate.Mutate();
        }
    }

    public override string ToString()
    {
        return string.Join("\n", Genes.Select(gene => gene.ToString()));
    }
}