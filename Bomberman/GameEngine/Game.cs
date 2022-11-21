using Bomberman.GameEngine.Enums;
using Bomberman.GameEngine.MapObjects;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Bomberman.GameEngine
{
  /// <summary>
  /// Controller of the game
  /// - GameLoop
  /// - GameOver
  /// - GameSetup
  /// 
  /// </summary>
  public class Game
  {
    private readonly List<MapObject> map = new();
    private Player player;
    private static Game? instance;
    public static Game Instance
    {
      get
      {
        if (instance == null) instance = new Game();
        return instance;
      }
    }
    private Game() { }
    public void InitGame()
    {
      player = new Player();
      InitMap();
    }
    public void InitMap()
    {
      using (StringReader reader = new StringReader(Config.Map))
      {
        string line;
        int i = 0, j = 0;
        while ((line = reader.ReadLine()) != null)
        {
          Debug.WriteLine(line);
          foreach (char c in line)
          {
            switch (c)
            {
              case (char)GameObject.Wall:
                Debug.WriteLine("Wall");
                Wall wall = new Wall(j, i);
                map.Add(wall);
                break;
              case (char)GameObject.Floor:
                Debug.WriteLine("Floor");
                Floor floor = new Floor(j, i);
                map.Add(floor);
                break;
            }
            j++;
          }
          i++;
          j = 0;
        }
      }
    }
    // player controls
    public void PlayerStartWalk(Direction dir)
    {
      player.Move(dir);
    }
    public void PlayerStopWalk(Direction dir)
    {
      player.StopMove(dir);
    }
  }
}
