using Bomberman.GameEngine.Enums;
using Bomberman.GameEngine.MapObjects;
using Bomberman.GameEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
    /// <summary>
    /// Key: index
    /// Value: a map object
    /// </summary>
    public bool Started { get; private set; }
    private readonly Dictionary<int, MapObject> map = new();
    private readonly Dictionary<int, Powerup> powerups = new();
    private readonly Dictionary<int, Mob> mobs = new();
    private readonly List<MapObject> updatableMapObjects = new();
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
    private FrameTimer timer;
    private Game() {
      timer = new FrameTimer(Config.WalkFrameDuration, Config.WalkFrames, OnTimerTick);
    }
    protected virtual void OnTimerTick(int frameNum)
    {
      if (!Started) return;
      // player mob collision
      foreach(Mob mob in mobs.Values)
      {
        if (mob.OverlapsWith(player))
        {
          Debug.WriteLine("Overlap");
          OnPlayerDead();
        }
      }
      // update all updatable objects
      foreach(MapObject obj in updatableMapObjects)
      {
        obj.Update();
      }
    }
    protected virtual void OnPlayerDead()
    {
      Debug.WriteLine("player dead");
      player.Dead();
      GameOver();
    }
    protected virtual void GameOver()
    {
      Debug.WriteLine("game over");
      timer.Stop();
      foreach(MovableMapObject obj in mobs.Values)
      {
        obj.StopMoveNow();
      }
      Started = false;
    }
    public IntPoint GetRandomPosition(bool isBlocker = true)
    {
      Random r = new();
      // 1 & -1 is cuz boundary all walls
      int x, y;
      string key;
      do
      {
        x = r.Next(1, Config.Width - 1);
        y = r.Next(1, Config.Height - 1);
        key = Point2Key(x, y);
      } while (
        blockers.Contains(key) ||
        (x == 1 && y == 1) ||
        (x == 1 && y == 2) ||
        (x == 2 && y == 1)
       );
      if (isBlocker)
      {
        blockers.Add(key);
      }
      return new IntPoint(x, y);
    }
    public List<IntPoint> GetRandomPositions(int count, bool isBlocker = true)
    {
      List<IntPoint> positions = new();
      HashSet<int> generated = new();
      for (int i = 0; i < count; i++)
      {
        IntPoint pos;
        int idx;
        do
        {
          pos = GetRandomPosition(isBlocker);
          idx = Point2Index(pos);
        } while (generated.Contains(idx));
        positions.Add(pos);
        generated.Add(idx);
      }
      return positions;
    }
    public void InitGame()
    {
      // init the stationary map (walls & floor)
      InitMap();
      // init bricks & their behinds
      InitBricksAndBehinds();
      // init player
      InitPlayer();
      // init mobs
      InitMobs();
      timer.Start();
      Started = true;
    }
    public static HashSet<int> GetRandomNumbers(int count, int min, int max)
    {
      HashSet<int> numbers = new();
      Random r = new();
      for (int i = 0; i < count; i++)
      {
        int num;
        do
        {
          num = r.Next(min, max);
        } while (numbers.Contains(num));
        numbers.Add(num);
      }
      return numbers;
    }
    protected virtual void InitMobs()
    {
      Debug.WriteLine("[INIT]: Mobs");
      List<IntPoint> positions = GetRandomPositions(Config.NumMobs, false);
      int count = 0;
      foreach(IntPoint pos in positions)
      {
        List<Direction> directions = GetMovableDirectionsList(new MovableGridPosition(pos));
        MobType type = (++count > Config.NumStraightMob) ? MobType.RandomWalk : MobType.StraightWalk;
        Mob mob = new(pos.X, pos.Y, type, directions);
        mob.BeforeNextMove += OnBeforeNextMove;
        mobs.Add(Point2Index(pos), mob);
      }
    }
    protected virtual void InitPlayer()
    {
      Debug.WriteLine("[INIT]: Player");
      player = new Player();
      player.BeforeNextMove += OnBeforeNextMove;

    }
    protected virtual void InitBricksAndBehinds()
    {
      Debug.WriteLine("[INIT]: Bricks & Behinds");
      /*
       * random pick some bricks for hiding pu & key & door
       * - 0, 1 for key & door
       * - rest for powerups
       */
      HashSet<int> magicBricks = GetRandomNumbers(2 + Config.NumPowerups, 0, Config.NumBricks);
      int countMagicBricks = 0;
      for (int i = 0; i < Config.NumBricks; i++)
      {
        IntPoint pos = GetRandomPosition(true);
        Brick brick = new Brick(pos.X, pos.Y);
        Debug.WriteLine(pos);
        int posIndex = Point2Index(pos);
        map.Add(posIndex, brick);
        // handle magic brick
        if (magicBricks.Contains(i))
        {
          if (countMagicBricks == 0)
          {
            // add key
          } else if (countMagicBricks == 1)
          {
            // add door
          } else if (countMagicBricks < (2 + Config.NumSpeedPU))
          {
            PowerupSpeed powerupSpeed = new(pos.X, pos.Y);
            powerups.Add(posIndex, powerupSpeed);
          } else
          {
            PowerupBomb powerupBomb = new(pos.X, pos.Y);
            powerups.Add(posIndex, powerupBomb);
          }
          countMagicBricks++;
        }
      }
    }
    public void RemoveBrick(IntPoint point)
    {
      blockers.Remove(Point2Key(point));
    }
    /// <summary>
    /// Init stationary map according to map definition
    /// </summary>
    /// <param name="mapDef"></param>
    protected virtual void InitMap(string? mapDef = null)
    {
      Debug.WriteLine("[INIT]: Map");
      using (StringReader reader = new(mapDef ?? Config.Map))
      {
        string line;
        int i = 0, j = 0;
        while ((line = reader.ReadLine()) != null)
        {
          foreach (char c in line)
          {
            bool isBlocking = false;
            switch (c)
            {
              case (char)GameObject.Wall:
                Wall wall = new(j, i);
                isBlocking = true;
                map.Add(Point2Index(j, i), wall);
                break;
              case (char)GameObject.Floor:
                Floor floor = new(j, i);
                break;
            }
            if (isBlocking)
            {
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
      if (Started) player.Move(dir);
    }
    public void PlayerPlaceBomb()
    {
      bool success = player.PlaceBomb();
      if (success)
      {
        IntPoint pos = player.Position.Position;
        Bomb bomb = new Bomb(pos.X, pos.Y, OnBombExploded);
        updatableMapObjects.Add(bomb);
        blockers.Add(bomb.Position.Key);
      }
    }
    public void OnBombExploded(object sender, EventArgs e)
    {
      Bomb bomb = (Bomb)sender;
      player.RemoveBomb();
      blockers.Remove(bomb.Position.Key);
    }
    public void OnBeforeNextMove(object sender, BeforeNextMoveEventArgs e)
    {
      // Debug.WriteLine($"{sender}: On before next move called");
      // Debug.WriteLine($"To: {e.To.X}-{e.To.Y}");
      // check can move (wall / brick / bomb)
      e.Cancel = !CanMoveTo(e.To);
      // if type is mob and blocked, call on intersection reached
      if (e.Cancel && sender.GetType() == typeof(Mob))
      {
        Mob mob = (Mob)sender;
        HashSet<Direction>? movableDirs = GetMovableDirectionsSet((MovableGridPosition)mob.Position, mob.CurrDir);
        mob.OnIntersectionReached(mob.CurrDir, movableDirs, e);
      }
    }
    // helpers
    public bool CanMoveTo(IntPoint to)
    {
      // if blocked
      string posKey = Point2Key(to.X, to.Y);
      if (blockers.Contains(posKey)) return false;
      // if out bound
      if (to.X < 0 || to.X >= Config.Width || to.Y < 0 || to.Y >= Config.Height) return false;
      return true;
    }
    public List<Direction> GetMovableDirectionsList(MovableGridPosition pos, Direction? exceptDir = null)
    {
      List<Direction> directions = new List<Direction>();
      // IntPoint from = obj.GridPosition;
      foreach(Direction dir in Config.Directions)
      {
        if (dir == exceptDir) continue;
          IntPoint to = pos.PostMovePosition(dir);
        if (CanMoveTo(to)) directions.Add(dir);
      }
      return directions;
    }
    public HashSet<Direction> GetMovableDirectionsSet(MovableGridPosition pos, Direction? exceptDir = null)
    {
      return new HashSet<Direction>(GetMovableDirectionsList(pos, exceptDir));
    }
    public void PlayerStopWalk(Direction dir)
    {
      player.StopMove(dir);
    }
    public static int Point2Index(IntPoint point)
    {
      return Point2Index(point.X, point.Y);
    }
    public static int Point2Index(int x, int y)
    {
      return x * Config.Width + y;
    }
    public static string Point2Key(IntPoint point)
    {
      return Point2Key(point.X, point.Y);
    }
    public static string Point2Key(int x, int y)
    {
      return $"{x}_{y}";
    }
  }
}
