using Bomberman.GameEngine;
using System.Windows.Controls;

namespace Bomberman
{
  /// <summary>
  /// Interaction logic for GamePage.xaml
  /// </summary>
  public partial class GamePage : Page
  {
    private readonly GameController gameController;
    public GamePage()
    {
      InitializeComponent();
      gameController = GameController.Instance;
      DataContext = gameController;
      gameController.BindCanvas(gameBoard);
      gameController.InitGame();
      gameController.StartGame();
    }
  }
}
