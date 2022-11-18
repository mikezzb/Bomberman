using System;
using System.Windows;

namespace Bomberman.GameEngine
{
  public class GridPosition
  {
    public int X { get; private set; }
    public int Y { get; private set; }
    public double OffsetX { get; private set; }
    public double OffsetY { get; private set; }
    public Point CanvasPosition
    {
      get => new Point(CanvasX, CanvasY);
    }
    public double PreciseX
    {
      get => (X + OffsetX);
    }
    public double PreciseY
    {
      get => (Y + OffsetY);
    }
    public double CanvasX
    {
      get => PreciseX * Config.ItemSize;
    }
    public double CanvasY
    {
      get => PreciseY * Config.ItemSize;
    }

    public GridPosition(int x, int y)
    {
      X = x;
      Y = y;
    }
    public void SetX(int x)
    {
      X = x;
    }
    public void SetY(int y)
    {
      Y = y;
    }
    public void ShiftX(double dx)
    {
      SetX(PreciseX + dx);
    }
    public void ShiftY(double dy)
    {
      SetY(PreciseY + dy);
    }
    public void SetX(double x)
    {
      X = (int)Math.Truncate(x);
      OffsetX = x - X;
    }
    public void SetY(double y)
    {
      Y = (int)Math.Truncate(y);
      OffsetY = y - Y;
    }
  }
}
