using Bomberman.GameEngine.Enums;
using Bomberman.GameEngine.MapObjects;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

namespace Bomberman.GameEngine
{
  /// <summary>
  /// Controller of the game
  /// - GameLoop
  /// - GameOver
  /// - GameSetup
  /// 
  /// </summary>
  public class Game
  {
    private readonly List<MapObject> map = new();
    /// <summary>
    /// Key: position
    /// Value: if blocker exists on that position
    /// </summary>
    private readonly HashSet<string> blockers = new();
    private Player player;
    private static Game? instance;
    public static Game Instance
    {
      get
      {
        if (instance == null) instance = new Game();
        return instance;
      }
    }
    private Game() { }
    public void InitGame()
    {
      player = new Player();
      player.BeforeNextMove += OnBeforeNextMove;
      InitMap();
    }
    public void InitMap(string? mapDef = null)
    {
      using (StringReader reader = new(mapDef ?? Config.Map))
      {
        string line;
        int i = 0, j = 0;
        while ((line = reader.ReadLine()) != null)
        {
          Debug.WriteLine(line);
          foreach (char c in line)
          {
            bool isBlocking = false;
            switch (c)
            {
              case (char)GameObject.Wall:
                Debug.WriteLine("Wall");
                Wall wall = new (j, i);
                isBlocking = true;
                map.Add(wall);
                break;
              case (char)GameObject.Floor:
                Debug.WriteLine("Floor");
                Floor floor = new (j, i);
                map.Add(floor);
                break;
            }
            if (isBlocking) {
              blockers.Add(Point2Key(j, i));
            }
            j++;
          }
          i++;
          j = 0;
        }
      }
    }
    // player controls
    public void PlayerStartWalk(Direction dir)
    {
      // if blocked
      IntPoint afterPos = player.PostMovePosition(dir);
      Debug.WriteLine($"[CURR]: x={player.MovablePosition.PreciseX} y={player.MovablePosition.PreciseY}");
      string afterPosKey = Point2Key(afterPos.X, afterPos.Y);
      Debug.WriteLine($"[AFTER]: x={afterPos.X} y={afterPos.Y} | {afterPosKey}");
      if (!CanMoveTo(afterPos))
      {
        Debug.WriteLine($"Blocked");
        return;
      }
      // move
      player.Move(dir);
    }
    public void OnBeforeNextMove(object sender, BeforeNextMoveEventArgs e)
    {
      Debug.WriteLine("On before next move called");
      Debug.WriteLine($"To: {e.To.X}-{e.To.Y}");
      // check can move (wall / brick / bomb)
      e.Cancel = !CanMoveTo(e.To);
    }
    public bool CanMoveTo(IntPoint to)
    {
      string posKey = Point2Key(to.X, to.Y);
      return !blockers.Contains(posKey);
    }
    public void PlayerStopWalk(Direction dir)
    {
      player.StopMove(dir);
    }
    public static string Point2Key(int x, int y)
    {
      return $"{x}_{y}";
    }
  }
}
