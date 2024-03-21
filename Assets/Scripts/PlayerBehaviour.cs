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
        energy = bornLand.hp;
        Capture(bornLand,bornLand,b,b);
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
            if(!IsNeighbor(curLand,preLand)) return;
        }
        else{
            Debug.Log("!");
            if(!Capture(curLand,preLand,cur,pre)) return;
            Debug.Log("?");
        }

        transform.position = p;
    }
    Vector2Int PosToCell(Vector3 p){
        return new Vector2Int( (int)Math.Round(p.x+p.y/1.732051f), (int)Math.Round(p.x-p.y/1.732051f) );
    }
    bool IsNeighbor(LandBehaviour a, LandBehaviour b){
        return true;
    }
    bool Capture(LandBehaviour curLand, LandBehaviour preLand, Vector2Int cur, Vector2Int pre){
        if(energy<curLand.hp) return false;
        LandBehaviour.Neighbor tmp = curLand.neighbor;
        curLand.neighbor = 0;
        if( cur.x+1==pre.x && cur.y==pre.y){
            curLand.neighbor |= LandBehaviour.Neighbor.RUp;
            preLand.neighbor |= LandBehaviour.Neighbor.LDown;
        }
        else if( cur.x-1==pre.x && cur.y==pre.y){
            curLand.neighbor |= LandBehaviour.Neighbor.LDown;
            preLand.neighbor |= LandBehaviour.Neighbor.RUp;
        }
        else if( cur.x==pre.x && cur.y+1==pre.y){
            curLand.neighbor |= LandBehaviour.Neighbor.RDown;
            preLand.neighbor |= LandBehaviour.Neighbor.LUp;
        }
        else if( cur.x==pre.x && cur.y-1==pre.y){
            curLand.neighbor |= LandBehaviour.Neighbor.LUp;
            preLand.neighbor |= LandBehaviour.Neighbor.RDown;
        }
        else if( cur.x+1==pre.x && cur.y+1==pre.y){
            curLand.neighbor |= LandBehaviour.Neighbor.Right;
            preLand.neighbor |= LandBehaviour.Neighbor.Left;
        }
        else if( cur.x-1==pre.x && cur.y-1==pre.y){
            curLand.neighbor |= LandBehaviour.Neighbor.Left;
            preLand.neighbor |= LandBehaviour.Neighbor.Right;
        }
        else if( cur.x==pre.x && cur.y==pre.y){
            
        }
        else{
            curLand.neighbor = tmp;
            return false;
        }
        energy -= curLand.hp;
        curLand.owner = pid;
        curLand.ChangeImg();
        preLand.ChangeImg();
        return true;
    }
}
