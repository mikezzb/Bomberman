using System.Windows;

namespace Bomberman.GameEngine
{
  public class GridPosition
  {
    public int X { get; protected set; }
    public int Y { get; protected set; }
    public Point CanvasPosition
    {
      get => new Point(CanvasX, CanvasY);
    }
    public virtual double CanvasX
    {
      get => X * Config.ItemSize;
    }
    public virtual double CanvasY
    {
      get => Y * Config.ItemSize;
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
  }
}
