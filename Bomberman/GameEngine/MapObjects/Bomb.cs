using System;

namespace Bomberman.GameEngine.MapObjects
{
  internal class Bomb : MapObject
  {
    private int framesToExplode = 60;
    public event EventHandler ExplodeEvent;

    internal Bomb(int x, int y, EventHandler ExplodeHandler) : base(x, y, "bomb", null, null) {
      ExplodeEvent += ExplodeHandler;
    }
    /// <summary>
    /// Frame update
    /// </summary>
    public override void Update()
    {
      framesToExplode--;
      if (framesToExplode == 0)
      {
        Explode();
      }
    }
    /// <summary>
    /// Handle explosion animation
    /// </summary>
    public void Explode()
    {
      // trigger event
      ExplodeEvent?.Invoke(this, new EventArgs());
      // set explosion animation
      Dispose();
    }
  } 
}
