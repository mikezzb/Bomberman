using System.Windows;
using System.Windows.Controls;
using Bomberman.GameEngine;
using Bomberman.GameEngine.Enums;
namespace Bomberman
{
  /// <summary>
  /// Interaction logic for HomePage.xaml
  /// </summary>
  public partial class HomePage : Page
  {
    private readonly GameContext store;
    public HomePage()
    {
      store = GameContext.Instance;
      InitializeComponent();
      // play home page BGM
      GameSoundPlayer sp = GameSoundPlayer.Instance;
      sp.PlaySound(GameSound.Title);
      DataContext = store;
    }

    private void StageIncrBtn_Click(object sender, RoutedEventArgs e)
    {
      store.SetStageNum(store.StageNum + 1);
    }

    private void StageDescBtn_Click(object sender, RoutedEventArgs e)
    {
      store.SetStageNum(store.StageNum - 1);
    }
  }
}
