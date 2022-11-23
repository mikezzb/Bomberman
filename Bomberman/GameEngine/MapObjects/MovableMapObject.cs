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
  internal abstract class MovableMapObject : MapObject
  {
    private double speed;
    protected readonly AnimatedSprite animatedSprite;
    protected readonly MovableGridPosition movablePosition;
    private Direction? currDir;
    private Direction? nextDir;
    private bool stopAfterMove;
    public bool Moving { get => currDir != null; }
    public Direction? CurrDir { get => currDir; }
    public event EventHandler<BeforeNextMoveEventArgs> BeforeNextMove;

    public MovableGridPosition MovablePosition { get => movablePosition; }
    protected MovableMapObject(int x, int y, string name, Dictionary<string, int?>? variant, string? defaultVariant, double speed = 1.0, int zIndex = 2) : base()
    {
      this.speed = speed;
      position = new MovableGridPosition(x, y);
      sprite = new AnimatedSprite(name, variant, defaultVariant, ref position, zIndex, true);
      animatedSprite = (AnimatedSprite)sprite;
      movablePosition = (MovableGridPosition)position;
      Debug.WriteLine($"Speed {this.speed} | {WalkSize}");
    }
    private double WalkSize { get => (speed / Config.WalkFrames); }
    public IntPoint PostMovePosition(Direction dir)
    {
      return movablePosition.PostMovePosition(dir);
    }
    /// <summary>
    /// Move image on canvas (for walking animation)
    /// </summary>
    /// <param name="frameNum"></param>
    public override void Update(int frameNum)
    {
      // nothing to update if not moving
      if (currDir == null) return;
      // shift position & draw
      movablePosition.Move(currDir, WalkSize);
      animatedSprite.Update(frameNum);
      if (frameNum % Config.WalkFrames == 0) OnMoveEnded();
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
    public void StopMoveNow()
    {
      animatedSprite.StopAnimation();
      currDir = null;
      nextDir = null;
    }
    public virtual void Dead()
    {
      StopMoveNow();
      animatedSprite.SwitchImage("dead");
    }
    public virtual bool OverlapsWith(MovableMapObject obj)
    {
      return movablePosition.IntersectsWith(obj.movablePosition);
    }
    private BeforeNextMoveEventArgs CanMove(Direction dir)
    {
      IntPoint from = movablePosition.Position;
      IntPoint to = movablePosition.PostMovePosition(dir);
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
        StopMove();
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
        if (e.TurnDirection == null)
        {
          Debug.WriteLine("[stop]: CONTINUE CANCELLED");
          StopMove();
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
    /// Stop immediately, only call after a walk is finished
    /// </summary>
    private void StopMove()
    {
      Debug.WriteLine("[stop]: STOP");
      currDir = null;
      animatedSprite.StopAnimation();
    }
    /// <summary>
    /// Display image for the move direction
    /// </summary>
    public void SwitchMoveImage()
    {
      string directionName = Config.DirectionName[(Direction)currDir];
      animatedSprite.SwitchImage(directionName);
    }

    // speed
    protected void SetSpeed(double speed)
    {
      this.speed = speed;
    }
    protected void SpeedUp()
    {
      speed *= 1.2;
    }
    protected void SpeedDown()
    {
      speed /= 1.2;
    }
    public override void Dispose()
    {
      base.Dispose();
    }
  }
}
