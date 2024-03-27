using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

public class MKeySetClass
{
  public KeyCode Up, Down, Left, Right, LUp, LDown, RUp, RDown, Back, Reinforce;
  public MKeySetClass(KeyCode up, KeyCode down, KeyCode left, KeyCode right, KeyCode back, KeyCode reinforce)
  {
    Up = up; Down = down; Left = left; Right = right; Back = back; Reinforce = reinforce;
  }
  public MKeySetClass(KeyCode lup, KeyCode rup, KeyCode left, KeyCode right, KeyCode ldown, KeyCode rdown, KeyCode back, KeyCode reinforce)
  {
    LUp = lup; RUp = rup; Left = left; Right = right; LDown = ldown; RDown = rdown; Back = back; Reinforce = reinforce;
  }
  public bool isKeyDown()
  {
    return ManageGameManager.GetKey(Up) || ManageGameManager.GetKey(Down) || ManageGameManager.GetKey(Left) || ManageGameManager.GetKey(Right) || ManageGameManager.GetKey(LDown) || ManageGameManager.GetKey(RDown);
  }
}

public class GameServer : MonoBehaviour
{
  public int ControlType;
  public static bool GameOverFlag = false;
  public List<Vector3> bornPos = new();
  public List<MKeySetClass> keySet = new();
  public static int n = 10;
  public int PlayerNumber = 6;
  public List<List<GameObject>> map = new();
  public List<List<LandBehaviour>> LBmap = new();
  public List<PlayerBehaviour> players = new();
  public List<VJoystickBehavior> vjoysticks = new();
  public List<Vector2Int> wallList = new();
  public List<Color> colors = new();
  public float game_pace = 1f / 3f;
  public AudioSource end_game=null;
  public Vector3 CellToPos(int x,int y){ //res.z=-4 for player
    return new(0.5f*(x+y),0.866025f*(x-y),-4);
  }
  public void Awake()
  {
    LandBehaviour.s = this;
    PlayerBehaviour.s = this;
    PestAndFruitProducer.mGameServer = this;
    VJoystickBehavior.s = this;
    RobotBehaviourHJQ.s=this;
    RobotBehaviourLYK.s=this;
    FruitBehavior.life_time = 50 * game_pace;
    transform.position = new(n, 0, -10);
    GetComponent<Camera>().orthographicSize = (n + 1) * 0.866025f;
    bornPos.Clear();
    bornPos.Add(new(0, 0, -4));
    bornPos.Add(new(2 * n, 0, -4));
    bornPos.Add(CellToPos(n, 0));
    bornPos.Add(CellToPos(0, n));
    bornPos.Add(CellToPos(2 * n, n));
    bornPos.Add(CellToPos(n, 2 * n));
    keySet.Clear();
    ControlType = 0;
    UpdateControlKeyCode();
    for(int i=0;i<PlayerNumber;++i)
      colors.Add(new(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f)));

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
    for(int i=0;i<PlayerNumber;++i){
      var p = PosToCell(bornPos[i]);
      var sr = map[p.x][p.y].transform.GetChild(6).GetComponent<SpriteRenderer>();
      var sqrt = Resources.Load<Sprite>("Textures/SquareRoot");
      sr.sprite = sqrt;
      var pl=players[i];
      pl.transform.position = bornPos[i];
      pl.curpos = PosToCell(bornPos[i]);
      LandBehaviour bornLand = map[pl.curpos.x][pl.curpos.y].GetComponent<LandBehaviour>();
      bornLand.owner=i;
      bornLand.hp = 50;
      bornLand.isRoot = true;
      bornLand.nearPlayer = true;
      bornLand.nearRoot = true;
      pl.energy = 3;
      bornLand.ChangeImg();
    }
    Camera.main.AddComponent<PestAndFruitProducer>();

    wallList = new(){};
    foreach(var p in wallList){
      LBmap[p.x][p.y].isWall = true;
      map[p.x][p.y].SetActive(false);
    }
    for(int i=0;i<=5;++i)
   {
    var p=players[i].AddComponent<RobotBehaviourHJQ>();
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
  float CalcDis(Vector3 p, Vector2Int q)
  {
    // only used in PosToCell
    return (p.x - (q.x + q.y) * 0.5f) * (p.x - (q.x + q.y) * 0.5f) + (p.y - (q.x - q.y) * 0.866025f) * (p.y - (q.x - q.y) * 0.866025f);
  }
  public Vector2Int PosToCell(Vector3 p)
  {
    Vector2Int res = new(0, 0);
    float dis2 = CalcDis(p, res);
    for (int i = -1; i <= 2 * n + 1; ++i)
    {
      for (int j = -1; j <= 2 * n + 1; ++j)
      {
        float tmp = CalcDis(p, new(i, j));
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
      if(!players[i].alive)continue;
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
    players[pid].curpos = PosToCell(bornPos[pid]);
    players[pid].transform.position = bornPos[pid];
  }

  void UpdateControlKeyCode()
  {
    keySet.Add(new(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Alpha1, KeyCode.Alpha2));
    keySet.Add(new(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.Comma, KeyCode.Period));
    keySet.Add(new(0,0,0,0,0,0));
    keySet.Add(new(0,0,0,0,0,0));
    keySet.Add(new(0,0,0,0,0,0));
    keySet.Add(new(0,0,0,0,0,0));
  }

  public virtual void EndGame()
  {
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
    GameOverFlag=true;
  }
}
