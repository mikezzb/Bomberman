namespace Bomberman.GameEngine
{
  /// <summary>
  /// Interface for any object requires to be updated for each frame
  /// </summary>
  internal interface IUpdatable
  {
    /// <summary>
    /// Each updatable required to have own frame num and responsible to sync itself
    /// </summary>
    int FrameNum { get; }
    /// <summary>
    /// Called each frame
    /// </summary>
    void Update();
  }
}
