using Bomberman.GameEngine.Enums;
using Bomberman.GameEngine.Graphics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Bomberman.GameEngine
{
  /// <summary>
  /// Control the game, interact with the UI
  /// </summary>
  public class GameController : INotifyPropertyChanged
  {
    private static GameController? instance;
    private Game game;
    private DispatcherTimer dispatcherTimer;
    public event PropertyChangedEventHandler? PropertyChanged;
    private GameState? state;
    private int secondsLeft = 180;
    /// <summary>
    /// Map of the game, load from text file
    /// </summary>
    private static readonly string map;
    // UI Level Fields
    public int SecondsLeft
    {
      get => secondsLeft; private set
      {
        if (value < 0)
        {
          OnGameEnded();
          return;
        }
        secondsLeft = value;
        InvokePropertyChanged("TimeLeft");
      }
    }
    // UI Level Properties
    public string TimeLeft
    {
      get
      {
        return Utilities.ToMMSS(SecondsLeft);
      }
    }
    private GameController()
    {
      game = Game.Instance;
    }
    // Singleton
    public static GameController Instance
    {
      get
      {
        if (instance == null) instance = new GameController();
        return instance;
      }
    }
    // Main Logics
    private void OnTick(object sender, EventArgs e)
    {
      SecondsLeft--;
    }
    public void InitGame()
    {
      // keydown event
      MainWindow.Instance.KeyDown += KeyDownHandler;
      MainWindow.Instance.KeyUp += KeyUpHandler;
      // timer
      dispatcherTimer = new DispatcherTimer();
      dispatcherTimer.Tick += OnTick;
      dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
      dispatcherTimer.Start();
      // general
      state = GameState.Started;
      game.InitGame();
    }
    public void OnGameEnded()
    {
      if (state == GameState.Ended) return; // If called already, return
      MainWindow.Instance.KeyDown -= KeyDownHandler;
      MainWindow.Instance.KeyUp -= KeyUpHandler;
      state = GameState.Ended;
      dispatcherTimer.Stop();
      Debug.WriteLine("Game ended");
    }
    // UX Binding
    public void KeyDownHandler(object sender, KeyEventArgs e)
    {
      // Debug.WriteLine("KeyDown" + e.Key);
      Direction? direction = Utilities.Key2Direction(e.Key);
      if (direction != null)
      {
        // Debug.WriteLine("[DOWN]" + e.Key);
        game.PlayerStartWalk((Direction)direction);
        return;
      }
      switch (e.Key)
      {
        case Key.Space:
          // plant bomb
          break;
      }
    }
    public void KeyUpHandler(object sender, KeyEventArgs e)
    {
      Direction? direction = Utilities.Key2Direction(e.Key);
      if (direction != null)
      {
        // Debug.WriteLine("[UP]" + e.Key);
        game.PlayerStopWalk((Direction)direction);
        return;
      }
      switch (e.Key)
      {
        case Key.Space:
          // plant bomb
          break;
      }
    }

    public void SetCanvas(Canvas cvs)
    {
      Renderer.Board = cvs;
    }
    private void InvokePropertyChanged(string name)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}
