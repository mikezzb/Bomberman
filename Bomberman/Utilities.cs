using Bomberman.GameEngine.Enums;
using System;
using System.Windows.Input;

namespace Bomberman
{
  public static class Utilities
  {
    public static string ToMMSS(int seconds)
    {
      TimeSpan time = TimeSpan.FromSeconds(seconds);
      return time.ToString(@"mm\:ss");
    }
    public static Direction? Key2Direction(Key key)
    {
      switch (key)
      {
        case Key.Up:
          return Direction.Up;
        case Key.Down:
          return Direction.Down;
        case Key.Left:
          return Direction.Left;
        case Key.Right:
          return Direction.Right;
        default:
          return null;
      }
    }
  }
}
