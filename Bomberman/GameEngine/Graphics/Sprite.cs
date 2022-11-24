using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bomberman.GameEngine.Graphics
{
  public class Sprite : IDisposable
  {
    protected readonly string name;
    protected readonly Dictionary<string, int?> variant;
    protected readonly string defaultName;
    protected string currVariant;
    protected GridPosition position;
    protected ImageBrush imageBrush = new ImageBrush();
    protected UIElement canvasImage;
    protected int zIndex;
    /// <summary>
    /// key: variantName
    /// </summary>
    protected readonly Dictionary<string, BitmapImage> images = new();
    public Sprite(
      string name,
      Dictionary<string, int?>? variant,
      string? defaultVariant,
      GridPosition position,
      int zIndex = 1
      )
    {
      this.zIndex = zIndex;
      this.name = name;
      this.variant = variant ?? new Dictionary<string, int?>() { { "default", null } };
      this.position = position;
      defaultName = defaultVariant == null ? name : $"{name}_{defaultVariant}";
      canvasImage = new Rectangle()
      {
        Width = Config.ItemSize,
        Height = Config.ItemSize,
        Fill = imageBrush
      };
      LoadImages(defaultVariant);
    }
    protected virtual void UpdateImage()
    {
      imageBrush.ImageSource = images[currVariant];
    }
    public virtual void SwitchImage(string variant)
    {
      currVariant = variant;
      UpdateImage();
      DrawUpdate();
    }
    protected virtual void LoadImages(string defaultVariant)
    {
      // set default sprite
      images["default"] = GetImage(GetImageUri("default"));
      currVariant = "default";
      imageBrush.ImageSource = images[currVariant];
      // Mount all variant images
      foreach (KeyValuePair<string, int?> pair in variant)
      {
        images[pair.Key] = GetImage(GetImageUri(pair.Key));
      }
      DrawElement();
    }
    public virtual void DrawElement()
    {
      Renderer.DrawElement(canvasImage, position.CanvasX, position.CanvasY, this.zIndex);
    }
    public void DrawUpdate()
    {
      // Debug.WriteLine($"Update x: {position.CanvasX} y: {position.CanvasY}");
      Renderer.PositionElement(canvasImage, position.CanvasX, position.CanvasY);
    }
    protected static BitmapImage GetImage(Uri uri)
    {
      return new BitmapImage(uri);
    }
    protected Uri GetImageUri(string variantName)
    {
      return GetImageUriFromName(variantName == "default" ? defaultName : $"{name}_{variantName}");
    }
    protected static Uri GetImageUriFromName(string name)
    {
      return new Uri("Resources/" + name + Config.ImageExt, UriKind.Relative);
    }

    public virtual void Dispose()
    {
      Renderer.RemoveElement(canvasImage);
      images.Clear();
    }
  }
}
