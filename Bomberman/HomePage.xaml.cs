using Bomberman.GameEngine;
using Bomberman.GameEngine.Enums;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Bomberman
{
  /// <summary>
  /// Interaction logic for HomePage.xaml
  /// </summary>
  public partial class HomePage : Page
  {
    private MainWindow win;
    private readonly GameContext store;
    public HomePage()
    {
      store = GameContext.Instance;
      InitializeComponent();
      // play home page BGM
      GameSoundPlayer sp = GameSoundPlayer.Instance;
      sp.PlaySound(GameSound.Title);
      DataContext = store;
      win = MainWindow.Instance;
      win.KeyDown += KeyDownHandler;
    }

    // UX Binding
    public void KeyDownHandler(object sender, KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Enter:
          Debug.WriteLine("Home page Enter");
          StartGame();
          break;
        case Key.Up:
          store.SetStageNum(store.StageNum + 1);
          break;
        case Key.Down:
          store.SetStageNum(store.StageNum - 1);
          break;
      }
    }

    public void StartGame()
    {
      win.KeyDown -= KeyDownHandler;
      win.SwitchView(MainWindow.PageType.Game);
    }

    private void StageIncrBtn_Click(object sender, RoutedEventArgs e)
    {
      store.SetStageNum(store.StageNum + 1);
    }

    private void StageDescBtn_Click(object sender, RoutedEventArgs e)
    {
      store.SetStageNum(store.StageNum - 1);
    }

    private void StartBtn_Click(object sender, RoutedEventArgs e)
    {
      StartGame();
    }
  }
}
