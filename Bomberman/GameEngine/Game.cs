using Bomberman.GameEngine.Enums;
using Bomberman.GameEngine.Graphics;
using Bomberman.GameEngine.MapObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

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
    /// <summary>
    /// All visible stationary map objects
    /// <para>- When powerup hiding behind brick, only brick in map</para>
    /// </summary>
    private readonly Dictionary<int, MapObject> map = new();
    private readonly Dictionary<int, Powerup> powerups = new();
    private readonly List<Bomb> bombs = new();
    private readonly List<Mob> mobs = new();
    private readonly List<IUpdatable> updatables = new();
    /// <summary>
    /// Key: position
    /// Value: if blocker exists on that position
    /// </summary>
    private readonly HashSet<int> blockers = new();
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
    private Game()
    {
      timer = new FrameTimer(Config.WalkFrameDuration, Config.WalkFrames, OnTimerTick);
    }
    private bool HasEntityAt<T>(int index)
    {
      MapObject? obj;
      map.TryGetValue(index, out obj);
      return obj.GetType() == typeof(T);
    }
    private MapObject? GetMapObjectAt(int index)
    {
      MapObject? obj;
      map.TryGetValue(index, out obj);
      return obj;
    }
    private MapObject? GetEntityAt<T>(int index)
    {
      MapObject? obj = GetMapObjectAt(index);
      return obj != null && obj.GetType() == typeof(T) ? obj : default;
    }
    private bool HasPowerupAt(int index) => HasEntityAt<Powerup>(index);
    private Brick? GetBrickAt(int index) => (Brick?)GetEntityAt<Brick>(index);
    protected virtual void OnTimerTick(int frameNum)
    {
      if (!Started) return;
      // player mob collision
      for (int i = mobs.Count - 1; i >= 0; i--)
      {
        if (mobs[i].OverlapsWith(player))
        {
          Debug.WriteLine("Overlap");
          OnPlayerDead();
        }
      }
      // player powerup collision
      if (powerups.ContainsKey(player.Position.Index))
      {
        Powerup powerup = powerups[player.Position.Index];
        powerups.Remove(player.Position.Index);
        player.ConsumePowerup(powerup);
      }
      // bomb collision
      for (int i = bombs.Count - 1; i >= 0; i--)
      {
        Bomb bomb = bombs[i];
        if (bomb.Explosing)
        {
          Debug.WriteLine($"Explosion checking: {bomb.Position}");
          foreach (Mob mob in mobs)
          {
            if (bomb.IntersectsWith(mob))
            {
              Debug.WriteLine("Mob killed by bomb");
              mob.Dead();
            }
          }
          if (bomb.IntersectsWith(player))
          {
            Debug.WriteLine("Player killed by bomb");
            OnPlayerDead();
          }
        }
      }

      // update all updatable objects
      for (int i = updatables.Count - 1; i >= 0; i--)
      {
        updatables[i].Update();
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
      foreach (MovableMapObject obj in mobs)
      {
        obj.StopMoveNow();
      }
      Started = false;
    }
    private void AddUpdatableMapObject(IUpdatable obj)
    {
      updatables.Add(obj);
    }
    private void RemoveUpdatable(IUpdatable obj)
    {
      updatables.Remove(obj);
    }
    public IntPoint GetRandomPosition(bool isBlocker = true, int minX = 1, int minY = 1)
    {
      Random r = new();
      // 1 & -1 is cuz boundary all walls
      int x, y;
      int key;
      do
      {
        x = r.Next(1, Config.Width - 1);
        y = r.Next(1, Config.Height - 1);
        key = Point2Index(x, y);
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
    public List<IntPoint> GetRandomPositions(int count, bool isBlocker = true, int minX = 1, int minY = 1)
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
      List<IntPoint> positions = GetRandomPositions(Config.NumMobs, false, 7, 7);
      int count = 0;
      foreach (IntPoint pos in positions)
      {
        List<Direction> directions = GetMovableDirectionsList(new MovableGridPosition(pos));
        MobType type = (++count > Config.NumStraightMob) ? MobType.RandomWalk : MobType.StraightWalk;
        Mob mob = new(pos.X, pos.Y, type, directions);
        mob.BeforeNextMove += OnBeforeNextMove;
        mobs.Add(mob);
        AddUpdatableMapObject(mob);
      }
    }
    protected virtual void InitPlayer()
    {
      Debug.WriteLine("[INIT]: Player");
      player = new Player();
      player.BeforeNextMove += OnBeforeNextMove;
      AddUpdatableMapObject(player);
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
        // blocker added here
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
          }
          else if (countMagicBricks == 1)
          {
            // add door
          }
          else if (countMagicBricks < (2 + Config.NumBombNumPU))
          {
            Powerup powerupBN = new(pos.X, pos.Y, PowerupType.BombNum);
            powerups.Add(posIndex, powerupBN);
          }
          else
          {
            Powerup powerupBR = new(pos.X, pos.Y, PowerupType.BombRange);
            powerups.Add(posIndex, powerupBR);
          }
          countMagicBricks++;
        }
      }
    }
    private void RemoveBrick(Brick brick)
    {
      int idx = brick.Position.Index;
      brick.Remove();
      blockers.Remove(idx);
      // replace brick with powerup
      if (powerups.ContainsKey(idx))
      {
        map[idx] = powerups[idx];
      }
      else
      {
        map.Remove(idx);
      }
    }
    private void RemovePowerup(Powerup powerup)
    {
      int idx = powerup.Position.Index;
      RemoveFromMap(idx, powerup, false, false, true);
      powerups.Remove(idx);
    }
    private void RemoveBomb(Bomb bomb)
    {
      Debug.WriteLine($"Removing bomb");
      RemoveFromMap(bomb.Position.Index, bomb, false, true, false);
      bombs.Remove(bomb);
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
            switch (c)
            {
              case (char)GameObject.Wall:
                Wall wall = new(j, i);
                AddToMap(wall, null, true, false);
                break;
              case (char)GameObject.Floor:
                Floor floor = new(j, i);
                break;
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
        IntPoint pos = player.Position;
        // if already planted
        if (GetMapObjectAt(pos.Index) != null) return;
        Debug.WriteLine($"Plcae Bomb w/ range {player.BombRange}");
        Bomb bomb = new(pos.X, pos.Y, player.BombRange, BombExplodeHandler, DelayedRemoveCallback);
        AddToMap(bomb, pos.Index, true, true);
        bombs.Add(bomb);
      }
    }
    private void DelayedRemoveCallback(MapObject sender)
    {
      Type type = sender.GetType();
      if (type == typeof(Bomb))
      {
        RemoveBomb((Bomb)sender);
      }
    }
    private void AddToMap(MapObject obj, int? index = null, bool isBlocker = false, bool isUpdateble = false)
    {
      int idx = index ?? obj.Position.Index;
      map.Add(idx, obj);
      if (isBlocker)
      {
        blockers.Add(idx);
      }
      if (isUpdateble)
      {
        AddUpdatableMapObject((IUpdatable)obj);
      }
    }
    private void RemoveFromMap(int index, MapObject? obj = null, bool isBlocker = false, bool isUpdateble = false, bool removeObj = false)
    {
      map.Remove(index);
      if (isBlocker) blockers.Remove(index);
      if (obj != null)
      {
        if (isUpdateble) RemoveUpdatable((IUpdatable)obj);
        if (removeObj)
        {
          obj.Remove();
        };
      }
    }
    private async void RemoveAfter(int index, int duratonInMs, MapObject? obj = null, bool isBlocker = false, bool isUpdateble = false, bool removeObj = false)
    {
      await Task.Delay(duratonInMs);
      RemoveFromMap(index, obj, isBlocker, isUpdateble, removeObj);
    }
    private void BombExplodeHandler(object sender, EventArgs e)
    {
      Debug.WriteLine("Exploded");
      Bomb bomb = (Bomb)sender;
      player.RemoveBomb();
      blockers.Remove(bomb.Position.Index);
      Dictionary<Direction, List<IntPoint>> canMoveTo = new();
      // check explosivable directions
      foreach (Direction dir in Constants.Directions)
      {
        List<IntPoint> positions = new();
        GridPosition anchor = new(bomb.Position.X, bomb.Position.Y);
        for (int i = 0; i < bomb.Range; i++)
        {
          // trans 1 unit to dir from anchor
          IntPoint to = anchor.PostTranslatePosition(dir);
          // check for explosion blocking (i.e. brick / block)
          int posIdx = to.Index;
          MapObject? obj = GetMapObjectAt(posIdx);
          Debug.WriteLine($"[BOMB_{dir}]: Found {obj} at {posIdx} | {to}");
          // if has object at that position, handle according to type
          if (obj != null)
          {
            Type objType = obj.GetType();
            // if has brick in direction, destroy & stop prop
            if (objType == typeof(Brick))
            {
              Debug.WriteLine($"[BOMB]: remove brick {posIdx}");
              RemoveBrick((Brick)obj);
              break;
            }
            // if has wall, stop prop
            else if (objType == typeof(Wall))
            {
              Debug.WriteLine($"[BOMB]: hit wall");
              break;
            }
            // if powerup on map, destroy (Note: only exposed powerup will on map & need destroy, so no need check powerups dict)
            else if (objType == typeof(Powerup))
            {
              RemovePowerup((Powerup)obj);
            }
          }
          Debug.WriteLine(to);
          // add as position of explosives
          positions.Add(to);
          // move anchor to that dir for 1 unit
          anchor.Set(to.X, to.Y);
        }
        canMoveTo.Add(dir, positions);
      }
      bomb.DrawExplosions(canMoveTo);

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
        mob.OnIntersectionReached((Direction)mob.CurrDir, movableDirs, e);
      }
    }
    // helpers
    public bool CanMoveTo(IntPoint to)
    {
      // if blocked
      int posIndex = Point2Index(to.X, to.Y);
      if (blockers.Contains(posIndex)) return false;
      // if out bound
      if (PositionOutBound(to)) return false;
      return true;
    }
    public static bool PositionOutBound(IntPoint p)
    {
      return p.X < 0 || p.X >= Config.Width || p.Y < 0 || p.Y >= Config.Height;
    }
    public List<Direction> GetMovableDirectionsList(MovableGridPosition pos, Direction? exceptDir = null)
    {
      List<Direction> directions = new List<Direction>();
      // IntPoint from = obj.GridPosition;
      foreach (Direction dir in Config.Directions)
      {
        if (dir == exceptDir) continue;
        IntPoint to = pos.PostTranslatePosition(dir);
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
  }
}
