using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
//using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

public class MKeySetClass
{
  public KeyCode Up, Down, Left, Right, LUp, LDown, RUp, RDown, Back, Reinforce;
  public bool vjoystick;
  public MKeySetClass(KeyCode up, KeyCode down, KeyCode left, KeyCode right, KeyCode back, KeyCode reinforce)
  {
    vjoystick = true;
    Up = up; Down = down; Left = left; Right = right; Back = back; Reinforce = reinforce;
  }
  public MKeySetClass(KeyCode lup, KeyCode rup, KeyCode left, KeyCode right, KeyCode ldown, KeyCode rdown, KeyCode back, KeyCode reinforce)
  {
    vjoystick = false;
    LUp = lup; RUp = rup; Left = left; Right = right; LDown = ldown; RDown = rdown; Back = back; Reinforce = reinforce;
  }
  public bool isKeyDown()
  {
    if (vjoystick) return ManageGameManager.GetKey(Up) || ManageGameManager.GetKey(Down) || ManageGameManager.GetKey(Left) || ManageGameManager.GetKey(Right) || ManageGameManager.GetKey(LDown) || ManageGameManager.GetKey(RDown);
    else return ManageGameManager.GetKey(LUp) || ManageGameManager.GetKey(RUp) || ManageGameManager.GetKey(LDown) || ManageGameManager.GetKey(RDown) || ManageGameManager.GetKey(Left) || ManageGameManager.GetKey(Right) || ManageGameManager.GetKey(LDown) || ManageGameManager.GetKey(RDown);
  }
}

public class GameServer : MonoBehaviour
{
  public static float gamePace = 0.4f;
  public int ControlType;
  public static bool GameOverFlag = false;
  public List<Vector3> bornPos = new();
  public static List<MKeySetClass> keySet = new();
  public static int n = 10;
  public static int PlayerNumber = 6;
  public static double wallweight = 0.1f;
  public List<List<GameObject>> map = new();
  public List<List<LandBehaviour>> LBmap = new();
  public List<PlayerBehaviour> players = new();
  public List<VJoystickBehavior> vjoysticks = new();
  public List<Color> colors = new();
  public float game_pace = 0.4f;// 1f / 3f;
  public static AudioSource end_game = null;

  public Vector3 CellToPos(int x, int y)
  { //res.z=-4 for player
    return new(0.5f * (x + y), 0.866025f * (x - y), -4);
  }
  public void Awake()
  {
    //Debug.Log(wallweight);
    game_pace = gamePace;
    LandBehaviour.s = this;
    PlayerBehaviour.s = this;
    PestAndFruitProducer.mGameServer = this;
    VJoystickBehavior.s = this;
    RobotBehaviourHJQ.s = this;
    RobotBehaviourLYK.s = this;
    FruitBehavior.life_time = 50 * game_pace;
    transform.position = new(n, 0, -10);
    GetComponent<Camera>().orthographicSize = (n + 1) * 0.866025f;
    bornPos.Clear();
    if (PlayerNumber == 2)
    {
      bornPos.Add(new(0, 0, -4));
      bornPos.Add(new(2 * n, 0, -4));
    }
    else if (PlayerNumber == 3)
    {
      bornPos.Add(new(0, 0, -4));
      bornPos.Add(CellToPos(2 * n, n));
      bornPos.Add(CellToPos(n, 2 * n));
    }
    else if (PlayerNumber == 4)
    {
      bornPos.Add(CellToPos(n, 0));
      bornPos.Add(CellToPos(2 * n, n));
      bornPos.Add(CellToPos(0, n));
      bornPos.Add(CellToPos(n, 2 * n));
    }
    else
    {
      bornPos.Add(new(0, 0, -4));
      bornPos.Add(new(2 * n, 0, -4));
      bornPos.Add(CellToPos(n, 0));
      bornPos.Add(CellToPos(0, n));
      bornPos.Add(CellToPos(2 * n, n));
      bornPos.Add(CellToPos(n, 2 * n));
    }
    for (int i = 0; i < PlayerNumber; ++i)
    {

    }
    //keySet.Clear();
    //ControlType = 0;
    //UpdateControlKeyCode();
    // for(int i=0;i<PlayerNumber;++i)
    //   colors.Add(new(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f)));
    colors = new() { new(0.1f, 0.8f, 0.1f), new(1f, 0.4f, 0.15f), Color.cyan, Color.magenta, Color.yellow, Color.gray };

    map.Clear();
    LBmap.Clear();
    for (int i = 0; i <= 2 * n; ++i)
    {
      map.Add(new List<GameObject>());
      LBmap.Add(new List<LandBehaviour>());
      for (int j = 0; j <= 2 * n; ++j)
      {
        if (i - j <= n && j - i <= n)
        {
          map[i].Add(Instantiate(Resources.Load("Land") as GameObject));
          LBmap[i].Add(map[i][j].GetComponent<LandBehaviour>());
          map[i][j].transform.localPosition = new((i + j) * 0.5f, (i - j) * 0.866025f, 0);
        }
        else
        {
          map[i].Add(null);
          LBmap[i].Add(null);
        }
      }
    }
    VJoystickAnchor anchor = new();
    anchor.prepos = map[n][n].transform.position;
    for (int i = 0; i < 6; ++i) anchor.curpos.Add(map[n + NeighborPos.Seek[i].x][n + NeighborPos.Seek[i].y].transform.position);
    anchor.curpos.Add(map[n][n].transform.position);
    VJoystickBehavior.anchor = anchor;
    players.Clear();
    for (int i = 0; i < PlayerNumber; ++i)
    {
      players.Add(Instantiate(Resources.Load("Player") as GameObject).GetComponent<PlayerBehaviour>());
      vjoysticks.Add(Instantiate(Resources.Load("VJoystick") as GameObject).GetComponent<VJoystickBehavior>());
      players[i].pid = i;
      vjoysticks[i].pid = i;
      vjoysticks[i].player = players[i];
      vjoysticks[i].transform.position = map[n][n].transform.position;
    }
    //UnityEngine.Debug.Log(PlayerNumber);
    for (int i = 0; i < PlayerNumber; ++i)
    {
      var p = PosToCell(bornPos[i]);
      var sr = map[p.x][p.y].transform.GetChild(6).GetComponent<SpriteRenderer>();
      var sqrt = Resources.Load<Sprite>("Textures/SquareRoot");
      sr.sprite = sqrt;
      var pl = players[i];
      pl.transform.position = bornPos[i];
      pl.curpos = PosToCell(bornPos[i]);
      LandBehaviour bornLand = map[pl.curpos.x][pl.curpos.y].GetComponent<LandBehaviour>();
      bornLand.owner = i;
      bornLand.hp = 50;
      bornLand.isRoot = true;
      bornLand.nearPlayer = true;
      bornLand.nearRoot = true;
      pl.energy = 3;
      bornLand.ChangeImg();
    }
    Camera.main.AddComponent<PestAndFruitProducer>();

    RandomWallGen();
    for (int i = 0; i <= 5; ++i)
    {
      if (ManageGameManager.init.robotStatus[i] == 1) players[i].AddComponent<RobotBehaviourLYK>();
      else if (ManageGameManager.init.robotStatus[i] == 2) players[i].AddComponent<RobotBehaviourHJQ>();
    }
  }
  /*void Update()
  {
    timeKeeper += Time.smoothDeltaTime;

    if (timeKeeper >= 0.5f)
    {
      timeKeeper = 0;
      for (int i = 0; i < PlayerNumber; ++i)
        players[i].Movable = true;
    }

    UpdateMap();
  }*/
  double CalcDis(Vector3 p, Vector2Int q)
  {
    // only used in PosToCell
    return (p.x - (q.x + q.y) * 0.5f) * (p.x - (q.x + q.y) * 0.5f) + (p.y - (q.x - q.y) * 0.866025f) * (p.y - (q.x - q.y) * 0.866025f);
  }
  public Vector2Int PosToCell(Vector3 p)
  {
    Vector2Int res = new(0, 0);
    double dis2 = CalcDis(p, res);
    for (int i = -1; i <= 2 * n + 1; ++i)
    {
      for (int j = -1; j <= 2 * n + 1; ++j)
      {
        double tmp = CalcDis(p, new(i, j));
        if (tmp < dis2)
        {
          dis2 = tmp;
          res = new(i, j);
        }
      }
    }
    return res;
  }
  public bool OutOfScreen(Vector2Int p)
  {
    return !((p.x >= 0) && (p.x <= 2 * n) && (p.y >= 0) && (p.y <= 2 * n) && (p.x - p.y <= n) && (p.y - p.x <= n));
  }
  public void ChangeNeighborOfNeighbor(int x, int y, Neighbor tmp)
  {
    if ((tmp & Neighbor.Left) != 0) { LBmap[x - 1][y - 1].neighbor &= ~Neighbor.Right; LBmap[x - 1][y - 1].ChangeImg(); }
    if ((tmp & Neighbor.Right) != 0) { LBmap[x + 1][y + 1].neighbor &= ~Neighbor.Left; LBmap[x + 1][y + 1].ChangeImg(); }
    if ((tmp & Neighbor.LUp) != 0) { LBmap[x][y - 1].neighbor &= ~Neighbor.RDown; LBmap[x][y - 1].ChangeImg(); }
    if ((tmp & Neighbor.RDown) != 0) { LBmap[x][y + 1].neighbor &= ~Neighbor.LUp; LBmap[x][y + 1].ChangeImg(); }
    if ((tmp & Neighbor.RUp) != 0) { LBmap[x + 1][y].neighbor &= ~Neighbor.LDown; LBmap[x + 1][y].ChangeImg(); }
    if ((tmp & Neighbor.LDown) != 0) { LBmap[x - 1][y].neighbor &= ~Neighbor.RUp; LBmap[x - 1][y].ChangeImg(); }
  }
  public void UpdateMap()
  {
    for (int i = 0; i <= 2 * n; ++i)
    {
      for (int j = 0; j <= 2 * n; ++j)
      {
        if (i - j <= n && j - i <= n)
        {
          LBmap[i][j].nearPlayer = false;
          LBmap[i][j].nearRoot = false;
        }
      }
    }
    for (int i = 0; i < PlayerNumber; ++i)
    {
      if (!players[i].alive) continue;
      dfsp(players[i].curpos);
      dfsr(PosToCell(bornPos[i]));
    }
  }
  void dfsp(Vector2Int cur)
  {
    if (OutOfScreen(cur)) return;
    if (LBmap[cur.x][cur.y].nearPlayer) return;
    LBmap[cur.x][cur.y].nearPlayer = true;
    Neighbor tmp = LBmap[cur.x][cur.y].neighbor;
    if ((tmp & Neighbor.Left) != 0) dfsp(cur + NeighborPos.Left);
    if ((tmp & Neighbor.Right) != 0) dfsp(cur + NeighborPos.Right);
    if ((tmp & Neighbor.LUp) != 0) dfsp(cur + NeighborPos.LUp);
    if ((tmp & Neighbor.RDown) != 0) dfsp(cur + NeighborPos.RDown);
    if ((tmp & Neighbor.RUp) != 0) dfsp(cur + NeighborPos.RUp);
    if ((tmp & Neighbor.LDown) != 0) dfsp(cur + NeighborPos.LDown);
  }
  void dfsr(Vector2Int cur)
  {
    if (OutOfScreen(cur)) return;
    if (LBmap[cur.x][cur.y].nearRoot) return;
    LBmap[cur.x][cur.y].nearRoot = true;
    Neighbor tmp = LBmap[cur.x][cur.y].neighbor;
    if ((tmp & Neighbor.Left) != 0) dfsr(cur + NeighborPos.Left);
    if ((tmp & Neighbor.Right) != 0) dfsr(cur + NeighborPos.Right);
    if ((tmp & Neighbor.LUp) != 0) dfsr(cur + NeighborPos.LUp);
    if ((tmp & Neighbor.RDown) != 0) dfsr(cur + NeighborPos.RDown);
    if ((tmp & Neighbor.RUp) != 0) dfsr(cur + NeighborPos.RUp);
    if ((tmp & Neighbor.LDown) != 0) dfsr(cur + NeighborPos.LDown);
  }
  public void BackHome(int pid)
  {
    if (!players[pid].returning && !players[pid].isRobot) players[pid].forced_return.Play();
    players[pid].curpos = PosToCell(bornPos[pid]);
    players[pid].transform.position = bornPos[pid];
  }

  void UpdateControlKeyCode()
  {
    keySet.Add(new(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Alpha1, KeyCode.Alpha2));
    keySet.Add(new(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.Comma, KeyCode.Period));
    keySet.Add(new(0, 0, 0, 0, 0, 0));
    keySet.Add(new(0, 0, 0, 0, 0, 0));
    keySet.Add(new(0, 0, 0, 0, 0, 0));
    keySet.Add(new(0, 0, 0, 0, 0, 0));
  }

  public virtual void EndGame()
  {
    //UnityEngine.Debug.Log("orz");
    GameOverFlag = true;
    foreach (var i in LBmap)
    {
      foreach (var j in i)
      {
        if (j != null) j.EndGame();
      }
    }
    foreach (var i in players)
    {
      i.EndGame();
    }
    foreach (var i in vjoysticks)
    {
      i.EndGame();
    }
    Destroy(Camera.main.GetComponent<PestAndFruitProducer>());
    Destroy(Camera.main.GetComponent<GameServer>());
  }

  public virtual void GameOver()
  {
    end_game.Play();
    GameOverFlag = true;
    if (ManageGameManager.isTutorial) Camera.main.GetComponent<ManageGameManager>().ChangeDisplayStatus(new() { 4, 5 });
    else Camera.main.GetComponent<ManageGameManager>().ChangeDisplayStatus(new() { 2 });
    //EndGame();
  }
  void RandomWallGen()
  {
    for (int i = 0; i <= 2 * n; ++i)
      for (int j = 0; j <= 2 * n; ++j)
        if (i - j <= n && j - i <= n)
        {
          LBmap[i][j].isWall = false;
          map[i][j].SetActive(true);
        }
    int mapNum = 3 * n * n + 3 * n + 1, wallNum = (int)(wallweight * mapNum), wallCnt = 0, failCnt = 0;
    while (wallCnt < wallNum && failCnt < 10 * wallNum)
    {
      int x = Random.Range(0, 2 * n + 1), y = Random.Range(0, 2 * n + 1);
      if (x - y > n || y - x > n || LBmap[x][y].isWall)
      {
        ++failCnt;
        continue;
      }
      LBmap[x][y].isWall = true;
      if (ConnectivityCheck(mapNum - wallCnt - 1))
      {
        ++wallCnt;
        map[x][y].SetActive(false);
      }
      else
      {
        LBmap[x][y].isWall = false;
        ++failCnt;
      }
    }
    if (wallCnt < wallNum) Debug.Log("fail: (" + wallCnt + "/" + wallNum + "), " + failCnt);
    else Debug.Log("succeed");
  }
  bool ConnectivityCheck(int spaceNum)
  {
    for (int i = 0; i < PlayerNumber; ++i)
    {
      var cur = PosToCell(bornPos[i]);
      if (LBmap[cur.x][cur.y].isWall)
        return false;
    }
    int spaceCnt = 0;
    Queue<Vector2Int> q = new();
    HashSet<Vector2Int> vis = new();
    q.Enqueue(PosToCell(bornPos[0]));
    vis.Add(PosToCell(bornPos[0]));
    while (q.Count != 0)
    {
      var cur = q.Dequeue();
      ++spaceCnt;
      for (int i = 0; i < 6; ++i)
      {
        var k = cur + NeighborPos.Seek[i];
        if (!OutOfScreen(k) && !vis.Contains(k) && !LBmap[k.x][k.y].isWall)
        {
          q.Enqueue(k);
          vis.Add(k);
        }
      }
    }
    return (spaceCnt == spaceNum);
  }
}
