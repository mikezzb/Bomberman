using System;
using System.Collections.Generic;

namespace Bomberman.GameEngine
{
  public static class Config
  {
    public static readonly Random Rnd = new Random();
    public static readonly int GameDuration = 300;
    public static readonly int ItemSize = 40;
    public static readonly int Height = 13;
    public static readonly int Width = 31;
    public static readonly string ImageExt = ".png";
    public static readonly string SoundExt = ".mp3";
    public static readonly string Map = Resources.map;
    public static readonly int FramesPerCycle = 10;
    public static readonly int FrameDuration = 25;
    // set to 246 to test random generation
    public static readonly int NumStraightMob = 2;
    public static readonly int NumRandomMob = 2;
    public static readonly int NumMobs = NumStraightMob + NumRandomMob;
    public static readonly int NumBricks = 50;
    public static readonly int NumBombNumPU = 2;
    public static readonly int NumBombRangePU = 2;
    public static readonly int NumPowerups = NumBombNumPU + NumBombRangePU;
    public static readonly int ImageUpdateFrameNum = 6;
    public static readonly int RemoveDeadObjectAfterFrameNum = Utilities.Duration2FrameNum(1000);
    public static readonly int NumFramesTillExplode = Utilities.Duration2FrameNum(2000);
    public static readonly int NumFramesTillExplosionFinish = Utilities.Duration2FrameNum(3000);
  };
  namespace Enums
  {
    public enum GameObject
    {
      Wall = '#',
      Floor = ' ',
      StraightMob = 's',
      RandomMob = 'r',
      Brick = '*',
      Player = 'p',
      BombNumPowerup = 'N',
      BombRangePowerup = 'R',
      Key = 'K',
      Door = 'D'
    }
    public enum GameSound
    {
      Title,
      StageStart,
      StageClear,
      Bgm,
      GetPowerup,
      GetKey,
      GameOver
    }
    public enum GameState
    {
      Started,
      Paused,
      Ended,
    }
    public enum Direction
    {
      Up,
      Down,
      Left,
      Right,
    }
    public enum MobType
    {
      StraightWalk,
      RandomWalk
    }
    public enum PowerupType
    {
      Speed,
      BombNum,
      BombRange
    }
    public enum GameEndType
    {
      Cleared,
      Dead,
      Timeout,
    }
  }
}
