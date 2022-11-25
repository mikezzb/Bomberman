using Bomberman.GameEngine.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;

namespace Bomberman.GameEngine
{
  /// <summary>
  /// Singleton sound player class to play game sound
  /// </summary>
  public class GameSoundPlayer
  {
    private static GameSoundPlayer? instance;
    private readonly MediaPlayer mediaPlayer; // prefer composition
    private bool replay = true;
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
      mediaPlayer.MediaEnded += MediaEndedHandler;
    }
    public GameSound? CurrSound { get; private set; }
    /// <summary>
    /// To resume playback after interrupt (e.g. bgm -> powerup -> bgm)
    /// </summary>
    public GameSound? NextSound { get; private set; }
    // Singleton
    public static GameSoundPlayer Instance
    {
      get
      {
        if (instance == null) instance = new GameSoundPlayer();
        return instance;
      }
    }
    public void PlaySound(GameSound type, bool replay = true)
    {
      this.replay = replay;
      mediaPlayer.Stop();
      mediaPlayer.Open(GetUriFromSoundType(type));
      CurrSound = type;
      mediaPlayer.Play();
    }
    public void PlayAfterEnded(GameSound type)
    {
      if (CurrSound != null)
      {
        NextSound = type;
      }
      else
      {
        PlaySound(type);
      }
    }
    /// <summary>
    /// Play a sound now, and resume previous sound after played
    /// </summary>
    /// <param name="type"></param>
    public void PlayInterruptSound(GameSound type)
    {
      NextSound = CurrSound;
      Debug.WriteLine($"Set next sound to {NextSound}");
      PlaySound(type);
    }
    private void MediaEndedHandler(object sender, EventArgs e)
    {
      Debug.WriteLine($"Media ended: next to play {NextSound ?? CurrSound}");
      if (NextSound != null)
      {
        Debug.WriteLine("Play next sound");
        PlaySound((GameSound)NextSound);
        NextSound = null;
      }
      else if (replay)
      {
        // replay current sound
        mediaPlayer.Position = TimeSpan.Zero;
        mediaPlayer.Play();
      }
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
