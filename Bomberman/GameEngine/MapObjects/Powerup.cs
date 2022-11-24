using Bomberman.GameEngine.Enums;

namespace Bomberman.GameEngine.MapObjects
{
  internal class Powerup : MapObject
  {
    public PowerupType Type { get; private set; }
    internal Powerup(int x, int y, PowerupType type) : base(x, y, $"powerup_{Constants.PowerupTypeName[type]}", null, null, 2)
    {
      Type = type;
    }
  }
}
