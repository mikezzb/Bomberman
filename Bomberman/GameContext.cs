using Bomberman.GameEngine;
using System.ComponentModel;
using System.Diagnostics;

namespace Bomberman
{
  /// <summary>
  /// Runtime configs / data for the game, also serve as ViewModel
  /// </summary>
  public class GameContext : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;
    public int StageNum { get; private set; }
    private static GameContext? instance;
    // Singleton
    public static GameContext Instance
    {
      get
      {
        if (instance == null) instance = new GameContext();
        return instance;
      }
    }
    public bool GameCleared => StageNum == Config.NumStages;
    public string StageText => $"STAGE {StageNum}";
    private GameContext() { }
    public void SetStageNum(int stageNum)
    {
      Debug.WriteLine($"Setting stage num to {stageNum}");
      if (stageNum < 0 || stageNum > Config.NumStages) return;
      StageNum = stageNum;
      InvokePropertyChanged("StageText");
    }
    private void InvokePropertyChanged(string name)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}
