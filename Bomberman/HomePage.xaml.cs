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
    public HomePage()
    {
      InitializeComponent();
      // play home page BGM
      GameSoundPlayer sp = GameSoundPlayer.Instance;
      sp.PlaySound(GameSound.Title);
    }
  }
}
