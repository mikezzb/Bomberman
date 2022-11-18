using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
 
namespace Bomberman.GameEngine
{
  public class Renderer
  {
    public static Renderer instance;
    private Canvas board;
    private Renderer() { }
    public static Renderer Instance
    {
      get
      {
        if (instance == null) instance = new Renderer();
        return instance;
      }
    }
    public void SetCanvas(Canvas canvas)
    {
      board = canvas;
    }
    public void DrawImage(Uri uri, double x, double y) {
      ImageBrush skin = new ImageBrush();
      skin.ImageSource = new BitmapImage(uri);
      DrawRectangle(skin, x, y);
    }
    public void DrawRectangle(Brush brush, double x, double y) {
      Rectangle rect = new Rectangle
      {
        Width = Config.ItemSize,
        Height = Config.ItemSize,
        Fill = brush,
      };
      DrawElement(rect, x, y);
    }
    /// <summary>
    /// Set x and y of a canvas item
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void DrawElement(UIElement el, double x, double y)
    {
      Canvas.SetTop(el, y);
      Canvas.SetLeft(el, x);
      board.Children.Add(el);
    }
  }
}
