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
  public class Player : MovableMapObject
  {
    private int numBombs = 1;
    private int placedBombs = 0;
    public int BombRange { get; private set; }
    public bool HasKey { get; private set; }
    private static readonly Dictionary<string, int?> variant = new()
    {
      { "dead", 2 },
      { "up", 2 },
      { "down", 2 },
      { "left", 2 },
      { "right", 2 },
    };
    public Player(int x, int y, EventHandler<BeforeNextMoveEventArgs> onBeforeNextMove) : base(x, y, onBeforeNextMove, "player", variant, "down", 1, 3)
    {
      BombRange = 1;
    }
    /// <summary>
    /// Init player at top left corner
    /// </summary>
    public Player(EventHandler<BeforeNextMoveEventArgs> onBeforeNextMove) : this(1, 1, onBeforeNextMove) { }
    /// <summary>
    /// Check if can place bomb, if can, increase count
    /// </summary>
    public bool CanPlaceBomb
    {
      get => placedBombs < numBombs;
    }
    public void PlaceBomb()
    {
      Debug.WriteLine($"[INCR] Place bomb {placedBombs}/{numBombs} placed");
      placedBombs++;
    }
    public void RemoveBomb()
    {
      placedBombs--;
      Debug.WriteLine($"[DESC] Remove bomb {placedBombs}/{numBombs} placed");
    }

    public void ApplyKey()
    {
      HasKey = true;
    }
    public void ApplyPowerup(Powerup powerup)
    {
      Debug.WriteLine($"Apply powerup: {powerup.Type}");
      switch (powerup.Type)
      {
        case PowerupType.Speed:
          Debug.WriteLine("Speed up");
          SpeedUp();
          break;
        case PowerupType.BombRange:
          Debug.WriteLine("Bomb range up");
          BombRange++;
          break;
        case PowerupType.BombNum:
          Debug.WriteLine("Bomb number up");
          numBombs++;
          break;
      }
    }
  }
}
