using System.Windows;
using System.Diagnostics;
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
    public virtual IntPoint Position { get => new IntPoint(X, Y); }
    public virtual double CanvasX
    {
      get => X * Config.ItemSize;
    }
    public virtual double CanvasY
    {
      get => Y * Config.ItemSize;
    }
    public double CanvasX2
    {
      get => CanvasX + Config.ItemSize;
    }
    public double CanvasY2
    {
      get => CanvasY + Config.ItemSize;
    }
    public string Key
    {
      get => $"{X}_{Y}";
    }
    public int Index
    {
      get => X * Config.Width + Y;
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
    public bool IntersectsWith(GridPosition p)
    {
      bool xIntersect = p.CanvasX2 >= CanvasX && p.CanvasX <= CanvasX2;
      bool yIntersect = p.CanvasY2 >= CanvasY && p.CanvasY <= CanvasY2;
      // Debug.WriteLine($"[Intersection]: {p.CanvasX},{p.CanvasY}");
      // Debug.WriteLine($"{CanvasX},{CanvasY}");
      // Debug.WriteLine($"x: {xIntersect} y: {yIntersect}");
      return xIntersect && yIntersect;
    }
  }
}
