using Bomberman.GameEngine.Graphics;
using System.Collections.Generic;

namespace Bomberman.GameEngine.MapObjects
{
  /// <summary>
  /// Abstract class of map object in game, including everything visible on the map
  /// <list type="bullet">
  /// <item>Position</item>
  /// <item>Display (Image)</item>
  /// </list>
  /// </summary>
  public abstract class AnimatedMapObject : MapObject, IUpdatable
  {
    public int FrameNum { get; protected set; }
    protected AnimatedSprite animatedSprite;
    protected AnimatedMapObject(int x, int y, string name, Dictionary<string, int?>? variant = null, string? defaultVariant = null, int zIndex = 1) : base(x, y, name, variant, defaultVariant, zIndex)
    {
      animatedSprite = (AnimatedSprite)sprite;
    }
    protected override Sprite CreateSprite(string name, Dictionary<string, int?>? variant = null, string? defaultVariant = null, int zIndex = 1)
    {
      return new AnimatedSprite(name, variant, defaultVariant, Position, zIndex);
    }
    /// <summary>
    /// Update the animated sprite
    /// </summary>
    public virtual void Update()
    {
      animatedSprite.Update();
    }
  }
}
