using System;
using Bomberman.GameEngine.Enums;

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
    public override string ToString()
    {
      return $"({X},{Y})";
    }
  }
  public class BeforeNextMoveEventArgs : EventArgs
  {
    public IntPoint From { get; private set; }
    public IntPoint To { get; private set; }
    public Direction? TurnDirection { get; set; }
    public bool Cancel { get; set; }
    public BeforeNextMoveEventArgs(IntPoint from, IntPoint to)
    {
      From = from;
      To = to;
    }
  }
}
