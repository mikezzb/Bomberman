using Bomberman.GameEngine.Enums;
using Bomberman.GameEngine.MapObjects;
using System;

namespace Bomberman.GameEngine
{
  /// <summary>
  /// More map definitions
  /// </summary>
  public class StageGame : Game
  {
    private readonly int stageNum;
    public StageGame(int stageNum, Action<GameEndType> onGameEnded) : base(onGameEnded)
    {
      this.stageNum = stageNum;
    }
    protected override void CreateMapEntries()
    {
      CreateMap(Constants.StagesMap[stageNum - 1]);
    }
    protected override void CreateMapEntry(char c, int x, int y)
    {
      bool addFloor = true;
      bool addBrick = false;
      IntPoint pos = new(x, y);
      switch (c)
      {
        case (char)GameObject.Wall:
          Wall wall = new(x, y);
          AddToMap(wall, null, true, false);
          addFloor = false;
          break;
        case (char)GameObject.StraightMob:
          CreateMob(pos, MobType.StraightWalk);
          break;
        case (char)GameObject.RandomMob:
          CreateMob(pos, MobType.RandomWalk);
          break;
        case (char)GameObject.Player:
          CreatePlayer(x, y);
          break;
        case (char)GameObject.Brick:
          addBrick = true;
          break;
        case (char)GameObject.BombNumPowerup:
          CreatePowerup(pos, PowerupType.BombNum);
          addBrick = true;
          break;
        case (char)GameObject.BombRangePowerup:
          CreatePowerup(pos, PowerupType.BombRange);
          addBrick = true;
          break;
        case (char)GameObject.Key:
          CreateKey(pos);
          addBrick = true;
          break;
        case (char)GameObject.Door:
          CreateDoor(pos);
          addBrick = true;
          break;
      }
      if (addFloor)
      {
        Floor floor = new(x, y);
        backgrounds.Add(floor);
      }
      if (addBrick)
      {
        CreateBrick(pos);
      }
    }
  }
}
