using System.Windows;
using System.Windows.Controls;

namespace Bomberman
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public static MainWindow Instance;
    public MainWindow()
    {
      InitializeComponent();
      SwitchView(new HomePage());
      Instance = this;
    }

    public void SwitchView(Page content)
    {
      frameNavigation.Content = content;
    }
  }
}
