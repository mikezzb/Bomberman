using System;
using System.Windows;
using Bomberman.GameEngine.Enums;

namespace Bomberman.GameEngine
{
  public class MovableGridPosition : GridPosition
  {
    public MovableGridPosition(int x, int y) : base(x, y) { }
    public double OffsetX { get; private set; }
    public double OffsetY { get; private set; }
    public double PreciseX
    {
      get => X + OffsetX;
    }
    public double PreciseY
    {
      get => Y + OffsetY;
    }
    public override double CanvasX
    {
      get => PreciseX * Config.ItemSize;
    }
    public override double CanvasY
    {
      get => PreciseY * Config.ItemSize;
    }
    public void ShiftX(double dx)
    {
      OffsetX += dx;
    }
    public void ShiftY(double dy)
    {
      OffsetY += dy;
    }
    public void Move(Direction? dir, double delta)
    {
      if (dir == null) return;
      switch (dir)
      {
        case Direction.Left:
          ShiftX(-delta);
          break;
        case Direction.Right:
          ShiftX(delta);
          break;
        case Direction.Up:
          ShiftY(-delta);
          break;
        case Direction.Down:
          ShiftY(delta);
          break;
      }
    }
    /// <summary>
    /// Called after move (i.e. when grid pos is int)
    /// </summary>
    public void ResetOffset()
    {
      X += (int)Math.Round(OffsetX);
      Y += (int)Math.Round(OffsetY);
      OffsetX = 0;
      OffsetY = 0;
    }
  }
}
