using Bomberman.GameEngine.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace Bomberman.GameEngine
{
  public class AnimatedSprite : Sprite
  {
    private Direction? currDir;
    private Direction? nextDir;
    private bool stopAfterMove;
    private double walkSize;
    public bool Moving { get; private set; }
    private int? currFrameNum;
    private Animator imageAnimator;
    private Animator moveAnimator;
    private readonly MovableGridPosition movablePosition;
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
      movablePosition = (MovableGridPosition)position;
      imageAnimator = new Animator(100, 2, OnTick);
      moveAnimator = new Animator(Config.WalkFrameDuration, Config.WalkFrames, OnMoveTick, OnMoveEnded);
    }
    public void Move(Direction dir, double walkSize)
    {
      if (currDir != null)
      {
        Debug.WriteLine($"[{dir} DONE]: RECORD NEXT MOVE");
        stopAfterMove = false;
        nextDir = dir;
        return;
      }
      currDir = dir;
      stopAfterMove = false;
      SwitchMoveImage();
      StartAnimation();
      this.walkSize = walkSize;
    }
    public void StopMove(Direction dir)
    {
      if (currDir == dir)
      {
        Debug.WriteLine($"[{dir} UP]: STOP AFTER DONE");
        stopAfterMove = true;
      }
    }
    private void OnMoveTick(int frameNum)
    {
      if (currDir == null) return;
      Debug.WriteLine($"[{currDir}]-{frameNum}");
      // shift position & draw
      movablePosition.Move(currDir, walkSize);
      DrawUpdate();
    }
    public void OnMoveEnded()
    {
      if (nextDir != null)
      {
        Debug.WriteLine("[stop]: CONTINUE");
        ContinueMove();
        return;
      }
      if (stopAfterMove)
      {
        Debug.WriteLine("[stop]: STOP");
        currDir = null;
        StopAnimation();
        return;
      }
    }
    public void ContinueMove()
    {
      bool sameDir = currDir == nextDir;
      currDir = nextDir;
      nextDir = null;
      if (!sameDir) SwitchMoveImage();
    }
    private void SwitchMoveImage()
    {
      string directionName = Config.DirectionName[(Direction)currDir];
      SwitchImage(directionName);
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
      imageAnimator.Start();
      moveAnimator.Start();
    }
    public void StopAnimation()
    {
      imageAnimator.Stop();
      moveAnimator.Stop();
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
    /*
    public void Move(Direction dir, EventHandler? callback)
    {
      if (Moving) return;
      Moving = true;
      nextDir = dir;
      Debug.WriteLine("Start move");
      Point postCanvasPos = ((MovableGridPosition)position).SetPostMovePosition(dir);
      Debug.WriteLine(postCanvasPos);

      ThicknessAnimation ani = new ThicknessAnimation();
      ani.From = new Thickness(position.CanvasX, position.CanvasY, 0, 0);
      ani.To = new Thickness(postCanvasPos.X, postCanvasPos.Y, 0, 0);
      ani.Duration = TimeSpan.FromMilliseconds(200);
      ani.FillBehavior = FillBehavior.HoldEnd;
      ani.Completed += OnMoveCompleted;
      ani.Completed += callback;
      Debug.WriteLine(ani.From);
      // ani.Completed += callback;
      canvasImage.BeginAnimation(FrameworkElement.MarginProperty, ani);
    }
    */
    private void OnMoveCompleted(object sender, EventArgs e)
    {
      Debug.WriteLine("stopped move");
      ((MovableGridPosition)position).Move(nextDir, 1);
      Moving = false;
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
