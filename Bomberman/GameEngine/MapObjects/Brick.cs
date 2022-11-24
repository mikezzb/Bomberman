namespace Bomberman.GameEngine.MapObjects
{
  public class Brick : MapObject
  {
    // z index is 3 to place on top of powerups
    public Brick(int x, int y) : base(x, y, "brick", null, null, 3) { }
  }
}
