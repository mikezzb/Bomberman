namespace Bomberman.GameEngine.MapObjects
{
  internal class Brick : MapObject
  {
    // z index is 3 to place on top of powerups
    internal Brick(int x, int y) : base(x, y, "brick", null, null, 3) { }
  }
}
