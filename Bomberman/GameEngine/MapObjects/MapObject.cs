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
  public abstract class MapObject : IRemovable
  {
    public GridPosition Position { get; protected set; }
    protected Sprite sprite;
    protected MapObject(int x, int y, string name, Dictionary<string, int?>? variant = null, string? defaultVariant = null, int zIndex = 1)
    {
      Position = CreatePosition(x, y);
      sprite = CreateSprite(name, variant, defaultVariant, zIndex);
    }
    protected virtual GridPosition CreatePosition(int x, int y)
    {
      return new GridPosition(x, y);
    }
    protected virtual Sprite CreateSprite(string name, Dictionary<string, int?>? variant = null, string? defaultVariant = null, int zIndex = 1)
    {
      return new Sprite(name, variant, defaultVariant, Position, zIndex);
    }
    public virtual void Remove()
    {
      sprite.Remove();
    }
  }
}
