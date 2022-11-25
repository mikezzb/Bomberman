using Bomberman.GameEngine.Enums;
using Bomberman.GameEngine.Graphics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;

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
    private readonly GameSoundPlayer sp;
    private int secondsLeft = Config.GameDuration;
    public int SecondsLeft
    {
      get => secondsLeft; private set
      {
        if (value < 0)
        {
          OnGameEnded(GameEndType.Timeout);
          return;
        }
        secondsLeft = value;
        GameStateText = $"TIME {SecondsLeft}";
        InvokePropertyChanged("TimeLeft");
      }
    }
    private bool GameEnded => state == GameState.Ended;
    // UI Level Properties
    private string gameBanner;
    public string GameStateText
    {
      get => gameBanner;
      private set
      {
        gameBanner = value;
        InvokePropertyChanged("GameStateText");
      }
    }
    public Visibility OverlayVisibility { get; private set; }
    public string OverlayText { get; private set; }
    private GameController()
    {
      OverlayVisibility = Visibility.Hidden;
      sp = GameSoundPlayer.Instance;
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
    public void StartStage(int stage = 1)
    {
      sp.PlaySound(GameSound.StageStart);
      InitGame();
      StartGame();
    }
    private void InitGame()
    {
      SecondsLeft = Config.GameDuration;
      game = new Game(OnGameEnded);
      // keydown event
      if (state == null)
      {
        MainWindow.Instance.KeyDown += KeyDownHandler;
        MainWindow.Instance.KeyUp += KeyUpHandler;
      }
      // timer
      dispatcherTimer = new DispatcherTimer();
      dispatcherTimer.Tick += OnTick;
      dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
      dispatcherTimer.Start();
      // general
      game.InitGame();
    }
    private void StartGame()
    {
      state = GameState.Started;
      HideOverlay();
      game.StartGame();
    }
    public void NewGame()
    {
      Debug.WriteLine("Restart game");
      Renderer.Clear();
      InitGame();
      StartGame();
    }
    public void OnGameEnded(GameEndType type)
    {
      if (GameEnded) return; // If called already, return
      state = GameState.Ended;
      dispatcherTimer.Stop();
      // draw banner
      string text = "";
      switch (type)
      {
        case GameEndType.Cleared:
          Debug.WriteLine("CLEARED");
          sp.PlaySound(GameSound.StageClear, false);
          text = "CLEARED - PRESS ENTER TO NEXT STAGE";
          break;
        case GameEndType.Dead:
          Debug.WriteLine("Dead");
          sp.PlaySound(GameSound.GameOver, false);
          text = "PLAYER DEAD - PRESS ENTER TO RESTART";
          break;
        case GameEndType.Timeout:
          Debug.WriteLine("Timeout");
          sp.PlaySound(GameSound.GameOver, false);
          text = "GAME OVER - PRESS ENTER TO RESTART";
          break;
      }
      GameStateText += $" - {text}";
      Debug.WriteLine("Game ended");
    }
    // UX Binding
    public void KeyDownHandler(object sender, KeyEventArgs e)
    {
      Debug.WriteLine($"Key down {GameEnded}");
      if (GameEnded)
      {
        switch (e.Key)
        {
          case Key.Enter:
            // restart game
            NewGame();
            break;
        }
        return;
      }
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
          game.PlayerPlaceBomb();
          break;
      }
    }
    public void KeyUpHandler(object sender, KeyEventArgs e)
    {
      if (GameEnded) return;
      Direction? direction = Utilities.Key2Direction(e.Key);
      if (direction != null)
      {
        // Debug.WriteLine("[UP]" + e.Key);
        game.PlayerStopWalk((Direction)direction);
        return;
      }
    }
    private void ShowOverlay(string text, int durationInMs = 2000)
    {
      Debug.WriteLine($"Show overlay {text}");
      OverlayVisibility = Visibility.Visible;
      OverlayText = text;
    }
    private void HideOverlay()
    {
      OverlayVisibility = Visibility.Hidden;
      OverlayText = "";
    }
    public void BindCanvas(Canvas cvs)
    {
      Renderer.Board = cvs;
    }
    private void InvokePropertyChanged(string name)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}
