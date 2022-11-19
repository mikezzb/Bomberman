using System;

namespace Bomberman
{
  public static class Utilities
  {
    public static string ToMMSS(int seconds)
    {
      TimeSpan time = TimeSpan.FromSeconds(seconds);
      return time.ToString(@"mm\:ss");
    }
  }
}
