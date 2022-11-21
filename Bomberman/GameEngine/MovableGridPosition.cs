﻿using Bomberman.GameEngine.Enums;
using System;
using System.Windows;

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
    public IntPoint GridPosition {  get => new IntPoint(GridX, GridY); }
    public int GridX { get => (int)Math.Ceiling(PreciseX);  }
    public int GridY { get => (int)Math.Ceiling(PreciseY); }
    public int NextX { get; private set; }
    public int NextY { get; private set; }
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
      Point translation = SolveMoveTranslation((Direction)dir, delta);
      Move(translation);
    }
    private void Move(Point translation)
    {
      if (translation.X != 0) ShiftX(translation.X);
      if (translation.Y != 0) ShiftY(translation.Y);
    }
    public IntPoint PostMovePosition(Direction dir) {

      Point translation = SolveMoveTranslation(dir, 1);
      // assume current walk is completed, so round up
      translation.X += X;
      translation.Y += Y;
      return new IntPoint((int)translation.X, (int)translation.Y);
    }
    public IntPoint PostMovePosition(Direction dir, IntPoint from)
    {

      Point translation = SolveMoveTranslation(dir, 1);
      // assume current walk is completed, so round up
      translation.X += from.X;
      translation.Y += from.Y;
      return new IntPoint((int)translation.X, (int)translation.Y);
    }
    private static Point SolveMoveTranslation(Direction dir, double delta = 1)
    {
      double dx = 0, dy = 0;
      switch (dir)
      {
        case Direction.Left:
          dx = -delta;
          break;
        case Direction.Right:
          dx = delta;
          break;
        case Direction.Up:
          dy = -delta;
          break;
        case Direction.Down:
          dy = delta;
          break;
      }
      return new Point(dx, dy);
    }
    /// <summary>
    /// Called after move (i.e. when grid pos is int)
    /// </summary>
    public void FinishMove()
    {
      X += (int)Math.Round(OffsetX);
      Y += (int)Math.Round(OffsetY);
      OffsetX = 0;
      OffsetY = 0;
    }
  }
}
