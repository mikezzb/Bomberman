using Bomberman.GameEngine.Enums;
using System;
using System.Windows.Input;

namespace Bomberman.GameEngine
{
  public class IntPoint
  {
    public int X { get; private set; }
    public int Y { get; private set; }
    public IntPoint(int x, int y)
    {
      X = x;
      Y = y;
    }
  }
  public class BeforeNextMoveEventArgs : EventArgs
  {
    public IntPoint From { get; private set; }
    public IntPoint To { get; private set; }
    public Boolean Cancel { get; set; }
    public BeforeNextMoveEventArgs(IntPoint from, IntPoint to)
    {
      From = from;
      To = to;
    }
  }
}
