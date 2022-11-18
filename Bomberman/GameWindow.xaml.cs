using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using Bomberman.GameEngine;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bomberman
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class GameWindow : Window
  {
    public GameWindow()
    {
      InitializeComponent();
      Debug.WriteLine("Test");
      // bind events
      gameWindow.KeyDown += GameController.Instance.KeyDownHandler;
    }
  }
}
