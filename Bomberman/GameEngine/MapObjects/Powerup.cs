namespace Bomberman.GameEngine.MapObjects
{
  internal class Powerup : MapObject
  {
    internal Powerup(int x, int y, string variant) : base(x, y, $"powerup_{variant}", null, null, 2) { }
  }
}
