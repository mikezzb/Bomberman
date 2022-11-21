using Bomberman.GameEngine.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace Bomberman.GameEngine.Graphics
{
  public class AnimatedSprite : Sprite
  {
    private  Direction? currDir;
    private Direction? nextDir;
    private bool stopAfterMove;
    private double walkSize;
    public bool Moving { get => currDir != null; }
    private int? currFrameNum;
    private readonly FrameTimer imageTimer;
    private readonly FrameTimer moveTimer;
    private readonly MovableGridPosition movablePosition;
    private readonly EventHandler<BeforeNextMoveEventArgs> onBeforeNextMove;
    /// <summary>
    /// key: variantName, value: frames of the variant
    /// </summary>
    private readonly Dictionary<string, List<BitmapImage>> animatedImages = new();
    public AnimatedSprite(
      string name,
      Dictionary<string, int?>? variant,
      string? defaultVariant,
      ref GridPosition position,
      EventHandler<BeforeNextMoveEventArgs> onBeforeNextMove
      ) : base(name, variant, defaultVariant, ref position)
    {
      this.onBeforeNextMove = onBeforeNextMove;
      movablePosition = (MovableGridPosition)position;
      imageTimer = new FrameTimer(100, 2, OnImageTick);
      moveTimer = new FrameTimer(Config.WalkFrameDuration, Config.WalkFrames, OnMoveTick, OnMoveEnded);
    }
    /// <summary>
    /// Move sprite until stop move
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="walkSize"></param>
    public void Move(Direction dir, double walkSize)
    {
      stopAfterMove = false;
      if (currDir != null)
      {
        Debug.WriteLine($"[NEXT MOVE]: {dir}");
        nextDir = dir;
        return;
      }
      Debug.WriteLine($"[MOVE]: {dir}");
      currDir = dir;
      SwitchMoveImage();
      StartAnimation();
      this.walkSize = walkSize;
    }
    public IntPoint PostMovePosition(Direction dir)
    {
      // if moving, need consider post move after current move
      if (Moving)
      {
        IntPoint afterCurrMovePos = movablePosition.PostMovePosition((Direction)currDir);
        return movablePosition.PostMovePosition(dir, afterCurrMovePos);
      }
      else
      {
        return movablePosition.PostMovePosition(dir);
      }
    }
    /// <summary>
    /// Stop moving sprite
    /// </summary>
    /// <param name="dir"></param>
    public void StopMove(Direction dir)
    {
      stopAfterMove = nextDir == null
        ? currDir == dir : nextDir == dir;
      if (stopAfterMove)
      {
        // Debug.WriteLine($"[{dir} UP]: STOP AFTER DONE");
      }
    }
    /// <summary>
    /// Called every frame to move image on canvas (for walking animation)
    /// </summary>
    /// <param name="frameNum"></param>
    private void OnMoveTick(int frameNum)
    {
      if (currDir == null) return;
      // Debug.WriteLine($"[{currDir}]-{frameNum}");
      // shift position & draw
      movablePosition.Move(currDir, walkSize);
      DrawUpdate();
    }
    /// <summary>
    /// Check game state once a 1 unit walk is done
    /// <para>- If scheduled next walk, continue move</para>
    /// <para>- If stop after move, stop move</para>
    /// </summary>
    public void OnMoveEnded()
    {
      movablePosition.FinishMove();
      if (nextDir != null)
      {
        ContinueMove();
      } else if (stopAfterMove)
      {
        StopMove();
      } else
      {
        ContinueMove();
      }
    }
    /// <summary>
    /// Continue to move
    /// <para>- Cond 1: if received next direction command during previous 1 unit walk</para>
    /// <para>- Cond 2: if current direction NOT key up yet</para>
    /// </summary>
    public void ContinueMove()
    {
      IntPoint from = movablePosition.GridPosition;
      IntPoint to = movablePosition.PostMovePosition((Direction)(nextDir ?? currDir));
      BeforeNextMoveEventArgs e = new(from, to);
      onBeforeNextMove(this, e);
      if (e.Cancel)
      {
        Debug.WriteLine("[stop]: CONTINUE CANCELLED");
        StopMove();
        nextDir = null;
      } else if (nextDir != null)
      {
        Debug.WriteLine("[stop]: CONTINUE NEXT DIR");
        bool sameDir = currDir == nextDir;
        currDir = nextDir;
        if (!sameDir) SwitchMoveImage();
        nextDir = null;
      } else
      {
        Debug.WriteLine("[stop]: CONTINUE TILL KEYUP");
      }
    }
    private void StopMove()
    {
      Debug.WriteLine("[stop]: STOP");
      currDir = null;
      StopAnimation();
    }
    /// <summary>
    /// Display image for the move direction
    /// </summary>
    private void SwitchMoveImage()
    {
      string directionName = Config.DirectionName[(Direction)currDir];
      SwitchImage(directionName);
    }
    /// <summary>
    /// Override update image to support animated image update
    /// </summary>
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
      imageTimer.Start();
      moveTimer.Start();
    }
    public void StopAnimation()
    {
      imageTimer.Stop();
      moveTimer.Stop();
    }
    private void OnImageTick(int frameNum)
    {
      if (currFrameNum != frameNum)
      {
        currFrameNum = frameNum;
        UpdateImage();
      }
      DrawUpdate();
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
    /// <summary>
    /// Draw image on canvas
    /// <para>As movable object shall display above background, it's default zIndex is 2</para>
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
  }
}
