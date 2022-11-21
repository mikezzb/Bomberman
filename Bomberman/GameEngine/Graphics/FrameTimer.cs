using System;
using System.Windows.Threading;

namespace Bomberman.GameEngine
{
  /// <summary>
  /// Frame based timer
  /// <para>Composite dispatch timer, and impl frame-based tick on top of it</para>
  /// </summary>
  public class FrameTimer
  {
    public int FrameCounter { get; private set; }
    private readonly int numFrames;
    private int frameDuration;
    private readonly DispatcherTimer timer;
    private readonly Action<int> tickCallback;
    private readonly Action? endCallback;
    public double Progress { get => (double)FrameCounter / numFrames; }
    public bool IsRunning { get; private set; }
    /// <summary>
    /// Create FrameTimer
    /// </summary>
    /// <param name="durationInMs"></param>
    /// <param name="numFrames"></param>
    /// <param name="onTick"></param>
    /// <param name="onEnd"></param>
    public FrameTimer(int durationInMs, int numFrames, Action<int> onTick, Action? onEnd = null)
    {
      // Debug.WriteLine("FrameTimer contructed");
      this.numFrames = numFrames;
      endCallback = onEnd;
      tickCallback = onTick;
      // timer
      timer = new DispatcherTimer();
      timer.Tick += onEnd == null ? OnTick : OnTickWithTimeout;
      SetFrameDuration(durationInMs);
    }
    public void Start()
    {
      FrameCounter = 0;
      IsRunning = true;
      timer.Start();
    }
    public void Stop()
    {
      // Debug.WriteLine("FrameTimer stopped");
      timer.Stop();
      IsRunning = false;
    }
    public void SetFrameDuration(int durationInMs)
    {
      frameDuration = durationInMs;
      timer.Interval = new TimeSpan(0, 0, 0, 0, frameDuration);
    }
    private void OnTick(object sender, EventArgs e)
    {
      // Debug.WriteLine(FrameCounter % numFrames);
      FrameCounter++;
      tickCallback(FrameCounter % numFrames);
    }
    private void OnTickWithTimeout(object sender, EventArgs e)
    {
      if (endCallback != null && FrameCounter == numFrames)
      {
        endCallback();
        FrameCounter = 0;
      }
      OnTick(sender, e);
    }
    public void ResetCycle()
    {
      if (!IsRunning) Start();
      else frameDuration = 0;
    }
  }
}
