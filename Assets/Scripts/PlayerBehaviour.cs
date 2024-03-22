using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Collections;
using UnityEditor.Build.Content;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public int pid;
    public float speed;
    public float energy;
    public float energyGrowthSpeed;
    GameServer s;
    // Start is called before the first frame update
    void Start()
    {
        s = Camera.main.GetComponent<GameServer>();
        transform.position = s.bornPos[pid]; 
        speed = 3.0f;
        energyGrowthSpeed = 5;
        Vector2Int b = PosToCell(s.bornPos[pid]);
        LandBehaviour bornLand = s.map[b.x][b.y].GetComponent<LandBehaviour>();
        energy = 114514; //big enough
        TryCapture(bornLand,bornLand,b,b);
        energy = 0;
    }

    // Update is called once per frame
    void Update()
    {
        energy += energyGrowthSpeed * Time.smoothDeltaTime;
        Vector3 p = transform.position;
        if(Input.GetKey(s.keySet[pid].Up))      p.y += speed * Time.smoothDeltaTime;
        if(Input.GetKey(s.keySet[pid].Down))    p.y -= speed * Time.smoothDeltaTime;
        if(Input.GetKey(s.keySet[pid].Left))    p.x -= speed * Time.smoothDeltaTime;
        if(Input.GetKey(s.keySet[pid].Right))   p.x += speed * Time.smoothDeltaTime;

        if(s.OutOfScreen(p)) return;

        Vector2Int pre = PosToCell(transform.position), cur = PosToCell(p);
        if(pre==cur) {
            transform.position = p;
            return;
        }
        LandBehaviour preLand = s.map[pre.x][pre.y].GetComponent<LandBehaviour>();
        LandBehaviour curLand = s.map[cur.x][cur.y].GetComponent<LandBehaviour>();
        if(curLand.owner == pid){
            if(!IsNeighbor(curLand,preLand,cur,pre))
                return;
        }
        else if(curLand.owner == pid+s.PlayerNumber){
            s.Regain(cur);
            if(!TryCapture(curLand,preLand,cur,pre)) return;
        }
        else{
            if(!TryCapture(curLand,preLand,cur,pre)) return;
        }

        transform.position = p;
    }
    bool TryConnect(LandBehaviour curLand, LandBehaviour preLand, Vector2Int cur, Vector2Int pre){
        if(cur+NeighborPos.RUp==pre){
            curLand.neighbor |= Neighbor.RUp;
            preLand.neighbor |= Neighbor.LDown;
        }
        else if(cur+NeighborPos.LDown==pre){
            curLand.neighbor |= Neighbor.LDown;
            preLand.neighbor |= Neighbor.RUp;
        }
        else if(cur+NeighborPos.RDown==pre){
            curLand.neighbor |= Neighbor.RDown;
            preLand.neighbor |= Neighbor.LUp;
        }
        else if(cur+NeighborPos.LUp==pre){
            curLand.neighbor |= Neighbor.LUp;
            preLand.neighbor |= Neighbor.RDown;
        }
        else if(cur+NeighborPos.Right==pre){
            curLand.neighbor |= Neighbor.Right;
            preLand.neighbor |= Neighbor.Left;
        }
        else if(cur+NeighborPos.Left==pre){
            curLand.neighbor |= Neighbor.Left;
            preLand.neighbor |= Neighbor.Right;
        }
        else if(cur==pre){
            
        }
        else{
            return false;
        }
        return true;
    }
    Vector2Int PosToCell(Vector3 p){
        return new Vector2Int( (int)Math.Round(p.x+p.y/1.732051f), (int)Math.Round(p.x-p.y/1.732051f) );
    }
    bool IsNeighbor(LandBehaviour a, LandBehaviour b, Vector2Int pa, Vector2Int pb){
        if(pa+NeighborPos.Left==pb && (a.neighbor&Neighbor.Left)!=0) return true;
        if(pa+NeighborPos.Right==pb && (a.neighbor&Neighbor.Right)!=0) return true;
        if(pa+NeighborPos.LUp==pb && (a.neighbor&Neighbor.LUp)!=0) return true;
        if(pa+NeighborPos.LDown==pb && (a.neighbor&Neighbor.LDown)!=0) return true;
        if(pa+NeighborPos.RUp==pb && (a.neighbor&Neighbor.RUp)!=0) return true;
        if(pa+NeighborPos.RDown==pb && (a.neighbor&Neighbor.RDown)!=0) return true;
        return false;
    }
    bool TryCapture(LandBehaviour curLand, LandBehaviour preLand, Vector2Int cur, Vector2Int pre){
        if(energy<curLand.hp) return false;
        Neighbor tmp = curLand.neighbor;
        curLand.neighbor = 0;
        if(!TryConnect(curLand,preLand,cur,pre)){
            curLand.neighbor = tmp;
            return false;
        }
        energy -= curLand.hp;
        if(curLand.owner != -1) s.ChangeNeighborOfNeighbor(cur.x,cur.y,tmp);
        curLand.owner = pid;
        curLand.ChangeImg();
        preLand.ChangeImg();
        return true;
    }
}
