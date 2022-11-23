using Bomberman.GameEngine.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace Bomberman.GameEngine.Graphics
{
  public class AnimatedSprite : Sprite, IUpdatable
  {
    /// <summary>
    /// If movable, need update every frame, else only update image per some frames
    /// </summary>
    private readonly bool movable = false;
    private bool running = false;
    private int imageFrames = 2;
    private int? currFrameNum;
    private readonly MovableGridPosition movablePosition;
    /// <summary>
    /// key: variantName, value: frames of the variant
    /// </summary>
    private readonly Dictionary<string, List<BitmapImage>> animatedImages = new();
    public AnimatedSprite(
      string name,
      Dictionary<string, int?>? variant,
      string? defaultVariant,
      ref GridPosition position,
      int zIndex = 1,
      bool movable = false
      ) : base(name, variant, defaultVariant, ref position, zIndex)
    {
      this.movable = movable;
      // set default sprite
      animatedImages["default"] = animatedImages[defaultVariant ?? name];
      movablePosition = (MovableGridPosition)position;
    }
    /// <summary>
    /// Update the frame image every 4 frames, update position every frame if movable
    /// </summary>
    public void Update(int frameNum)
    {
      if (!running) return;
      if (!movable)
      {
        if (frameNum % 4 != 0) return;
        currFrameNum += (currFrameNum) % imageFrames;
        UpdateImage();
      }
      DrawUpdate();
    }
    /// <summary>
    /// Override update image to support animated image update
    /// </summary>
    protected override void UpdateImage()
    {
      if (currFrameNum == null) base.UpdateImage();
      else if (animatedImages.ContainsKey(currVariant))
      {
         imageBrush.ImageSource = animatedImages[currVariant][(int)currFrameNum];
      }
    }
    public override void SwitchImage(string variant)
    {
      // if static image
      if (images.ContainsKey(variant))
      {
        base.SwitchImage(variant);
        currFrameNum = null;
      }
      else if (animatedImages.ContainsKey(variant))
      {
        currVariant = variant;
        currFrameNum = 0;
        UpdateImage();
        DrawUpdate();
      }
    }
    public void StartAnimation()
    {
      running = true;
    }
    public void StopAnimation()
    {
      running = false;
    }
    /// <summary>
    /// Load both static & animated images
    /// </summary>
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
    private Uri GetAnimatedImageUri(string variantName, int frameNum)
    {
      return frameNum == 0 ?
        GetImageUri(variantName) :
        GetImageUriFromName($"{name}_{variantName}{frameNum}");
    }
  }
}
