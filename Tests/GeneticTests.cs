using BoardGame;
using Genetic;
using Xunit;
using Xunit.Abstractions;

namespace Tests;

public class GeneticTests
{
    private readonly ITestOutputHelper output;

    public GeneticTests(ITestOutputHelper output)
    {
        this.output = output;
    }    
    
    [Fact]
    public void CanGeneratePopulation()
    {
        var card = new Card(88, 63, 1);
        var tile = new Tile(100, 100, 1);
        var game = new Game(300, 300, 120);
        game.AddItems(card, 100);
        game.AddItems(tile, 100);
        
        var genetic = new GeneticSolver(game, 5);
        genetic.GeneratePopulation();
        
        Assert.Equal(5, genetic.Population.Count);
    }

    [Fact]
    public void CanSolve()
    {
        var card = new Card(88, 63, 1);
        var tile = new Tile(100, 100, 1);
        var game = new Game(300, 300, 120);
        game.AddItems(card, 100);
        game.AddItems(tile, 100);
        
        var genetic = new GeneticSolver(game, 200);
        genetic.GeneratePopulation();
        
        var winner = genetic.Solve(2000);
        var fitness = genetic.ComputeFitness(winner);
        output.WriteLine($"Winner fitness: {fitness}");
        output.WriteLine($"{winner}");
        Assert.Equal(0, fitness);
        
    }
}