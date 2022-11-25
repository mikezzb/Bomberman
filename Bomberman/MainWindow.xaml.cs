using System.Windows;
using System.Windows.Controls;

namespace Bomberman
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public enum PageType
    {
      Home,
      Game,
    }
    private Page homePage;
    private Page gamePage;
    public Page HomePage
    {
      get
      {
        if (homePage == null)
        {
          homePage = new HomePage();
        }
        return homePage;
      }
    }
    public Page GamePage
    {
      get
      {
        if (gamePage == null)
        {
          gamePage = new GamePage();
        }
        return gamePage;
      }
    }
    public static MainWindow Instance;

    public MainWindow()
    {
      InitializeComponent();
      Instance = this;
      SwitchView(PageType.Home);
    }

    public void SwitchView(PageType type)
    {
      switch (type)
      {
        case PageType.Home:
          frameNavigation.Content = HomePage;
          break;
        case PageType.Game:
          frameNavigation.Content = GamePage;
          break;
      }
    }
  }
}
