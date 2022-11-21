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
    public static readonly int WalkFrames = 8;
    public static readonly int WalkFrameDuration = 30;
    // set to 246 to test random generation
    public static readonly int NumBricks = 50;
    public static readonly int NumExplosionPU = 5;
    public static readonly int NumSpeedPU = 5;
    public static readonly int NumPowerups = NumExplosionPU + NumSpeedPU;
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
  }
}
