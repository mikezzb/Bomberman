using Bomberman.GameEngine.Enums;
using Bomberman.GameEngine.Graphics;
using Bomberman.GameEngine.MapObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Bomberman.GameEngine
{
  /// <summary>
  /// The Bomberman Game
  /// </summary>
  public class Game
  {
    public bool Started { get; protected set; }
    /// <summary>
    /// All visible stationary map objects
    /// <para>i.e. When powerup hiding behind brick, only brick in map</para>
    /// </summary>
    protected readonly Dictionary<int, MapObject> map = new();
    protected readonly Dictionary<int, Powerup> powerups = new();
    protected readonly List<Bomb> bombs = new();
    protected readonly List<Mob> mobs = new();
    protected readonly List<MapObject> backgrounds = new();
    protected readonly List<IUpdatable> updatables = new();
    private readonly GameSoundPlayer sp;
    /// <summary>
    /// Positions w/ blocker
    /// </summary>
    protected readonly HashSet<int> blockers = new();
    protected Key? key;
    protected Door door;
    protected Player player;
    protected FrameTimer timer;
    protected Action<GameEndType> onGameEnded;
    public Game(Action<GameEndType> onGameEnded)
    {
      timer = new FrameTimer(Config.FrameDuration, Config.FramesPerCycle, OnTimerTick);
      this.onGameEnded = onGameEnded;
      sp = GameSoundPlayer.Instance;
    }
    /// <summary>
    /// Passive lazy init
    /// </summary>
    public virtual void InitGame()
    {
      InitMapEntries();
      InitMobsMovement();
    }
    /// <summary>
    /// Start game
    /// </summary>
    public virtual void StartGame()
    {
      sp.PlaySound(GameSound.Bgm);
      timer.Start();
      Started = true;
    }
    // player controls
    public void PlayerStartWalk(Direction dir)
    {
      if (Started) player.Move(dir);
    }
    public void PlayerPlaceBomb()
    {
      bool validBombNum = player.CanPlaceBomb;
      if (validBombNum)
      {
        IntPoint pos = player.Position;
        // if already planted at that position
        if (GetMapObjectAt(pos.Index) != null)
        {
          Debug.WriteLine("[BOMB]: BLOCKED");
          Debug.WriteLine(GetMapObjectAt(pos.Index));
          return;
        };
        // plant
        player.PlaceBomb();
        CreateBomb(pos);
      }
    }
    public void PlayerStopWalk(Direction dir)
    {
      player.StopMove(dir);
    }
    protected virtual void InitMapEntries()
    {
      // init the stationary map (walls & floor)
      InitMap();
      // init bricks & their behinds
      InitBricksAndBehinds();
      // init player
      InitPlayer();
      // init mobs
      InitMobs();
    }
    protected virtual void InitMobs()
    {
      Debug.WriteLine("[INIT]: Mobs");
      List<IntPoint> positions = GetRandomPositions(Config.NumMobs, false, Config.MobMinX, Config.MobMinY);
      int count = 0;
      foreach (IntPoint pos in positions)
      {
        MobType type = (++count > Config.NumStraightMob) ? MobType.RandomWalk : MobType.StraightWalk;
        CreateMob(pos, type);
      }
    }
    /// <summary>
    /// After map is initiated, make mobs movable (pick a movable direction)
    /// </summary>
    protected virtual void InitMobsMovement()
    {
      foreach (Mob mob in mobs)
      {
        List<Direction> directions = GetMovableDirectionsList(mob.MovablePosition);
        mob.InitMovement(directions);
      }
    }
    protected virtual void CreateMob(IntPoint pos, MobType type)
    {
      Mob mob = new(pos.X, pos.Y, type);
      mob.BeforeNextMove += OnBeforeNextMove;
      AddMob(mob);
    }
    protected virtual void InitPlayer(int x = 1, int y = 1)
    {
      Debug.WriteLine("[INIT]: Player");
      player = new Player(x, y);
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
      HashSet<int> magicBricks = Utilities.GetRandomNumbers(2 + Config.NumPowerups, 0, Config.NumBricks);
      int countMagicBricks = 0;
      for (int i = 0; i < Config.NumBricks; i++)
      {
        // blocker added here
        IntPoint pos = GetRandomPosition(true);
        CreateBrick(pos);
        // handle magic brick
        if (magicBricks.Contains(i))
        {
          if (countMagicBricks == 0)
          {
            // create key
            CreateKey(pos);
          }
          else if (countMagicBricks == 1)
          {
            // create door
            CreateDoor(pos);
          }
          else if (countMagicBricks < (2 + Config.NumBombNumPU))
          {
            CreatePowerup(pos, PowerupType.BombNum);
          }
          else
          {
            CreatePowerup(pos, PowerupType.BombRange);
          }
          countMagicBricks++;
        }
      }
    }
    protected void CreateBrick(IntPoint pos)
    {
      Brick brick = new Brick(pos.X, pos.Y);
      int posIndex = pos.Index;
      AddToMap(brick, posIndex, true, false);
    }
    protected void CreatePowerup(IntPoint pos, PowerupType type)
    {
      Powerup powerup = new(pos.X, pos.Y, type);
      powerups.Add(pos.Index, powerup);
    }
    protected void CreateKey(IntPoint pos)
    {
      key = new Key(pos.X, pos.Y);
    }
    protected void RemoveKey()
    {
      key?.Remove();
      key = null;
    }
    protected void CreateDoor(IntPoint pos)
    {
      door = new Door(pos.X, pos.Y);
    }
    /// <summary>
    /// Remove bricks from map and replace it with powerup behind (if any)
    /// </summary>
    /// <param name="brick"></param>
    protected void RemoveBrick(Brick brick)
    {
      int idx = brick.Position.Index;
      brick.Remove();
      blockers.Remove(idx);
      // replace brick with powerup
      if (powerups.ContainsKey(idx))
      {
        Debug.WriteLine($"Replace brick with powerup {idx}");
        map[idx] = powerups[idx];
      }
      else
      {
        Debug.WriteLine($"No powerup behind brick at {idx}");
        map.Remove(idx);
      }
    }
    protected void OnMobDead(Mob mob)
    {
      RemoveMob(mob);
    }
    protected void AddMob(Mob mob)
    {
      mobs.Add(mob);
      AddUpdatableMapObject(mob);
    }
    protected void RemoveMob(Mob mob)
    {
      mobs.Remove(mob);
      RemoveUpdatable(mob);
      mob.Remove();
    }
    protected void RemovePowerup(Powerup powerup)
    {
      int idx = powerup.Position.Index;
      Debug.WriteLine($"Remove powerup {idx}");
      RemoveFromMap(idx, powerup, false, false, true);
      powerups.Remove(idx);
    }
    protected void CreateBomb(IntPoint pos)
    {
      Bomb bomb = new(pos.X, pos.Y, player.BombRange, BombExplodeHandler, DelayedRemoveCallback);
      Debug.WriteLine($"Plcae Bomb w/ range {player.BombRange} @ {pos.Index}");
      AddToMap(bomb, pos.Index, true, true);
      bombs.Add(bomb);
    }
    protected void RemoveBomb(Bomb bomb)
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
      using (StringReader reader = new(mapDef ?? Constants.Map))
      {
        string line;
        int i = 0, j = 0;
        while ((line = reader.ReadLine()) != null)
        {
          foreach (char c in line)
          {
            InitMapEntry(c, j, i);
            j++;
          }
          i++;
          j = 0;
        }
      }
    }
    protected virtual void InitMapEntry(char c, int x, int y)
    {
      switch (c)
      {
        case (char)GameObject.Wall:
          Wall wall = new(x, y);
          AddToMap(wall, null, true, false);
          break;
        case (char)GameObject.Floor:
          Floor floor = new(x, y);
          backgrounds.Add(floor);
          break;
      }
    }
    protected void DelayedRemoveCallback(MapObject sender)
    {
      Type type = sender.GetType();
      if (type == typeof(Bomb))
      {
        RemoveBomb((Bomb)sender);
      }
    }
    protected void AddToMap(MapObject obj, int? index = null, bool isBlocker = false, bool isUpdateble = false)
    {
      int idx = index ?? obj.Position.Index;
      bool success = map.TryAdd(idx, obj);
      if (!success)
      {
        Debug.WriteLine($"FAIL TO ADD, EXISTS @{GetMapObjectAt(idx)}");
        throw new Exception();
      }
      if (isBlocker)
      {
        blockers.Add(idx);
      }
      if (isUpdateble)
      {
        AddUpdatableMapObject((IUpdatable)obj);
      }
    }
    protected void RemoveFromMap(int index, MapObject? obj = null, bool isBlocker = false, bool isUpdateble = false, bool removeObj = false)
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
    protected void BombExplodeHandler(object sender, EventArgs e)
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
            // if bomb, explode it
            else if (objType == typeof(Bomb))
            {
              Debug.WriteLine($"Chain reaction: {obj}");
              ((Bomb)obj).Explode();
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
    protected bool HasEntityAt<T>(int index)
    {
      MapObject? obj;
      map.TryGetValue(index, out obj);
      return obj != null && obj.GetType() == typeof(T);
    }
    protected MapObject? GetMapObjectAt(int index)
    {
      MapObject? obj;
      map.TryGetValue(index, out obj);
      return obj;
    }
    protected MapObject? GetEntityAt<T>(int index)
    {
      MapObject? obj = GetMapObjectAt(index);
      return obj != null && obj.GetType() == typeof(T) ? obj : default;
    }
    protected bool HasPowerupAt(int index) => HasEntityAt<Powerup>(index);
    protected Brick? GetBrickAt(int index) => (Brick?)GetEntityAt<Brick>(index);
    protected virtual void OnTimerTick(int frameNum)
    {
      if (!Started) return;
      // player mob collision
      for (int i = mobs.Count - 1; i >= 0; i--)
      {
        if (mobs[i].OverlapsWith(player))
        {
          Debug.WriteLine("[DEAD]: mob collision");
          OnPlayerDead();
        }
      }
      // key collision
      if (key != null && player.Position.Index == key.Position.Index)
      {
        player.ApplyKey();
        RemoveKey();
      }
      // door collision
      if (player.HasKey && player.Position.Index == door.Position.Index)
      {
        GameOver(GameEndType.Cleared);
      }
      // player powerup collision
      if (powerups.ContainsKey(player.Position.Index))
      {
        Powerup powerup = powerups[player.Position.Index];
        powerups.Remove(player.Position.Index);
        player.ApplyPowerup(powerup);
        RemovePowerup(powerup);
      }
      // bomb collision
      for (int i = bombs.Count - 1; i >= 0; i--)
      {
        Bomb bomb = bombs[i];
        if (bomb.Explosing)
        {
          // Debug.WriteLine($"Explosion checking: {bomb.Position}");
          for (int j = mobs.Count - 1; j >= 0; j--)
          {
            if (bomb.IntersectsWith(mobs[j]))
            {
              Debug.WriteLine("Mob killed by bomb");
              OnMobDead(mobs[j]);
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
      GameOver(GameEndType.Dead);
    }
    protected virtual void GameOver(GameEndType type)
    {
      Debug.WriteLine("game over");
      timer.Stop();
      foreach (MovableMapObject obj in mobs)
      {
        obj.StopMoveNow();
      }
      Started = false;
      onGameEnded.Invoke(type);
    }
    protected void AddUpdatableMapObject(IUpdatable obj)
    {
      updatables.Add(obj);
    }
    protected void RemoveUpdatable(IUpdatable obj)
    {
      updatables.Remove(obj);
    }
    /// <summary>
    /// Random non blocked
    /// </summary>
    /// <param name="isBlocker"></param>
    /// <param name="minX"></param>
    /// <param name="minY"></param>
    /// <returns></returns>
    protected IntPoint GetRandomPosition(bool isBlocker = true, int minX = 1, int minY = 1)
    {
      Random r = Config.Rnd;
      // 1 & -1 is cuz boundary all walls
      int x, y;
      int key;
      do
      {
        x = r.Next(minX, Config.Width - 1);
        y = r.Next(minY, Config.Height - 1);
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
    protected List<IntPoint> GetRandomPositions(int count, bool isBlocker = true, int minX = 1, int minY = 1)
    {
      List<IntPoint> positions = new();
      HashSet<int> generated = new();
      for (int i = 0; i < count; i++)
      {
        IntPoint pos;
        int idx;
        do
        {
          pos = GetRandomPosition(isBlocker, minX, minY);
          idx = pos.Index;
        } while (generated.Contains(idx));
        positions.Add(pos);
        generated.Add(idx);
      }
      return positions;
    }
    protected void OnBeforeNextMove(object sender, BeforeNextMoveEventArgs e)
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
        mob.OnIntersectionReached(movableDirs, e);
      }
    }
    // helpers
    protected bool CanMoveTo(IntPoint to)
    {
      // if blocked
      int posIndex = Point2Index(to.X, to.Y);
      if (blockers.Contains(posIndex)) return false;
      // if out bound
      if (PositionOutBound(to)) return false;
      return true;
    }
    protected static bool PositionOutBound(IntPoint p)
    {
      return p.X < 0 || p.X >= Config.Width || p.Y < 0 || p.Y >= Config.Height;
    }
    protected List<Direction> GetMovableDirectionsList(MovableGridPosition pos, Direction? exceptDir = null)
    {
      List<Direction> directions = new List<Direction>();
      // IntPoint from = obj.GridPosition;
      foreach (Direction dir in Constants.Directions)
      {
        if (dir == exceptDir) continue;
        IntPoint to = pos.PostTranslatePosition(dir);
        if (CanMoveTo(to)) directions.Add(dir);
      }
      return directions;
    }
    protected HashSet<Direction> GetMovableDirectionsSet(MovableGridPosition pos, Direction? exceptDir = null)
    {
      return new HashSet<Direction>(GetMovableDirectionsList(pos, exceptDir));
    }
    protected static int Point2Index(int x, int y)
    {
      return x * Config.Width + y;
    }
  }
}
