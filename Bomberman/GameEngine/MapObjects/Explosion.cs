using System.Collections.Generic;

namespace Bomberman.GameEngine.MapObjects
{
  public class Explosion : AnimatedMapObject
  {
    public static Dictionary<string, int?> GetVariants(string variant) => new()
    {
      {
        variant,
        2
      }
    };
    public Explosion(int x, int y, string variant) : base(x, y, "explosion", GetVariants(variant), variant, 1)
    {
      animatedSprite.StartAnimation();
    }
  }
}
