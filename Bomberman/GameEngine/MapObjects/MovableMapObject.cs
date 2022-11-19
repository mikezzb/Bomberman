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
    private Direction? currDirection;
    protected MovableMapObject(int x, int y, string name, Dictionary<string, int>? variant, string? defaultVariant, double speed = 1.0) : base(x, y, name, variant, defaultVariant)
    {
      this.speed = speed;
    }
    private double WalkTime { get { return Config.UnitWalkTime * speed } }
    /// <summary>
    /// Each walk is 1 unit only
    /// </summary>
    public void StartWalk(Direction dir)
    {
      if (dir == currDirection) return;
      currDirection = dir;
      string directionName = Config.DirectionName[dir];
      sprite.SwitchImage(directionName);
      sprite.StartAnimation();
    }
    /// <summary>
    /// Stop Walk
    /// </summary>
    public void StopWalk() {
      Debug.WriteLine("Stopped walk");
      sprite.StopAnimation();
      currDirection = null;
    }
    public void SetSpeed(double speed)
    {
      this.speed = speed;
    }
    public void SpeedUp()
    {
      speed += 0.2;
    }
    public void SpeedDown()
    {
      speed -= 0.2;
    }
  }
}
