using Bomberman.GameEngine.Enums;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using Bomberman.GameEngine.Graphics;

namespace Bomberman.GameEngine.MapObjects
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
    private readonly AnimatedSprite animatedSprite;
    private readonly MovableGridPosition movablePosition;
    public event EventHandler<BeforeNextMoveEventArgs> BeforeNextMove;

    public MovableGridPosition MovablePosition { get => movablePosition; }
    protected MovableMapObject(int x, int y, string name, Dictionary<string, int?>? variant, string? defaultVariant, double speed = 1.0) : base()
    {
      this.speed = speed;
      position = new MovableGridPosition(x, y);
      sprite = new AnimatedSprite(name, variant, defaultVariant, ref position, OnBeforeNextMove);
      animatedSprite = (AnimatedSprite)sprite;
      movablePosition = (MovableGridPosition)position;
      Debug.WriteLine($"Speed {this.speed} | {NormalizedWalkSize}");
    }
    private double NormalizedWalkSize { get => (speed / Config.WalkFrames); }
    /// <summary>
    /// Each walk is 1 unit only
    /// </summary>
    public void Move(Direction dir)
    {
      animatedSprite.Move(dir, NormalizedWalkSize);
    }
    public void StopMove(Direction dir)
    {
      animatedSprite.StopMove(dir);
    }
    public IntPoint PostMovePosition(Direction dir)
    {
      return animatedSprite.PostMovePosition(dir);
    }
    public void OnBeforeNextMove(object sender, BeforeNextMoveEventArgs e)
    {
      BeforeNextMove?.Invoke(this, e);
    }
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
