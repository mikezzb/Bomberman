﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bomberman.GameEngine
{
  public class Renderer
  {
    private static Canvas? board;
    public static Canvas Board
    {
      private get
      {
        if (board == null)
        {
          throw new Exception("Missing rendering canvas");
        }
        return board;
      }
      set
      {
        board = value;
      }
    }
    private Renderer() { }
    public static void DrawImage(Uri uri, double x, double y)
    {
      ImageBrush skin = new ImageBrush();
      skin.ImageSource = new BitmapImage(uri);
      DrawRectangle(skin, x, y);
    }
    public static void DrawRectangle(Brush brush, double x, double y)
    {
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
    public static void DrawElement(UIElement el, double x, double y, int zIndex = 1)
    {
      PositionElement(el, x, y);
      Canvas.SetZIndex(el, zIndex);
      Board.Children.Add(el);
    }
    public static void PositionElement(UIElement el, double x, double y)
    {
      Canvas.SetTop(el, y);
      Canvas.SetLeft(el, x);
    }
    public static void RemoveElement(UIElement el)
    {
      Board.Children.Remove(el);
    }
  }
}