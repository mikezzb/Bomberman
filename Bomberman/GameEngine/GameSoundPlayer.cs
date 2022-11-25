using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using Bomberman.GameEngine.Enums;

namespace Bomberman.GameEngine
{
  /// <summary>
  /// Singleton sound player class to play game sound
  /// </summary>
  public class GameSoundPlayer
  {
    private static GameSoundPlayer? instance;
    private readonly MediaPlayer mediaPlayer; // prefer composition
    public static readonly Dictionary<GameSound, string> GameSoundName = new()
    {
      { GameSound.Title, "01 Title Screen" },
      { GameSound.StageStart, "02 Stage Start" },
      { GameSound.StageClear, "05 Stage Clear" },
      { GameSound.Bgm, "03 Main BGM" },
      { GameSound.GetPowerup, "04 Power-Up Get" },
      { GameSound.GetKey, "07 Special Power-Up Get" },
      { GameSound.GameOver, "10 Game Over" }
    };
    private GameSoundPlayer()
    {
      mediaPlayer = new MediaPlayer();
    }
    public GameSound? CurrSound { get; private set; }
    // Singleton
    public static GameSoundPlayer Instance
    {
      get
      {
        if (instance == null) instance = new GameSoundPlayer();
        return instance;
      }
    }
    public void PlaySound(GameSound type)
    {
      mediaPlayer.Stop();
      mediaPlayer.Open(GetUriFromSoundType(type));
      CurrSound = type;
      mediaPlayer.Play();
    }
    /// <summary>
    /// Check if stopping the current sound, if yes then stop it
    /// </summary>
    /// <param name="type"></param>
    public void StopPlay(GameSound type)
    {
      if (CurrSound == type) StopPlay();
    }
    private void StopPlay()
    {
      CurrSound = null;
      mediaPlayer.Stop();
    }
    public static Uri GetUriFromSoundType(GameSound type)
    {
      return new Uri("Resources/sounds/" + GameSoundName[type] + Config.SoundExt, UriKind.Relative);
    }
  }
}
