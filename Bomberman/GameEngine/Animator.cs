using System;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.GameEngine
{
  public class Animator
  {
    private int frameCounter = 0;
    private int numFrames;
    private DispatcherTimer timer;
    private Action<int> tickCallback;
    /// <summary>
    /// Create Animator
    /// </summary>
    /// <param name="frameDuration">In milliseconds</param>
    /// <param name="numFrames"></param>
    /// <param name="onTick"></param>
    public Animator(int frameDuration, int numFrames, Action<int> onTick)
    {
      this.numFrames = numFrames;
      tickCallback = onTick;
      // timer
      timer = new DispatcherTimer();
      timer.Tick += new EventHandler(OnTick);
      timer.Interval = new TimeSpan(0, 0, 0, frameDuration);
      Start();
    }
    public void Stop()
    {
      timer.Stop();
    }
    public void Start()
    {
      timer.Start();
    }
    private void OnTick(object sender, EventArgs e)
    {
      frameCounter++;
      tickCallback(frameCounter % numFrames);
    }
  }
}
