using Bomberman.GameEngine.Enums;
using System.Diagnostics;
using System.Windows;
namespace Bomberman.GameEngine
{
  public class GridPosition : IntPoint
  {
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
    public GridPosition(int x, int y) : base(x, y) { }
    public void Set(int x, int y)
    {
      Debug.WriteLine("test");
      X = x;
      Y = y;
    }
    public bool IntersectsWith(GridPosition p)
    {
      bool xIntersect = p.CanvasX2 > CanvasX && p.CanvasX < CanvasX2;
      bool yIntersect = p.CanvasY2 > CanvasY && p.CanvasY < CanvasY2;
      return xIntersect && yIntersect;
    }
    public IntPoint PostTranslatePosition(Direction dir)
    {
      IntPoint translation = SolveDirectionTranslation(dir);
      return AfterTranslation(translation);
    }
    public IntPoint AfterTranslation(IntPoint translation)
    {
      return new IntPoint(translation.X + X, translation.Y + Y);
    }
    public static IntPoint SolveDirectionTranslation(Direction dir)
    {
      return Constants.DirectionTranslation[dir];
    }
  }
}
