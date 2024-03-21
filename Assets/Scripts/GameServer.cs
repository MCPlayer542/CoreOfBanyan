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
  int n = 5;
  public List<List<GameObject>> map = new();
  public void Awake() {
    transform.position = new(n,0,-10);
    GetComponent<Camera>().orthographicSize = (n+1)*0.866025f;
    bornPos.Clear();
    bornPos.Add(new(0,0,-1));
    bornPos.Add(new(2*n,0,-1));
    keySet.Clear();
    keySet.Add(new(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D));
    keySet.Add(new(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow));

    map.Clear();
    for(int i=0;i<=2*n;++i) {
      map.Add(new List<GameObject>());
      for(int j=0;j<=2*n;++j) {
        if(i-j<=n&&j-i<=n){
          map[i].Add(Instantiate(Resources.Load("Land") as GameObject));
          map[i][j].transform.localPosition = new((i+j)*0.5f,(i-j)*0.866025f,0);
        }
        else{
          map[i].Add(new());
        }
      }
    }
  }
  public bool OutOfScreen(Vector3 p){
    return false;
  }
}
