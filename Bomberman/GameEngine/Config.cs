using System.Collections.Generic;

namespace Bomberman.GameEngine
{
  public static class Config
  {
    public static readonly int ItemSize = 40;
    public static readonly int Height = 13;
    public static readonly int Width = 31;
    public static readonly string ImageExt = ".png";
    public static readonly string Map = Resources.map;
    public static readonly int WalkFrames = 10;
    public static readonly int WalkFrameDuration = 20;
    // set to 246 to test random generation
    public static readonly int NumStraightMob = 2;
    public static readonly int NumRandomMob = 2;
    public static readonly int NumMobs = NumStraightMob + NumRandomMob;
    public static readonly int NumBricks = 50;
    public static readonly int NumBombNumPU = 23;
    public static readonly int NumBombRangePU = 25;
    public static readonly int NumPowerups = NumBombNumPU + NumBombRangePU;
    public static readonly int NumFramesTillExplode = Utilities.Duration2FrameNum(2000);
    public static readonly int NumFramesTillExplosionFinish = Utilities.Duration2FrameNum(3000);
    /// <summary>
    /// Time to walk 1 unit in ms
    /// </summary>
    public static readonly int WalkDuration = WalkFrameDuration * WalkFrames;
    public static readonly Dictionary<Enums.Direction, string> DirectionName = new()
    {
      { Enums.Direction.Up, "up" },
      { Enums.Direction.Down, "down" },
      { Enums.Direction.Left, "left" },
      { Enums.Direction.Right, "right" },
    };
    public static readonly List<Enums.Direction> Directions = new()
    {
      Enums.Direction.Up,
      Enums.Direction.Down,
      Enums.Direction.Left,
      Enums.Direction.Right,
    };
    public static readonly HashSet<Enums.Direction> DirectionsSet = new(Directions);
  };
  namespace Enums
  {
    public enum GameObject
    {
      Wall = '#',
      Floor = ' ',
    }
    public enum GameState
    {
      Started,
      Paused,
      Ended,
    }
    public enum Direction
    {
      Up,
      Down,
      Left,
      Right,
    }
    public enum MobType
    {
      StraightWalk,
      RandomWalk
    }
    public enum PowerupType
    {
      Speed,
      BombNum,
      BombRange
    }
  }
}
