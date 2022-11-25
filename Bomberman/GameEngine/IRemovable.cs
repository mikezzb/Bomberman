namespace Bomberman.GameEngine
{
  /// <summary>
  /// Interface for any class that can be removed from the renderer
  /// </summary>
  public interface IRemovable
  {
    /// <summary>
    /// Remove itself from renderer
    /// </summary>
    void Remove();
  }
}
