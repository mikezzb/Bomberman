using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bomberman.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman.GameEngine.Enums;

namespace Bomberman.GameEngine.Tests
{
  [TestClass()]
  public class GameSoundPlayerTests
  {
    [TestMethod()]
    public void GameSoundPlayerTest()
    {
      GameSoundPlayer soundPlayer = GameSoundPlayer.Instance;
      // play
      soundPlayer.PlaySound(GameSound.StageStart);
      Assert.AreEqual(GameSound.StageStart, soundPlayer.CurrSound);
      // switch
      Task.Delay(1000).Wait(); // listen to the sound to see if switched lol
      soundPlayer.PlaySound(GameSound.GetPowerup);
      Task.Delay(1000).Wait(); // listen to the sound to see if switched lol
      Assert.AreEqual(GameSound.GetPowerup, soundPlayer.CurrSound);
    }
  }
}