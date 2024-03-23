using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class MKeySetClass{
  public KeyCode Up, Down, Left, Right,LUp,LDown,RUp,RDown,Back,Enforce;
  public MKeySetClass(KeyCode up, KeyCode down, KeyCode left, KeyCode right){
    Up=up; Down=down; Left=left; Right=right;
  }
  public MKeySetClass(KeyCode lup,KeyCode rup,KeyCode left,KeyCode right,KeyCode ldown,KeyCode rdown,KeyCode back,KeyCode enforce){
    LUp=lup; RUp=rup; Left=left; Right=right; LDown=ldown; RDown=rdown; Back=back; Enforce=enforce;
  }
}
public class GameServer : MonoBehaviour
{
  public int ControlType;
  public bool GameOverFlag=false;
  public List<Vector3> bornPos = new();
  public List<MKeySetClass> keySet = new();
  public int n = 5;
  public int PlayerNumber = 2;
  public List<List<GameObject>> map = new();
  public List<List<LandBehaviour>> LBmap = new();
  public List<PlayerBehaviour> players = new();
  public void Awake() {
    LandBehaviour.s = this;
    transform.position = new(n,0,-10);
    GetComponent<Camera>().orthographicSize = (n+1)*0.866025f;
    bornPos.Clear();
    bornPos.Add(new(0,0,-4));
    bornPos.Add(new(2*n,0,-4));
    keySet.Clear();
    ControlType=0;///////////////////////////////////////////////////////////////////////////////////////
    if(ControlType==0){
      keySet.Add(new(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D));
      keySet.Add(new(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow));
    }
    else{
      keySet.Add(new(KeyCode.Q, KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.Z, KeyCode.X, KeyCode.E, KeyCode.D));
      keySet.Add(new(KeyCode.I, KeyCode.O, KeyCode.K, KeyCode.L, KeyCode.Comma, KeyCode.Period, KeyCode.P, KeyCode.Semicolon));
    }

    map.Clear();
    for(int i=0;i<=2*n;++i) {
      map.Add(new List<GameObject>());
      LBmap.Add(new List<LandBehaviour>());
      for(int j=0;j<=2*n;++j) {
        if(i-j<=n&&j-i<=n){
          map[i].Add(Instantiate(Resources.Load("Land") as GameObject));
          LBmap[i].Add(map[i][j].GetComponent<LandBehaviour>());
          map[i][j].transform.localPosition = new((i+j)*0.5f,(i-j)*0.866025f,0);
        }
        else{
          map[i].Add(new());
          LBmap[i].Add(new());
        }
      }
    }

    for(int i=0;i<PlayerNumber;++i){
      players.Add(Instantiate(Resources.Load("Player") as GameObject).GetComponent<PlayerBehaviour>());
      players[i].pid=i;
    }
  }
  float CalcDis(Vector3 p, Vector2Int q){
    // only used in PosToCell
    return (p.x-(q.x+q.y)*0.5f)*(p.x-(q.x+q.y)*0.5f) + (p.y-(q.x-q.y)*0.866025f)*(p.y-(q.x-q.y)*0.866025f);
  }
  public Vector2Int PosToCell(Vector3 p){
    Vector2Int res = new(0,0);
    float dis2 = CalcDis(p,res);
    for(int i=-1;i<=2*n+1;++i) {
      for(int j=-1;j<=2*n+1;++j) {
        float tmp = CalcDis(p,new(i,j));
        if(tmp<dis2){
          dis2=tmp;
          res=new(i,j);
        }
      }
    }
    return res;
  }
  public bool OutOfScreen(Vector2Int p){
    return ! ( (p.x>=0)&&(p.x<=2*n)&&(p.y>=0)&&(p.y<=2*n)&&(p.x-p.y<=n)&&(p.y-p.x<=n) );
  }
  public void ChangeNeighborOfNeighbor(int x,int y,Neighbor tmp){
    if((tmp&Neighbor.Left)!=0) {LBmap[x-1][y-1].neighbor &= ~Neighbor.Right;LBmap[x-1][y-1].ChangeImg();}
    if((tmp&Neighbor.Right)!=0) {LBmap[x+1][y+1].neighbor &= ~Neighbor.Left;LBmap[x+1][y+1].ChangeImg();}
    if((tmp&Neighbor.LUp)!=0) {LBmap[x][y-1].neighbor &= ~Neighbor.RDown;LBmap[x][y-1].ChangeImg();}
    if((tmp&Neighbor.RDown)!=0) {LBmap[x][y+1].neighbor &= ~Neighbor.LUp;LBmap[x][y+1].ChangeImg();}
    if((tmp&Neighbor.RUp)!=0) {LBmap[x+1][y].neighbor &= ~Neighbor.LDown;LBmap[x+1][y].ChangeImg();}
    if((tmp&Neighbor.LDown)!=0) {LBmap[x-1][y].neighbor &= ~Neighbor.RUp;LBmap[x-1][y].ChangeImg();}
  }
  public void UpdateMap(){
    for(int i=0;i<=2*n;++i) {
      for(int j=0;j<=2*n;++j) {
        if(i-j<=n&&j-i<=n){
          if(LBmap[i][j].owner != -1){
            LBmap[i][j].nearPlayer = false;
            LBmap[i][j].nearRoot = false;
          }
        }
      }
    }
    for(int i=0;i<PlayerNumber;++i){
      dfsp(players[i].curpos);
      dfsr(PosToCell(bornPos[i]));
    }
  }
  void dfsp(Vector2Int cur){
    if(OutOfScreen(cur)) return;
    if(LBmap[cur.x][cur.y].nearPlayer) return;
    LBmap[cur.x][cur.y].nearPlayer = true;
    Neighbor tmp = LBmap[cur.x][cur.y].neighbor;
    if((tmp&Neighbor.Left)!=0) dfsp(cur+NeighborPos.Left);
    if((tmp&Neighbor.Right)!=0) dfsp(cur+NeighborPos.Right);
    if((tmp&Neighbor.LUp)!=0) dfsp(cur+NeighborPos.LUp);
    if((tmp&Neighbor.RDown)!=0) dfsp(cur+NeighborPos.RDown);
    if((tmp&Neighbor.RUp)!=0) dfsp(cur+NeighborPos.RUp);
    if((tmp&Neighbor.LDown)!=0) dfsp(cur+NeighborPos.LDown);
  }
  void dfsr(Vector2Int cur){
    if(OutOfScreen(cur)) return;
    if(LBmap[cur.x][cur.y].nearRoot) return;
    LBmap[cur.x][cur.y].nearRoot = true;
    Neighbor tmp = LBmap[cur.x][cur.y].neighbor;
    if((tmp&Neighbor.Left)!=0) dfsr(cur+NeighborPos.Left);
    if((tmp&Neighbor.Right)!=0) dfsr(cur+NeighborPos.Right);
    if((tmp&Neighbor.LUp)!=0) dfsr(cur+NeighborPos.LUp);
    if((tmp&Neighbor.RDown)!=0) dfsr(cur+NeighborPos.RDown);
    if((tmp&Neighbor.RUp)!=0) dfsr(cur+NeighborPos.RUp);
    if((tmp&Neighbor.LDown)!=0) dfsr(cur+NeighborPos.LDown);
  }
  public void BackHome(int pid){
    players[pid].curpos = PosToCell(bornPos[pid]);
    players[pid].transform.position = bornPos[pid];
  }
  void Update(){
    if(Input.GetKeyDown(KeyCode.Backslash))ControlType^=1;
  }
}
