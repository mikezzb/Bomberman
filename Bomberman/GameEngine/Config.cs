namespace Bomberman.GameEngine
{
  public static class Config
  {
    public static readonly int ItemSize = 40;
    public static readonly int Height = 13;
    public static readonly int Width = 31;
    public static readonly string ImageExt = ".png";
    public static readonly string Map = Resources.map;
  };
  namespace Enums
  {
    public enum GameObject
    {
      Wall = '#',
      Floor = ' ',
    }
    public enum GameState
    {
      Started,
      Paused,
      Ended,
    }
  }
}
