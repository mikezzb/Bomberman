using Bomberman.GameEngine.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bomberman.GameEngine.MapObjects
{
  public class Bomb : AnimatedMapObject
  {
    public event EventHandler ExplodeEvent;
    private static readonly Dictionary<Direction, string> Direction2ExplosionName = new()
    {
      { Direction.Left, "horizontal" },
      { Direction.Right, "horizontal" },
      { Direction.Up, "vertical" },
      { Direction.Down, "vertical" }
    };
    private static readonly Dictionary<string, int?> variants = new()
    {
      { "original", 2 },
      { "exploded", 2 }
    };
    private Action<MapObject> removeCallback;
    private List<Explosion> explosions;
    // for intersection detection
    private int minX = int.MaxValue;
    private int maxX = 0;
    private int minY = int.MaxValue;
    private int maxY = 0;
    private GameEngineRect? explosionHorizontalRect;
    private GameEngineRect? explosionVerticalRect;
    public Bomb(int x, int y, int range, EventHandler ExplodeHandler, Action<MapObject> removeCallback) : base(x, y, "bomb", variants, "original", 1)
    {
      this.removeCallback = removeCallback;
      ExplodeEvent += ExplodeHandler;
      Range = range;
      animatedSprite.StartAnimation();
    }
    public int Range { get; private set; }
    public bool Explosing { get; private set; }
    public bool Exploded { get; private set; }
    public override void Update()
    {
      base.Update();
      if (FrameNum++ == Config.NumFramesTillExplode)
      {
        Explode(false);
      }
      else if (FrameNum == Config.NumFramesTillExplosionFinish)
      {
        Remove();
      }
      else if (Explosing && !Exploded)
      {
        // explosion animation
        foreach (Explosion exp in explosions)
        {
          exp.Update();
        }
      }
    }
    /// <summary>
    /// Manual explode method
    /// </summary>
    public void Explode()
    {
      Explode(true);
    }
    /// <summary>
    /// Draw explosions based on info from Game
    /// </summary>
    /// <param name="drawableRange"></param>
    public void DrawExplosions(Dictionary<Direction, List<IntPoint>> drawableRange)
    {
      foreach (KeyValuePair<Direction, List<IntPoint>> pair in drawableRange)
      {
        int len = pair.Value.Count;
        if (len == 0) continue;

        for (int i = 0; i < len; i++)
        {
          string explosionVariant = Direction2ExplosionName[pair.Key];
          IntPoint pos = pair.Value[i];
          if (i == (len - 1))
          {
            explosionVariant += $"_{Constants.DirectionName[pair.Key]}_last";
          }
          UpdateExtreme(pos);
          Explosion explosion = new Explosion(pos.X, pos.Y, explosionVariant);
          explosions.Add(explosion);
        }
      }
      UpdateExtreme(Position);
      UpdateExplosionRect();
    }
    /// <summary>
    /// Explode logic
    /// </summary>
    /// <param name="manual"></param>
    private void Explode(bool manual)
    {
      // if ticker triggered same time with manual, prevent explode twice
      if (Explosing || Exploded) return;
      Debug.WriteLine($"[EXPLODE]: manual={manual}");
      if (manual)
      {
        FrameNum = Config.NumFramesTillExplode; // to start count explosion animation
      }
      sprite.SwitchImage("exploded");
      Explosing = true;
      Exploded = false;
      // set explosion animation
      explosions = new List<Explosion>();
      // trigger event
      ExplodeEvent?.Invoke(this, new EventArgs());
    }
    private void UpdateExplosionRect()
    {
      // Debug.WriteLine($"X: {minX}-{maxX} Y: {minY}-{maxY}");
      int width = maxX - minX + 1;
      int height = maxY - minY + 1;
      // Debug.WriteLine($"Width: {width} Height: {height}");
      explosionHorizontalRect = new(minX, Position.Y, width, 1);
      explosionVerticalRect = new(Position.X, minY, 1, height);
      // Debug.WriteLine($"[EXPL HORI]: {explosionHorizontalRect}");
      // Debug.WriteLine($"[EXPL VERT]: {explosionVerticalRect}");
    }
    private void UpdateExtreme(IntPoint pos)
    {
      if (pos.X < minX)
      {
        minX = pos.X;
      }
      if (pos.X > maxX)
      {
        maxX = pos.X;
      }
      if (pos.Y < minY)
      {
        minY = pos.Y;
      }
      if (pos.Y > maxY)
      {
        maxY = pos.Y;
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
      GameEngineRect objRect = obj.MovablePosition.Rect;
      bool horiIntersects = objRect.OverlapsWith(explosionHorizontalRect);
      if (horiIntersects) return true;
      bool vertIntersects = objRect.OverlapsWith(explosionVerticalRect);
      return vertIntersects;
    }
  }
}
