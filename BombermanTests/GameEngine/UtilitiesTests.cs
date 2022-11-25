using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bomberman.GameEngine.Tests
{
  [TestClass()]
  public class UtilitiesTests
  {
    [TestMethod()]
    public void Duration2FrameNumTest()
    {
      // normal case
      Assert.AreEqual(Utilities.Duration2FrameNum(2000), 80);
      // rouding
      Assert.AreEqual(Utilities.Duration2FrameNum(2010), 80);
      // rouding
      Assert.AreEqual(Utilities.Duration2FrameNum(3000), 120);
    }
  }
}