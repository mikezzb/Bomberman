using System.Collections.Generic;

namespace Bomberman.GameEngine.MapObjects
{
  /// <summary>
  /// Control the actions of a player (e.g. movement + drop bomb)
  /// - Not singleton for future multi-player
  /// </summary>
  internal class Player : MovableMapObject
  {
    private static readonly Dictionary<string, int?> variant = new()
    {
      { "default", 0 },
      { "up", 2 },
      { "down", 2 },
      { "left", 2 },
      { "right", 2 },
    };
    internal Player(int x, int y) : base(x, y, "player", variant, "down") { }
    /// <summary>
    /// Init player at top left corner
    /// </summary>
    internal Player() : base(1, 1, "player", variant, "down") { }
  }
}
