using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bomberman.GameEngine
{
  public class Sprite : IDisposable
  {
    private readonly string name;
    private readonly Dictionary<string, int> variant;
    private string currVariant;
    private GridPosition position;
    private int? currFrameNum;
    private ImageBrush imageBrush = new ImageBrush();
    private UIElement canvasImage;
    private Animator? animator;
    public Animator Animator
    {
      get
      {
        if (animator == null) animator = new Animator(100, 2, OnTick); return animator;
      }
    }
    /// <summary>
    /// key: variantName
    /// </summary>
    private readonly Dictionary<string, BitmapImage> images = new();
    /// <summary>
    /// key: variantName, value: frames of the variant
    /// </summary>
    private readonly Dictionary<string, List<BitmapImage>> animatedImages = new();
    /// <summary>
    /// Detail: key is variant name of image, value is how many animated frames it has (1 if no animation)
    /// - At least the detail shall include "default" 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="variant"></param>
    public Sprite(
      string name,
      Dictionary<string, int> variant,
      GridPosition position
      )
    {
      this.name = name;
      this.variant = variant;
      this.position = position;
      canvasImage = new Rectangle()
      {
        Width = Config.ItemSize,
        Height = Config.ItemSize,
        Fill = imageBrush
      };
      InitImages();
    }
    private void UpdateImage()
    {

      if (currFrameNum == null) imageBrush.ImageSource = images[currVariant];
      else imageBrush.ImageSource = animatedImages[currVariant][(int)currFrameNum];
    }
    public void SwitchImage(string variant)
    {
      if (images.ContainsKey(variant))
      {
        currVariant = variant;
        currFrameNum = null;
      }
      else
      {
        currVariant = variant;
        currFrameNum = 0;
      }
      UpdateImage();
      DrawUpdate();
    }
    public void StartAnimation()
    {
      Animator.Start();
    }
    public void StopAnimation()
    {
      Animator.Stop();
    }
    private void OnTick(int frameNum)
    {
      if (currFrameNum != frameNum)
      {
        currFrameNum = frameNum;
        UpdateImage();
      }
      DrawUpdate();
    }
    private void InitImages()
    {
      // Mount all variants
      foreach (KeyValuePair<string, int> pair in variant)
      {
        if (pair.Value == 1)
        {
          images[pair.Key] = GetImage(GetImageName(pair.Key));
        }
        else
        {
          List<BitmapImage> li = new List<BitmapImage>();
          for (int frameNum = 1; frameNum <= pair.Value; frameNum++)
          {
            li.Add(GetImage(GetAnimatedImageName(pair.Key, frameNum)));
          }
          animatedImages[pair.Key] = li;
        }
      }
      // Set default image
      currVariant = "default";
      imageBrush.ImageSource = images[currVariant];
      Draw();
    }
    public void Draw()
    {
      Renderer.DrawElement(canvasImage, position.CanvasX, position.CanvasY);
    }
    public void DrawUpdate()
    {
      Renderer.MoveElement(canvasImage, position.CanvasX, position.CanvasY);
    }
    private static BitmapImage GetImage(string uri)
    {
      return new BitmapImage(new Uri(uri));
    }
    private string GetImageName(string variantName)
    {
      return $"{name}_{variantName}";
    }
    private string GetAnimatedImageName(string variantName, int frameNum)
    {
      return frameNum == 0 ? GetImageName(variantName) : $"{GetImageName(variantName)}{frameNum}";
    }

    public void Dispose()
    {
      Renderer.RemoveElement(canvasImage);
    }
  }
}
