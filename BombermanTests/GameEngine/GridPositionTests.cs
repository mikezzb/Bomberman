using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bomberman.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      gp.SetX(13);
      gp.SetY(1);
      Assert.AreEqual(520, gp.CanvasPosition.X);
      Assert.AreEqual(40, gp.CanvasPosition.Y);
    }
    /// <summary>
    /// Black box test
    /// </summary>
    [TestMethod()]
    public void GridPositionFloatTest()
    {
      GridPosition gp = new(7, 3);
      gp.ShiftX(0.1);
      gp.ShiftY(1.5);
      Assert.AreEqual(284, gp.CanvasPosition.X);
      Assert.AreEqual(180, gp.CanvasPosition.Y);
    }
  }
}
