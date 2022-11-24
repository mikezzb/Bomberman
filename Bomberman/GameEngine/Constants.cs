using Bomberman.GameEngine.Enums;
using System.Collections.Generic;

namespace Bomberman.GameEngine
{
  public static class Constants
  {
    public static readonly Dictionary<Direction, string> DirectionName = new()
    {
      { Direction.Up, "up" },
      { Direction.Down, "down" },
      { Direction.Left, "left" },
      { Direction.Right, "right" },
    };
    public static readonly List<Direction> Directions = new()
    {
      Direction.Up,
      Direction.Down,
      Direction.Left,
      Direction.Right,
    };
    public static readonly Dictionary<Direction, IntPoint> DirectionTranslation = new()
    {
      { Direction.Up, new IntPoint(0, -1) },
      { Direction.Down, new IntPoint(0, 1) },
      { Direction.Left, new IntPoint(-1, 0) },
      { Direction.Right, new IntPoint(1, 0) },
    };
    public static readonly HashSet<Direction> DirectionsSet = new(Directions);
    public static readonly Dictionary<PowerupType, string> PowerupTypeName = new()
    {
      { PowerupType.BombNum, "bombs" },
      { PowerupType.BombRange, "flames" }
    };
  };
}
