using System.Collections.Concurrent;
using BoardGame;

namespace Genetic;

public class GeneticSolver
{
    private readonly Game _game;
    private readonly int _populationSize;
    private readonly int _tournamentSize;
    private readonly int[] _populationChoice;
    private List<ItemClass> _itemClasses;
    private readonly int _geneCount;
    private readonly Random _random;

    public GeneticSolver(Game game, int populationSize)
    {
        _game = game;
        _populationSize = populationSize;
        _populationChoice = new int[_populationSize];
        for(var i = 0; i < _populationSize; i++)
            _populationChoice[i] = i;
        
        _tournamentSize = (int)Max(4, _populationSize / 100);
        _itemClasses = _game.GetItemClasses();
        
        _geneCount = _itemClasses.Count;
        _random = new Random();
    }
    
    public double MutationRate { get; init; } = 0.25;
    public double MutationRateNumberOfElementsUp { get; init; } = 0.1;
    public double MutationRateNumberOfElementsDown { get; init; } = 0.1;

    public double NumberOfElementsFactor { get; init; } = 0.0;
    public double UnusedSpaceFactor { get; init; } = 1.0;
    public double UnfitFactor { get; init; } = 10.0;
    public double OverlapFactor { get; init; } = 10.0;
    public double OverfitFactor { get; init; } = 10.0;
    public int MaxNumberOfElements { get; init; } = 1;
    public (double alpha_min, double alpha_max) AlphaRange { get; init; } = (-0.05, 1.05);

    public List<Individual> Population { get; private set; } = [];

    public Individual Solve(int numberOfGenerations, double tolerance = 0.001)
    {
        Individual? winner = null;
        for (var i = 0; i < numberOfGenerations; i++)
        {
            RunGeneration();
            Parallel.ForEach(Population, ind =>
            {
                ComputeFitness(ind);
            });
            winner = Population.MaxBy(ComputeFitness);
            if(winner != null && Math.Abs(winner.Fitness) <= tolerance)
                break;
            var fitnesses = Population.Select(x => x.Fitness).ToList();
            var mean = fitnesses.Average();
            var variance = fitnesses.Select(fit => (mean - fit)*(mean -fit)).Average();
            Console.WriteLine($"Generation {i} fitness: {winner.Fitness}");
            Console.WriteLine($"Fitness Distribution: ({mean},{Math.Sqrt(variance)})");
        }
        return winner;
    }

    public List<Individual> RunGeneration()
    {
        var numberOfElites = (int)Max(1, _populationSize * 6 / 100 );
        var sortedPopulation = Population.OrderByDescending(ComputeFitness).ToList();
        
        var elites = sortedPopulation.Take(numberOfElites).ToList();
        var newPopulation = new ConcurrentBag<Individual>(elites); //new List<Individual>(elites);

        Parallel.For(0, (_populationSize - numberOfElites) / 2, i =>
        {
            var parent1 = Select();
            var parent2 = Select();
            var child1 = Crossover(parent1, parent2);
            var child2 = Crossover(parent1, parent2);
            child1.Mutate(MutationRate);
            child2.Mutate(MutationRate);
            newPopulation.Add(child1);
            newPopulation.Add(child2);
        });
        
        Population = newPopulation.Take(_populationSize).ToList();
        return Population;
    }

    private void Mutate(Individual individual)
    {
        var gene = _random.GetItems(individual.Genes.ToArray(), 1)[0];
        var rand = _random.NextDouble();
        if (rand < MutationRate)
        {
            var elementToMutate = _random.GetItems(gene.Data.ToArray(), 1)[0];
            elementToMutate.Mutate();
        }
    }
    
    private Individual Crossover(Individual parent1, Individual parent2)
    {
        var child = new Individual(parent1.Genes.Count, parent1.MaxNumberOfElements);
        for (var i = 0; i < parent1.Genes.Count; i++)
        {
            var gene = child.Genes[i];
            gene.Data.Clear();
            var parent1Trays = parent1.Genes[i].Data;
            var parent2Trays = parent2.Genes[i].Data;
            var maxNumberOfElements = Max(parent1Trays.Count, parent2Trays.Count);
            for (var j = 0; j < maxNumberOfElements; j++)
            {
                var alpha = AlphaRange.alpha_min + _random.NextDouble()*(AlphaRange.alpha_max - AlphaRange.alpha_min);
                IElement element = null;
                if (j < parent1Trays.Count && j < parent2Trays.Count)
                {
                    var tray1 = parent1Trays[j];
                    var tray2 = parent2Trays[j];
                    element = new Tray
                    {
                        Layer = tray1.Layer,
                        X = (int)Max(0, WeightedAverage(tray1.X, tray2.X, alpha)),
                        Y = (int)Max(0, WeightedAverage(tray1.Y, tray2.Y, alpha)),
                        Z = (int)Max(0, WeightedAverage(tray1.Z, tray2.Z, alpha)),
                        Item = new Item(
                            WeightedAverage(tray1.Item.Length, tray2.Item.Length, alpha),
                            WeightedAverage(tray1.Item.Width, tray2.Item.Width, alpha),
                            WeightedAverage(tray1.Item.Height, tray2.Item.Height, alpha)
                        )
                    };
                }
                else if (j < parent1Trays.Count)
                    element = parent1Trays[j];
                else if (j < parent2Trays.Count)
                    element = parent2Trays[j];
                if (element != null)
                    gene.Data.Add(element);
            }
        }
        return child;
    }

    private int WeightedAverage(int x, int y, double alpha)
    {
        return Convert.ToInt32((alpha*x + (1-alpha)*y));
    }
    
    private Individual Select()
    {
        var tournament = _random.GetItems(_populationChoice, _tournamentSize).Select(i => Population[i]);
        var winner = tournament.OrderByDescending(individual => individual.Fitness).First();
        return winner;
    }
    
    public double ComputeFitness(Individual individual)
    {
        if(!double.IsNaN(individual.Fitness))
            return individual.Fitness;
        
        var numberOfElements = individual.Genes.Sum(x => x.Data.Count);
        var minNumberOfElements = individual.Genes.Sum(x => 1);

        var unusedSpace = _game.TotalSpace();
        var unfitPenalty = 0.0;
        var overfitPenalty = 0.0;
        var overlapPenalty = 0.0;
        var gapPenalty = 0.0;
        
        var fitness = 0.0;
        var allElements = individual.Genes.SelectMany(x => x.Data).ToList();
        var ( maxX, maxY, maxZ ) = _game.GetLayerSize();
        for (var i = 0; i < individual.Genes.Count; i++)
        {
            var gene = individual.Genes[i];
            var itemClass = _itemClasses[i];
            var (requiredX, requiredY, requiredZ) = (itemClass.BoundingBox.x, itemClass.BoundingBox.y, itemClass.BoundingBox.z);
            var (unfitX, unfitY, unfitZ) = (0.0, 0.0, 0.0);
            var (overfitX, overfitY, overfitZ) = (0.0, 0.0, 0.0);
            foreach (var element in gene.Data)
            {
                // check unused space
                var (dx, dy, dz) = element.Item.BoundingBox;
                unusedSpace[element.Layer] -= dx*dy*dz;
                
                // check if element dimensions meet or exceed item class requirements
                unfitX += Max(0, requiredX - dx)*maxY*maxZ;
                unfitY += maxX*Max(0, requiredY - dy)*maxZ;
                unfitZ += maxX*maxY*Max(0, requiredZ - dz);
                
                // check if element is outside the box
                overfitX += Max(0, element.X + dx - maxX) * maxY * maxZ ;
                overfitY += maxX * Max(0, element.Y + dy - maxY) * maxZ;
                overfitZ += maxX * maxY * Max(0, element.Z + dz - maxZ);
                
                foreach (var otherElement in allElements
                             .Where(otherElement => otherElement != element && otherElement.Layer == element.Layer))
                {
                    var (overlapX, overlapY, overlapZ) = element.GetOverlap(otherElement);
                    unusedSpace[element.Layer] += overlapX*overlapY*overlapZ;
                    overlapPenalty += overlapX*overlapY*overlapZ;
                }
            }

            unfitPenalty += unfitX + unfitY + unfitZ;
            overfitPenalty += overfitX + overfitY + overfitZ;
        }
        fitness = UnusedSpaceFactor * unusedSpace.Sum()
                  + UnfitFactor * unfitPenalty
                  + OverlapFactor * overlapPenalty
                  + OverfitFactor * overfitPenalty;
            
        fitness /= (maxX*maxY*maxZ);
        fitness *= 100;
        fitness += NumberOfElementsFactor * (numberOfElements - minNumberOfElements) / individual.Genes.Count / 100;
        fitness = -fitness;
        
        individual.Fitness = fitness;
        return fitness;
    }
    
    public void GeneratePopulation()
    {
        for (var i = 0; i < _populationSize; i++)
        {
            var individual = new Individual(_geneCount, MaxNumberOfElements);
            foreach (var gene in individual.Genes)
                gene.Data.Add(Tray.FromGame(_game));
            
            Population.Add(individual);
        }
    }
    
    private static double Max(int x, int y)
    {
        return Convert.ToDouble(Math.Max(x,y));
    }
    
    private static double Max(double x, double y) => Math.Max(x, y);
}