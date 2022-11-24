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
  public abstract class MapObject
  {
    public GridPosition Position { get; protected set; }
    protected Sprite sprite;
    /// <summary>
    /// Do nothing constructor for inherited class to define their own
    /// </summary>
    protected MapObject() { }
    protected MapObject(int x, int y)
    {
      Position = new GridPosition(x, y);
    }
    protected MapObject(int x, int y, string name, Dictionary<string, int?>? variant = null, string? defaultVariant = null, int zIndex = 1) : this(x, y)
    {
      sprite = new Sprite(name, variant, defaultVariant, Position, zIndex);
    }
    public virtual void Remove()
    {
      sprite?.Dispose();
    }
  }
}
