using BoardGame;
using Xunit;

namespace Tests;

public class BoardGameTests
{
    [Fact]
    public void CanApplyOrientation()
    {
        var card = new Card(100, 50, 2, ItemOrientation.VERTICAL, Plane.XY);
        Assert.Equal((50, 100, 2), card.BoundingBox);
        var card2 = new Card(100, 50, 2, ItemOrientation.HORIZONTAL, Plane.XY);
        Assert.Equal((100, 50, 2), card2.BoundingBox);
        var card4 = new Card(100, 50, 2, ItemOrientation.DOWN, Plane.XY);
        Assert.Equal((50, 100, -2), card4.BoundingBox);
        
        var card6 = new Card(100, 50, 2, ItemOrientation.HORIZONTAL | ItemOrientation.DOWN, Plane.XY);
        Assert.Equal((100, 50, -2), card6.BoundingBox);

        /*var card3 = new Card(100, 50, 2, ItemOrientation.LEFT);
        Assert.Equal((-50, 100, 2), card3.BoundingBox);
        var card7 = new Card(100, 50, 2, ItemOrientation.HORIZONTAL | ItemOrientation.LEFT);
        Assert.Equal((-100, 50, 2), card7.BoundingBox);
        var card5 = new Card(100, 50, 2, ItemOrientation.LEFT | ItemOrientation.DOWN);
        Assert.Equal((-50, 100, -2), card5.BoundingBox);
        var card8 = new Card(100, 50, 2, ItemOrientation.HORIZONTAL | ItemOrientation.DOWN | ItemOrientation.LEFT);
        Assert.Equal((-100, 50, -2), card8.BoundingBox);*/
    }

    [Fact]
    public void CanGetItemClassBoundingBox()
    {
        var card = new Card(100, 50, 2, ItemOrientation.VERTICAL, Plane.XY);
        var cards = new ItemClass(card, 10);
        Assert.Equal((50, 100, 20), cards.BoundingBox);
        
        var horizontalCard = new Card(100, 50, 2, ItemOrientation.HORIZONTAL, Plane.XY);
        var horizontalCards = new ItemClass(horizontalCard, 10);
        Assert.Equal((100, 50, 20), horizontalCards.BoundingBox);
        
        var horizontalCards2 = new ItemClass(horizontalCard, 10, StackingDirection.X);
        Assert.Equal((1000, 50, 2), horizontalCards2.BoundingBox);
    }
    
    [Fact]
    public void CanCreateGame()
    {
        var card = new Card(88, 63, 1);
        var tile = new Tile(100, 100, 1);
        var game = new Game(88, 300, 120);
        game.AddItems(card, 100);
        game.AddItems(tile, 100);
        
        var itemClasses = game.GetItemClasses();
        Assert.Equal(2, itemClasses.Count);
    }
}