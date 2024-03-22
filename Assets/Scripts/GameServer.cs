using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class MKeySetClass{
  public KeyCode Up, Down, Left, Right;
  public MKeySetClass(KeyCode up, KeyCode down, KeyCode left, KeyCode right){
    Up=up; Down=down; Left=left; Right=right;
  }
}
public class GameServer : MonoBehaviour
{
  public List<Vector3> bornPos = new();
  public List<MKeySetClass> keySet = new();
  int n = 5;
  public int PlayerNumber = 2;
  public List<List<GameObject>> map = new();
  public List<List<LandBehaviour>> LBmap = new();
  public List<PlayerBehaviour> players = new();
  public void Awake() {
    transform.position = new(n,0,-10);
    GetComponent<Camera>().orthographicSize = (n+1)*0.866025f;
    bornPos.Clear();
    bornPos.Add(new(0,0,-4));
    bornPos.Add(new(2*n,0,-4));
    keySet.Clear();
    keySet.Add(new(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D));
    keySet.Add(new(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow));

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
  public bool OutOfScreen(Vector3 p){
    return false;
  }
  public void ChangeNeighborOfNeighbor(int x,int y,Neighbor tmp){
    if((tmp&Neighbor.Left)!=0) {LBmap[x-1][y-1].neighbor &= ~Neighbor.Right;LBmap[x-1][y-1].ChangeImg();}
    if((tmp&Neighbor.Right)!=0) {LBmap[x+1][y+1].neighbor &= ~Neighbor.Left;LBmap[x+1][y+1].ChangeImg();}
    if((tmp&Neighbor.LUp)!=0) {LBmap[x][y-1].neighbor &= ~Neighbor.RDown;LBmap[x][y-1].ChangeImg();}
    if((tmp&Neighbor.RDown)!=0) {LBmap[x][y+1].neighbor &= ~Neighbor.LUp;LBmap[x][y+1].ChangeImg();}
    if((tmp&Neighbor.RUp)!=0) {LBmap[x+1][y].neighbor &= ~Neighbor.LDown;LBmap[x+1][y].ChangeImg();}
    if((tmp&Neighbor.LDown)!=0) {LBmap[x-1][y].neighbor &= ~Neighbor.RUp;LBmap[x-1][y].ChangeImg();}
  }
  public void Regain(Vector2Int p){
    LBmap[p.x][p.y].owner -= PlayerNumber;
    Neighbor tmp = LBmap[p.x][p.y].neighbor;
    if((tmp&Neighbor.Left)!=0) Regain(p+NeighborPos.Left);
    if((tmp&Neighbor.Right)!=0) Regain(p+NeighborPos.Right);
    if((tmp&Neighbor.LUp)!=0) Regain(p+NeighborPos.LUp);
    if((tmp&Neighbor.RDown)!=0) Regain(p+NeighborPos.RDown);
    if((tmp&Neighbor.RUp)!=0) Regain(p+NeighborPos.RUp);
    if((tmp&Neighbor.LDown)!=0) Regain(p+NeighborPos.LDown);
  }
  public void UpdateMap(){
  }
}
