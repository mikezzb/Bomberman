using Bomberman.GameEngine.Enums;
using System;

namespace Bomberman.GameEngine
{
  public class IntPoint
  {
    public int X { get; protected set; }
    public int Y { get; protected set; }
    public IntPoint(int x, int y)
    {
      X = x;
      Y = y;
    }
    public override string ToString()
    {
      return $"({X},{Y})";
    }
    public int Index
    {
      get => X * Config.Width + Y;
    }
  }
  public class GameEngineRect
  {
    public double X { get; protected set; }
    public double Y { get; protected set; }
    public double Height { get; protected set; }
    public double Width { get; protected set; }
    public double Right { get; protected set; }
    public double Bot { get; protected set; }
    public double Left => X;
    public double Top => Y;
    public GameEngineRect(double x, double y, double width, double height)
    {
      X = x;
      Y = y;
      Width = width;
      Right = x + width;
      Height = height;
      Bot = y + height;
    }
    public override string ToString()
    {
      return $"({X},{Y},{Width},{Height})";
    }
    /// <summary>
    /// Strict intersect (i.e. Must overlap)
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public bool OverlapsWith(GameEngineRect r)
    {
      bool xOverlap = r.Right > Left && r.Left < Right;
      bool yOverlap = r.Bot > Top && r.Top < Bot;
      return xOverlap && yOverlap;
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
