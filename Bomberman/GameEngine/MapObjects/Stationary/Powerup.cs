using Bomberman.GameEngine.Enums;

namespace Bomberman.GameEngine.MapObjects
{
  public class Powerup : MapObject
  {
    public PowerupType Type { get; private set; }
    public Powerup(int x, int y, PowerupType type) : base(x, y, $"powerup_{Constants.PowerupTypeName[type]}", null, null, 2)
    {
      Type = type;
    }
  }
}
