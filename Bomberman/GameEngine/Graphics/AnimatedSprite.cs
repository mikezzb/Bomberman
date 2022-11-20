using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bomberman.GameEngine
{
  public class AnimatedSprite : Sprite
  {
    private int? currFrameNum;
    private Animator? animator;
    public Animator Animator
    {
      get
      {
        if (animator == null) animator = new Animator(100, 2, OnTick); return animator;
      }
    }
    /// <summary>
    /// key: variantName, value: frames of the variant
    /// </summary>
    private readonly Dictionary<string, List<BitmapImage>> animatedImages = new();
    public AnimatedSprite(
      string name,
      Dictionary<string, int?>? variant,
      string? defaultVariant,
      ref GridPosition position
      ) : base(name, variant, defaultVariant, ref position)
    {
    }
    protected override void UpdateImage()
    {
      if (currFrameNum == null) base.UpdateImage();
      else imageBrush.ImageSource = animatedImages[currVariant][(int)currFrameNum];
    }
    public override void SwitchImage(string variant)
    {
      // if static image
      if (images.ContainsKey(variant))
      {
        base.SwitchImage(variant);
        currFrameNum = null;
      }
      else
      {
        currVariant = variant;
        currFrameNum = 0;
        UpdateImage();
        DrawUpdate();
      }
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
      Debug.WriteLine("Sprite ticking");
      DrawUpdate();
    }
    protected override void LoadImages()
    {
      // Mount all images
      foreach (KeyValuePair<string, int?> pair in variant)
      {
        if (pair.Value == null)
        {
          images[pair.Key] = GetImage(GetImageUri(pair.Key));
        }
        else
        {
          List<BitmapImage> li = new List<BitmapImage>();
          for (int frameNum = 1; frameNum <= pair.Value; frameNum++)
          {
            li.Add(GetImage(GetAnimatedImageUri(pair.Key, frameNum)));
          }
          animatedImages[pair.Key] = li;
        }
      }
      DrawElement();
    }
    /// <summary>
    /// As movable object shall display above background, it's default zIndex is 2
    /// </summary>
    /// <param name="zIndex"></param>
    public override void DrawElement(int zIndex = 2)
    {
      base.DrawElement(zIndex);
    }
    private Uri GetAnimatedImageUri(string variantName, int frameNum)
    {
      return frameNum == 0 ?
        GetImageUri(variantName) :
        GetImageUriFromName($"{name}_{variantName}{frameNum}");
    }
    private static Uri GetImageUriFromName(string name)
    {
      return new Uri("Resources/" + name + Config.ImageExt, UriKind.Relative);
    }
  }
}
