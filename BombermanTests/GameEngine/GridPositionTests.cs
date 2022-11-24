using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bomberman.GameEngine.Tests
{
  [TestClass()]
  public class GridPositionTests
  {
    /// <summary>
    /// Black box test
    /// </summary>
    [TestMethod()]
    public void GridPositionIntTest()
    {
      GridPosition gp = new(7, 3);
      gp.Set(13, 1);
      Assert.AreEqual(520, gp.CanvasPosition.X);
      Assert.AreEqual(40, gp.CanvasPosition.Y);
    }
    /// <summary>
    /// Black box test
    /// </summary>
    [TestMethod()]
    public void GridPositionFloatTest()
    {
      MovableGridPosition gp = new(7, 3);
      gp.ShiftX(0.1);
      gp.ShiftY(1.5);
      Assert.AreEqual(284, gp.CanvasPosition.X);
      Assert.AreEqual(180, gp.CanvasPosition.Y);
    }

    [TestMethod()]
    public void IntersectsWithTest()
    {
      MovableGridPosition p1 = new(7, 3);
      p1.ShiftX(-0.3);
      p1.ShiftY(-0.1);
      MovableGridPosition p2 = new(6, 2);
      Assert.IsTrue(p1.IntersectsWith(p2));
      p1.ShiftX(0.29999);
      Assert.IsTrue(p1.IntersectsWith(p2));
      p2.ShiftX(-0.01);
      Assert.IsFalse(p1.IntersectsWith(p2));
      p1.ShiftX(-0.01);
      p1.ShiftY(0.2);
      Assert.IsFalse(p1.IntersectsWith(p2));
      // diagonal
      MovableGridPosition p3 = new(1, 1);
      MovableGridPosition p4 = new(2, 2);
      Assert.IsFalse(p3.IntersectsWith(p4));
    }
  }
}
