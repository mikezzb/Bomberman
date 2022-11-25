using Bomberman.GameEngine.Enums;
using Bomberman.GameEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bomberman.GameEngine.MapObjects
{
  /// <summary>
  /// Abstract class of movable map object in game, including everything visible on the map
  /// <list type="bullet">
  /// <item>Position</item>
  /// <item>Display (Image)</item>
  /// </list>
  /// </summary>
  public abstract class MovableMapObject : AnimatedMapObject
  {
    protected int removeCountdown = Config.RemoveDeadObjectAfterFrameNum;
    private double speed;
    public bool IsDead { get; protected set; }
    protected readonly MovableGridPosition movablePosition;
    private Direction? currDir;
    private Direction? nextDir;
    private bool stopAfterMove;
    public bool Moving { get => currDir != null; }
    public Direction? CurrDir { get => currDir; }
    public event EventHandler<BeforeNextMoveEventArgs> BeforeNextMove;

    public MovableGridPosition MovablePosition { get => movablePosition; }
    protected MovableMapObject(int x, int y, EventHandler<BeforeNextMoveEventArgs> onBeforeNextMove, string name, Dictionary<string, int?>? variant, string? defaultVariant, double speed = 1.0, int zIndex = 2) : base()
    {
      BeforeNextMove += onBeforeNextMove;
      this.speed = speed;
      Position = new MovableGridPosition(x, y);
      sprite = new AnimatedSprite(name, variant, defaultVariant, Position, zIndex, true);
      animatedSprite = (AnimatedSprite)sprite;
      movablePosition = (MovableGridPosition)Position;
      Debug.WriteLine($"Speed {this.speed} | {WalkSize}");
    }
    private double WalkSize { get => (speed / Config.FramesPerCycle); }
    public IntPoint PostMovePosition(Direction dir)
    {
      return movablePosition.PostTranslatePosition(dir);
    }
    /// <summary>
    /// Move image on canvas (for walking animation)
    /// </summary>
    public override void Update()
    {
      if (IsDead)
      {
        if (removeCountdown-- == 0)
        {
          Remove();
        }
        base.Update();
        return;
      }
      // nothing to update if not moving
      if (currDir == null) return;
      // shift position & draw
      movablePosition.Move(currDir, WalkSize);
      base.Update();
      if (++FrameNum % Config.FramesPerCycle == 0) OnMoveEnded();
    }

    /// <summary>
    /// Move sprite until stop move
    /// <para>Each walk is 1 unit only</para>
    /// </summary>
    public void Move(Direction dir)
    {
      stopAfterMove = false;
      if (currDir != null)
      {
        Debug.WriteLine($"[NEXT MOVE]: {dir}");
        nextDir = dir;
        return;
      }
      StartMove(dir);
    }
    public void StartMove(Direction dir, bool skipCheck = false)
    {
      if (!skipCheck && CanMove(dir).Cancel) return;
      // Debug.WriteLine($"[MOVE]: {dir}");
      currDir = dir;
      SwitchMoveImage();
      animatedSprite.StartAnimation();
      FrameNum = 0;
    }
    public void ResetMove(Direction dir)
    {
      StartMove(dir, true);
    }
    /// <summary>
    /// Stop moving sprite
    /// </summary>
    /// <param name="dir"></param>
    public void StopMove(Direction dir)
    {
      stopAfterMove = nextDir == null
        ? currDir == dir : nextDir == dir;
      if (stopAfterMove)
      {
        // Debug.WriteLine($"[{dir} UP]: STOP AFTER DONE");
      }
    }
    /// <summary>
    /// Movable object is an organism, it shall be able to die
    /// </summary>
    public virtual void Dead()
    {
      IsDead = true;
      // StopMoveNow();
      animatedSprite.SwitchImage("dead");
    }
    public virtual bool OverlapsWith(MovableMapObject obj)
    {
      return movablePosition.IntersectsWith(obj.movablePosition);
    }
    protected virtual BeforeNextMoveEventArgs CanMove(Direction dir)
    {
      IntPoint from = movablePosition;
      IntPoint to = movablePosition.PostTranslatePosition(dir);
      BeforeNextMoveEventArgs e = new(from, to);
      InvokeBeforeNextMove(e);
      return e;
    }

    public virtual void InvokeBeforeNextMove(BeforeNextMoveEventArgs e)
    {
      BeforeNextMove?.Invoke(this, e);
    }


    /// <summary>
    /// Check game state once a 1 unit walk is done
    /// <para>- If scheduled next walk, continue move</para>
    /// <para>- If stop after move, stop move</para>
    /// </summary>
    public void OnMoveEnded()
    {
      movablePosition.FinishMove();
      if (nextDir != null)
      {
        ContinueMove();
      }
      else if (stopAfterMove)
      {
        StopMoveNow();
      }
      else
      {
        ContinueMove();
      }
    }
    /// <summary>
    /// Continue to move
    /// <para>- Cond 1: if received next direction command during previous 1 unit walk</para>
    /// <para>- Cond 2: if current direction NOT key up yet</para>
    /// </summary>
    public void ContinueMove()
    {
      BeforeNextMoveEventArgs e = CanMove((Direction)(nextDir ?? currDir));
      if (e.Cancel)
      {
        nextDir = null;
        // if turn direction, then cancel is not move but not stop move
        if (e.TurnDirection == null)
        {
          Debug.WriteLine("[stop]: CONTINUE CANCELLED");
          StopMoveNow();
        }
      }
      else if (nextDir != null)
      {
        // Debug.WriteLine("[stop]: CONTINUE NEXT DIR");
        bool sameDir = currDir == nextDir;
        currDir = nextDir;
        if (!sameDir) SwitchMoveImage();
        nextDir = null;
      }
      else
      {
        // Debug.WriteLine("[stop]: CONTINUE TILL KEYUP");
      }
    }
    /// <summary>
    /// Stop immediately, only call after a walk is finished / force stopped
    /// </summary>
    public void StopMoveNow()
    {
      Debug.WriteLine("[stop]: STOP");
      animatedSprite.StopAnimation();
      currDir = null;
      nextDir = null;
    }
    /// <summary>
    /// Display image for the move direction
    /// </summary>
    public void SwitchMoveImage()
    {
      string directionName = Constants.DirectionName[(Direction)currDir];
      animatedSprite.SwitchImage(directionName);
    }

    // speed
    protected void SetSpeed(double speed)
    {
      this.speed = speed;
    }
    protected virtual void SpeedUp()
    {
      speed *= 1.2;
    }
    protected virtual void SpeedDown()
    {
      speed /= 1.2;
    }
    /// <summary>
    /// Remove from map, play dead animation before if havn't
    /// </summary>
    public override void Remove()
    {
      /*
      if (IsDead == false)
      {
        Dead();
        return;
      }
      */
      base.Remove();
    }
  }
}