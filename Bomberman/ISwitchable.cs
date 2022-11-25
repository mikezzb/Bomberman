namespace Bomberman
{
  public interface ISwitchable
  {
    /// <summary>
    /// Respond to switch out
    /// </summary>
    void OnSwitchOut();
    /// <summary>
    /// Respond to switch in
    /// </summary>
    void OnSwitchIn();
  }
}
