﻿using Bomberman.GameEngine.Enums;
using Bomberman.GameEngine.Graphics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Bomberman.GameEngine
{
  /// <summary>
  /// Control the game, interact with the UI
  /// </summary>
  public class GameController : INotifyPropertyChanged, IRemovable, ISwitchable
  {
    private MainWindow win;
    private static GameController? instance;
    private Game game;
    private DispatcherTimer dispatcherTimer;
    public event PropertyChangedEventHandler? PropertyChanged;
    private GameState? state;
    private GameContext store;
    private readonly GameSoundPlayer sp;
    private int secondsLeft = Config.GameDuration;
    public int SecondsLeft
    {
      get => secondsLeft; private set
      {
        if (value < 0)
        {
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
      store = GameContext.Instance;
      win = MainWindow.Instance;
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
    public void StartStage()
    {
      Debug.WriteLine($"Start Stage #{store.StageNum}");
      sp.PlaySound(GameSound.StageStart);
      InitGame();
      StartGame();
    }
    /// <summary>
    /// Game factory
    /// </summary>
    /// <param name="stageNum"></param>
    /// <returns></returns>
    private Game CreateGame(int stageNum)
    {
      if (stageNum == 0) return new Game(OnGameEnded);
      return new StageGame(stageNum, OnGameEnded);
    }
    private void InitGame()
    {
      SecondsLeft = Config.GameDuration;
      game = CreateGame(store.StageNum);
      // timer
      dispatcherTimer = new DispatcherTimer();
      dispatcherTimer.Tick += OnTick;
      dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
      // general
      game.InitGame();
    }
    private void StartGame()
    {
      state = GameState.Started;
      game.StartGame();
      dispatcherTimer.Start();
    }
    public void NewGame()
    {
      Debug.WriteLine("Restart new game");
      game.Remove();
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
          store.SetStageNum(store.StageNum + 1);
          if (store.GameCleared)
          {
            store.SetStageNum(0);
            text = "CLEARED ALL STAGES - PRESS ENTER TO RESTART";
          }
          else
          {
            text = "CLEARED - PRESS ENTER TO NEXT STAGE";
          }
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
      Debug.WriteLine($"Key down {GameEnded} | {e.Key}");
      if (GameEnded)
      {
        switch (e.Key)
        {
          case Key.Enter:
            // restart game
            NewGame();
            break;
          case Key.Escape:
            // back to home page
            ReturnToHome();
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

    public void ReturnToHome()
    {
      win.SwitchView(this, MainWindow.PageType.Home);
    }
    /// <summary>
    /// Fake remove, but cleanup
    /// </summary>
    public void Remove()
    {
      state = null;
      secondsLeft = Config.GameDuration;
      game.Remove();
    }
    public void OnSwitchIn()
    {
      if (state != GameState.Started)
      {
        StartStage();
      }
      win.KeyDown += KeyDownHandler;
      win.KeyUp += KeyUpHandler;
    }
    public void OnSwitchOut()
    {
      Remove();
      win.KeyDown -= KeyDownHandler;
      win.KeyUp -= KeyUpHandler;
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
