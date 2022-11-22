using System.Collections.Generic;
using Bomberman.GameEngine.Enums;
using System.Diagnostics;

namespace Bomberman.GameEngine.MapObjects
{
  /// <summary>
  /// Control the actions of a player (e.g. movement + drop bomb)
  /// - Not singleton for future multi-player
  /// </summary>
  internal class Mob : MovableMapObject
  {
    public MobType Type { get; private set; }
    public Direction CurrDir { get => (Direction)animatedSprite.CurrDir; }
    public Direction OppositeDir { get => Utilities.GetOppositeDirection(CurrDir); }
    private static readonly Dictionary<string, int?> variant = new()
    {
      { "left", 2 },
      { "right", 2 },
    };
    internal Mob(int x, int y, MobType type, List<Direction> movableDirs) : base(x, y, type == MobType.RandomWalk ? "oneal" : "minvo", variant, "left")
    {
      Type = type;
      Debug.WriteLine($"Movable to {movableDirs.Count} dirs");
      if (movableDirs.Count == 0) return;
      Direction initDir = Utilities.GetRandomListElement(movableDirs);
      Move(initDir);
    }
    /// <summary>
    /// Callback after intersection reached
    /// </summary>
    public virtual void OnIntersectionReached(Direction currDir, HashSet<Direction> movableDirs, BeforeNextMoveEventArgs e)
    {
      Debug.WriteLine($"On intersection: {currDir}");
      Direction oppositeDirection = Utilities.GetOppositeDirection(currDir);
      Direction? nextDir = null;
      // to opposite direction
      if (Type == MobType.StraightWalk)
      {
        Debug.WriteLine($"[MOB]: to opposite {oppositeDirection}");
        nextDir = oppositeDirection;
      } else
      {
        // random choose a direction or opposite dir
        Direction randomDirection = Utilities.GetRandomDirection();
        // if the random dir is valid, then change dir, the dir can be either opposite or other valid dir
        if (randomDirection != currDir && movableDirs.Contains(randomDirection))
        {
          Debug.WriteLine($"[MOB_{Type}]: to random {randomDirection}");
          nextDir = randomDirection;
        } else
        {
          Debug.WriteLine($"[MOB_{Type}]: to oppo {oppositeDirection}");
          if (movableDirs.Contains(oppositeDirection))
          {
            nextDir = oppositeDirection;
          }
        }
      }
      if (nextDir != null && movableDirs.Contains((Direction)nextDir))
      {
        Debug.WriteLine($"[MOB_{Type}]: valid move to {nextDir}");
        ResetMove((Direction)nextDir);
        e.TurnDirection = nextDir;
      }
      else
      {
        Debug.WriteLine($"[MOB_{Type}]: Invalid move");
      }
    }
    public override void OnBeforeNextMove(object sender, BeforeNextMoveEventArgs e)
    {
      base.OnBeforeNextMove(sender, e);
    }
  }
}
