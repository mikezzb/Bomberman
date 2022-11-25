using Bomberman.GameEngine.Enums;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Bomberman.GameEngine
{
  /// <summary>
  /// Utilities for bomberman game
  /// </summary>
  public static class Utilities
  {
    public static HashSet<int> GetRandomNumbers(int count, int min, int max)
    {
      HashSet<int> numbers = new();
      Random r = Config.Rnd;
      for (int i = 0; i < count; i++)
      {
        int num;
        do
        {
          num = r.Next(min, max);
        } while (numbers.Contains(num));
        numbers.Add(num);
      }
      return numbers;
    }
    public static string ToMMSS(int seconds)
    {
      TimeSpan time = TimeSpan.FromSeconds(seconds);
      return time.ToString(@"mm\:ss");
    }
    public static Direction? Key2Direction(Key key)
    {
      return key switch
      {
        Key.Up => Direction.Up,
        Key.Down => Direction.Down,
        Key.Left => Direction.Left,
        Key.Right => Direction.Right,
        _ => null,
      };
    }
    public static Direction GetOppositeDirection(Direction dir)
    {
      return dir switch
      {
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        Direction.Left => Direction.Right,
        Direction.Right => Direction.Left,
        _ => throw new Exception("Invalid direction"),
      };
    }
    public static HashSet<Direction> DirectionsExclude(HashSet<Direction> dirSet)
    {
      HashSet<Direction> result = new(Constants.DirectionsSet);
      result.ExceptWith(dirSet);
      return result;
    }
    /// <summary>
    /// Get true / false, each 50% chance
    /// </summary>
    /// <returns></returns>
    public static bool GetRandomBool()
    {
      Random r = Config.Rnd;
      return r.Next(0, 2) == 1;
    }
    public static Direction GetRandomDirection()
    {
      Random r = Config.Rnd;
      Direction dir = (Direction)r.Next(0, 4);
      return dir;
    }
    public static T GetRandomListElement<T>(List<T> li)
    {
      Random r = Config.Rnd;
      return li[r.Next(li.Count)];
    }

    public static int Duration2FrameNum(int durationInMs)
    {
      return durationInMs / Config.FrameDuration;
    }
  }
}
