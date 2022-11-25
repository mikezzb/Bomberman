using Bomberman.GameEngine.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bomberman.GameEngine.MapObjects
{
  /// <summary>
  /// Control the actions of a player (e.g. movement + drop bomb)
  /// - Not singleton for future multi-player
  /// </summary>
  public class Mob : MovableMapObject
  {
    private static readonly int StuckRetryInterval = Utilities.Duration2FrameNum(1000);
    private bool stuck = false;
    private Direction initDir;
    public MobType Type { get; private set; }
    private static readonly Dictionary<string, int?> variant = new()
    {
      { "dead", null },
      { "left", 2 },
      { "right", 2 },
    };
    public Mob(int x, int y, MobType type, EventHandler<BeforeNextMoveEventArgs> onBeforeNextMove) : base(x, y, onBeforeNextMove, type == MobType.RandomWalk ? "oneal" : "minvo", variant, "left")
    {
      Type = type;
    }
    /// <summary>
    /// Pick initial direction
    /// </summary>
    public virtual void InitMovement(List<Direction> movableDirs)
    {
      Debug.WriteLine($"Movable to {movableDirs.Count} dirs");
      if (movableDirs.Count == 0)
      {
        stuck = true;
        initDir = Utilities.GetRandomDirection();
        return;
      };
      initDir = Utilities.GetRandomListElement(movableDirs);
      Move(initDir);
    }
    protected override BeforeNextMoveEventArgs CanMove(Direction dir)
    {
      BeforeNextMoveEventArgs e = base.CanMove(dir);
      if (stuck && !e.Cancel)
      {
        Debug.WriteLine($"Unstuck at {dir}");
        stuck = false;
      }
      return e;
    }
    public override void Update()
    {
      // if stuck, choose random dir to try again
      if (stuck && FrameNum % StuckRetryInterval == 0)
      {
        initDir = Utilities.GetRandomDirection();
        // Debug.WriteLine($"[MOB_{Type}]: stuck retry | {CurrDir} | {initDir}");
        Move(initDir);
      }
      base.Update();
    }
    /// <summary>
    /// Callback after intersection reached
    /// </summary>
    public virtual void OnIntersectionReached(HashSet<Direction> movableDirs, BeforeNextMoveEventArgs e)
    {
      Direction currDir = CurrDir ?? initDir;
      // Debug.WriteLine($"On intersection: {currDir}");
      Direction oppositeDirection = Utilities.GetOppositeDirection(currDir);
      Direction? nextDir = null;
      // to opposite direction
      if (Type == MobType.StraightWalk)
      {
        // Debug.WriteLine($"[MOB]: to opposite {oppositeDirection}");
        nextDir = oppositeDirection;
      }
      else
      {
        // random choose a direction or opposite dir
        Direction randomDirection = Utilities.GetRandomDirection();
        // if the random dir is valid, then change dir, the dir can be either opposite or other valid dir
        if (randomDirection != currDir && movableDirs.Contains(randomDirection))
        {
          // Debug.WriteLine($"[MOB_{Type}]: to random {randomDirection}");
          nextDir = randomDirection;
        }
        else
        {
          // Debug.WriteLine($"[MOB_{Type}]: to oppo {oppositeDirection}");
          if (movableDirs.Contains(oppositeDirection))
          {
            nextDir = oppositeDirection;
          }
        }
      }
      if (nextDir != null && movableDirs.Contains((Direction)nextDir))
      {
        // Debug.WriteLine($"[MOB_{Type}]: valid move to {nextDir}");
        ResetMove((Direction)nextDir);
        e.TurnDirection = nextDir;
      }
      else
      {
        stuck = true;
        Debug.WriteLine($"[MOB_{Type}]: Invalid move");
      }
    }
  }
}
