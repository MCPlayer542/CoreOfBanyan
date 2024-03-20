using System.Collections;
using System.Collections.Generic;
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
  int n = 20;
  public List<List<GameObject>> map = new();
  public void Awake() {
    bornPos.Clear();
    bornPos.Add(new(1,1,-1));
    bornPos.Add(new(20,20,-1));
    keySet.Clear();
    keySet.Add(new(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D));
    keySet.Add(new(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow));

    map.Clear(); map.Add(new());
    for(int i=1;i<=n;++i) {
      map.Add(new List<GameObject>(){new()});
      for(int j=1;j<=n;++j) {
        map[i].Add(Instantiate(Resources.Load("Land") as GameObject));
        map[i][j].transform.localPosition = new(i,j,0);
      }
    }
  }
  public bool OutOfScreen(Vector3 p){
    return p.x < 0.5 || p.x > n+0.5 || p.y < 0.5 || p.y > n+0.5;
  }
}
