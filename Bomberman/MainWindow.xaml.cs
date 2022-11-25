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
      SwitchView(null, PageType.Home);
    }

    public void SwitchView(ISwitchable? sender, PageType type)
    {
      if (sender != null)
      {
        sender.OnSwitchOut();
      }
      switch (type)
      {
        case PageType.Home:
          frameNavigation.Content = HomePage;
          ((ISwitchable)HomePage).OnSwitchIn();
          break;
        case PageType.Game:
          frameNavigation.Content = GamePage;
          ((ISwitchable)GamePage).OnSwitchIn();
          break;
      }
    }
  }
}
