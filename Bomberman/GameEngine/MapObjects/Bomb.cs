using Bomberman.GameEngine.Enums;
using System;
using System.Collections.Generic;

namespace Bomberman.GameEngine.MapObjects
{
  internal class Bomb : MapObject, IUpdatable
  {
    public static Dictionary<Direction, string> Direction2Explosion = new()
    {
      { Direction.Left, "horizontal" },
      { Direction.Right, "horizontal" },
      { Direction.Up, "vertical" },
      { Direction.Down, "vertical" }
    };
    public static Dictionary<string, int?> variants = new()
    {
      { "bomb", 2 },
      {
        "exploded",
        2
      },
    };
    public int FrameNum { get; private set; }
    public int Range { get; private set; }
    public bool Explosing { get; private set; }
    public bool Exploded { get; private set; }
    private Action<MapObject> removeCallback;
    private List<Explosion> explosions;
    public event EventHandler ExplodeEvent;
    /// <summary>
    /// [minX, maxX, minY, maxY] for fast intersections check
    /// </summary>
    private int[] explosionSpan = new int[4] {
      int.MaxValue,
      0,
      int.MaxValue,
      0
    };
    private int MinX
    {
      get => explosionSpan[0]; set
      {
        explosionSpan[0] = value;
      }
    }
    private int MaxX
    {
      get => explosionSpan[1]; set
      {
        explosionSpan[1] = value;
      }
    }
    private int MinY
    {
      get => explosionSpan[2]; set
      {
        explosionSpan[2] = value;
      }
    }
    private int MaxY
    {
      get => explosionSpan[3]; set
      {
        explosionSpan[3] = value;
      }
    }

    internal Bomb(int x, int y, int range, EventHandler ExplodeHandler, Action<MapObject> removeCallback) : base(x, y, "bomb", variants, null, 1, true)
    {
      this.removeCallback = removeCallback;
      ExplodeEvent += ExplodeHandler;
      Range = range;
    }
    public void Update()
    {
      if (FrameNum++ == Config.NumFramesTillExplode)
      {
        Explode();
      }
      else if (FrameNum == Config.NumFramesTillExplosionFinish)
      {
        Remove();
      }
    }
    /// <summary>
    /// Handle explosion animation
    /// </summary>
    public void Explode()
    {
      sprite.SwitchImage("exploded");
      Explosing = true;
      Exploded = false;
      // set explosion animation
      explosions = new List<Explosion>();
      // trigger event
      ExplodeEvent?.Invoke(this, new EventArgs());
    }
    public void DrawExplosions(Dictionary<Direction, List<IntPoint>> drawableRange)
    {
      foreach (KeyValuePair<Direction, List<IntPoint>> pair in drawableRange)
      {
        int len = pair.Value.Count;
        if (len == 0) continue;

        for (int i = 0; i < len; i++)
        {
          string explosionVariant = Direction2Explosion[pair.Key];
          IntPoint pos = pair.Value[i];
          if (i == (len - 1))
          {
            explosionVariant += $"_{Constants.DirectionName[pair.Key]}_last";
          }
          if (pos.X < MinX)
          {
            MinX = pos.X;
          }
          else if (pos.X > MaxX)
          {
            MaxX = pos.X;
          }
          if (pos.Y < MinY)
          {
            MinY = pos.Y;
          }
          else if (pos.Y > MaxY)
          {
            MaxY = pos.Y;
          }
          explosions.Add(new Explosion(pos.X, pos.Y, explosionVariant));
        }
      }
    }
    public override void Remove()
    {
      Exploded = true;
      removeCallback(this);
      foreach (Explosion exp in explosions)
      {
        exp.Remove();
      }
      base.Remove();
    }
    public bool IntersectsWith(MovableMapObject obj)
    {
      if (Exploded) return false;
      bool xIntersets = obj.MovablePosition.PreciseX > MinX && obj.MovablePosition.PreciseX < MaxX;
      bool yIntersets = obj.MovablePosition.PreciseY > MinY && obj.MovablePosition.PreciseY < MaxY;
      return xIntersets && yIntersets;
    }
  }
}
