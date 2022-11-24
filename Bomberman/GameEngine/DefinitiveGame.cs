using Bomberman.GameEngine.Enums;
using Bomberman.GameEngine.MapObjects;
using System;

namespace Bomberman.GameEngine
{
  /// <summary>
  /// More map definitions
  /// </summary>
  public class DefinitiveGame : Game
  {
    public DefinitiveGame(Action<GameEndType> onGameEnded) : base(onGameEnded) { }
    protected override void InitMapEntries()
    {
      InitMap();
    }
    protected override void InitMapEntry(char c, int x, int y)
    {
      base.InitMapEntry(c, x, y);
      // Add more init
      bool addFloor = false;
      switch (c)
      {
        case (char)GameObject.StraightMob:
          Wall wall = new(x, y);
          AddToMap(wall, null, true, false);
          break;
        case (char)GameObject.Floor:
          Floor floor = new(x, y);
          break;
      }
      if (addFloor)
      {
        Floor floor = new(x, y);
      }
    }
  }
}
