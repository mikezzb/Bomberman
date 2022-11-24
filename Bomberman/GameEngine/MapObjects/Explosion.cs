using System.Collections.Generic;

namespace Bomberman.GameEngine.MapObjects
{
  internal class Explosion : MapObject
  {
    public static Dictionary<string, int?> GetVariants(string variant) => new()
    {
      {
        GetVariantName(variant),
        2
      }
    };
    public static string GetVariantName(string variant) => $"explosion_{variant}";
    internal Explosion(int x, int y, string variant) : base(x, y, GetVariantName(variant), GetVariants(variant), null, 1, true) { }
  }
}
