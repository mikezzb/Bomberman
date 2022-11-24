using System.Collections.Generic;
using Bomberman.GameEngine.Graphics;

namespace Bomberman.GameEngine.MapObjects
{
  internal class Explosion : AnimatedMapObject
  {
    public static Dictionary<string, int?> GetVariants(string variant) => new()
    {
      {
        variant,
        2
      }
    };
    internal Explosion(int x, int y, string variant) : base(x, y, "explosion", GetVariants(variant), variant, 1) {
      animatedSprite.StartAnimation();
    }
  }
}
