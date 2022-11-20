using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bomberman.GameEngine.Enums;

namespace Bomberman.GameEngine
{
  /// <summary>
  /// Abstract class of movable map object in game, including everything visible on the map
  /// <list type="bullet">
  /// <item>Position</item>
  /// <item>Display (Image)</item>
  /// </list>
  /// </summary>
  public abstract class MovableMapObject : MapObject
  {
    private double speed;
    private double Speed
    {
      get => speed;
      set
      {
        speed = value;
        Debug.WriteLine($"Setting speed to {speed}");
        moveAnimator.SetFrameDuration(WalkFrameDuration);
      }
    }
    private Direction? currDirection;
    private Animator moveAnimator;
    private bool stopAfterWalk = false;
    private readonly AnimatedSprite animatedSprite;
    private readonly MovableGridPosition movablePosition;
    protected MovableMapObject(int x, int y, string name, Dictionary<string, int?>? variant, string? defaultVariant, double speed = 1.0) : base()
    {
      position = new MovableGridPosition(x, y);
      sprite = new AnimatedSprite(name, variant, defaultVariant, ref position);
      animatedSprite = (AnimatedSprite)sprite;
      movablePosition = (MovableGridPosition)position;
      moveAnimator = new Animator(WalkFrameDuration, Config.WalkFrames, OnAnimationTick);
      Speed = speed;
    }
    private int WalkFrameDuration { get => (int)(Config.WalkFrameDuration * speed); }
    private double WalkFrameLength { get => 1.0 / Config.WalkFrames; }
    /// <summary>
    /// Each walk is 1 unit only
    /// </summary>
    public void StartWalk(Direction dir)
    {
      if (currDirection != null) return;
      stopAfterWalk = false;
      currDirection = dir;
      string directionName = Config.DirectionName[dir];
      animatedSprite.SwitchImage(directionName);
      animatedSprite.StartAnimation();
      moveAnimator.Start();
    }
    /// <summary>
    /// Stop Walk (but still will finish current walk)
    /// </summary>
    public void StopWalk() {
      Debug.WriteLine("Stopped walk scheduled");
      stopAfterWalk = true;
    }
    public void SetSpeed(double speed)
    {
      Speed = speed;
    }
    public void SpeedUp()
    {
      Speed += 0.2;
    }
    public void SpeedDown()
    {
      Speed -= 0.2;
    }
    /// <summary>
    /// Draw displacement as walking
    /// </summary>
    private void OnAnimationTick(int frameNum)
    {
      if (currDirection == null) return;
      bool stopAnimation = (stopAfterWalk && frameNum == 0);
      // shift position & draw
      movablePosition.Move(currDirection, WalkFrameLength);
      animatedSprite.DrawUpdate();
      if (stopAnimation) StopWalkAnimation();
    }
    private void StopWalkAnimation()
    {
      Debug.WriteLine("Stop Walk Animation");
      animatedSprite.StopAnimation();
      movablePosition.ResetOffset();
      moveAnimator.Stop();
      currDirection = null;
      stopAfterWalk = false;
    }
  }
}
