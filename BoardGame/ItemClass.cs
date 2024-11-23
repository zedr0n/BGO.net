namespace BoardGame;

public class ItemClass(Item item, int count, StackingDirection stackingDirection = StackingDirection.MINIMAL)
{
    private (int x, int y, int z) _boundingBox = (0,0,0);
    
    public (int x, int y, int z) BoundingBox
    {
        get
        {
            if (_boundingBox.x != 0 || _boundingBox.y != 0 || _boundingBox.z != 0) 
                return _boundingBox;
            
            var boundingBox = item.BoundingBox;
            switch (stackingDirection)
            {
                case StackingDirection.X:
                    _boundingBox = (boundingBox.x * count, boundingBox.y, boundingBox.z);
                    break;
                case StackingDirection.Y:
                    _boundingBox = (boundingBox.x, boundingBox.y * count, boundingBox.z);
                    break;
                case StackingDirection.Z:
                    _boundingBox = (boundingBox.x, boundingBox.y, boundingBox.z * count);
                    break;
                case StackingDirection.MINIMAL:
                    var min = Math.Min(boundingBox.x, Math.Min(boundingBox.y, boundingBox.z));
                    if ( boundingBox.x == min )
                        _boundingBox = (boundingBox.x * count, boundingBox.y, boundingBox.z);
                    if ( boundingBox.y == min )
                        _boundingBox = (boundingBox.x, boundingBox.y * count, boundingBox.z);
                    if ( boundingBox.z == min )
                        _boundingBox = (boundingBox.x, boundingBox.y, boundingBox.z * count);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return _boundingBox;
        }
    }
}