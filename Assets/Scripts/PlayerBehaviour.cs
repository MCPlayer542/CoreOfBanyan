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
    public Vector2Int curpos;
    GameServer s;
    // Start is called before the first frame update
    void Start()
    {
        s = Camera.main.GetComponent<GameServer>();
        transform.position = s.bornPos[pid]; 
        speed = 3.0f;
        energyGrowthSpeed = 5;
        curpos = s.PosToCell(s.bornPos[pid]);
        LandBehaviour bornLand = s.map[curpos.x][curpos.y].GetComponent<LandBehaviour>();
        energy = 114514; //big enough
        TryCapture(bornLand,bornLand,curpos,curpos,true);
        energy = 0;
    }
    private const float S3_2=0.8660254f;
    public class DirVector{
        static public Vector3 Left=new(-1f,0f,0f), Right=new(1f,0f,0f), Up=new(0f,1f,0f),Down=new(0f,-1f,0f),
        RUp=new(0.5f,S3_2,0f), LDown=new(-0.5f,-S3_2,0f), RDown=new(0.5f,-S3_2,0f), LUp=new(-0.5f,S3_2,0f);
    }
    // Update is called once per frame
    void Update()
    {
        if(s.GameOverFlag)return;
        energy += energyGrowthSpeed * Time.smoothDeltaTime;
        Vector3 p = transform.position;
        int tx=0,ty=0;
        if(Input.GetKey(s.keySet[pid].Up))      ty++;
        if(Input.GetKey(s.keySet[pid].Down))    ty--;
        if(Input.GetKey(s.keySet[pid].Left))    tx--;
        if(Input.GetKey(s.keySet[pid].Right))   tx++;
        if(tx!=0&&ty!=0){
            if(tx==1&&ty==1)p+=speed*Time.smoothDeltaTime*DirVector.RUp;
            if(tx==1&&ty==-1)p+=speed*Time.smoothDeltaTime*DirVector.RDown;
            if(tx==-1&&ty==1)p+=speed*Time.smoothDeltaTime*DirVector.LUp;
            if(tx==-1&&ty==-1)p+=speed*Time.smoothDeltaTime*DirVector.LDown;
        }
        else{
            if(tx==1)p+=speed*Time.smoothDeltaTime*DirVector.Right;
            if(tx==-1)p+=speed*Time.smoothDeltaTime*DirVector.Left;
            if(ty==1)p+=speed*Time.smoothDeltaTime*DirVector.Up;
            if(ty==-1)p+=speed*Time.smoothDeltaTime*DirVector.Down;
        }

        Vector2Int pre = s.PosToCell(transform.position), cur = s.PosToCell(p);
        if(s.OutOfScreen(cur)) return;
        if(pre==cur) {
            transform.position = p;
            return;
        }
        LandBehaviour preLand = s.map[pre.x][pre.y].GetComponent<LandBehaviour>();
        LandBehaviour curLand = s.map[cur.x][cur.y].GetComponent<LandBehaviour>();
        if(curLand.owner == pid){
            if(curLand.nearPlayer){
                if(!IsNeighbor(curLand,preLand,cur,pre))
                    return;
            }
            else{
                if(!TryCapture(curLand,preLand,cur,pre,false)) return;
            }
        }
        else{
            if(!TryCapture(curLand,preLand,cur,pre,true)) return;
            Vector2Int p1=new(0,0),p2=new(2*s.n,2*s.n);
            if(cur==p1||cur==p2)s.GameOverFlag=true;
        }
        s.UpdateMap();
        transform.position = p;
        curpos = cur;
        s.LBmap[pre.x][pre.y].hp -= energy;
        if(s.LBmap[cur.x][cur.y].mFruit!=null){
            Destroy(s.LBmap[cur.x][cur.y].mFruit);
            energy+=s.LBmap[cur.x][cur.y].GetFruitsEnergy();
            s.LBmap[cur.x][cur.y].mFruit=null;
        }
        if(s.LBmap[cur.x][cur.y].mPest!=null){
            Destroy(s.LBmap[cur.x][cur.y].mPest);
            energy-=s.LBmap[cur.x][cur.y].GetPestsEnergy();
            s.LBmap[cur.x][cur.y].mPest=null;
        }
        s.LBmap[cur.x][cur.y].hp += energy;
        Conflict();
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
    bool IsNeighbor(LandBehaviour a, LandBehaviour b, Vector2Int pa, Vector2Int pb){
        if(pa+NeighborPos.Left==pb && (a.neighbor&Neighbor.Left)!=0) return true;
        if(pa+NeighborPos.Right==pb && (a.neighbor&Neighbor.Right)!=0) return true;
        if(pa+NeighborPos.LUp==pb && (a.neighbor&Neighbor.LUp)!=0) return true;
        if(pa+NeighborPos.LDown==pb && (a.neighbor&Neighbor.LDown)!=0) return true;
        if(pa+NeighborPos.RUp==pb && (a.neighbor&Neighbor.RUp)!=0) return true;
        if(pa+NeighborPos.RDown==pb && (a.neighbor&Neighbor.RDown)!=0) return true;
        return false;
    }
    bool TryCapture(LandBehaviour curLand, LandBehaviour preLand, Vector2Int cur, Vector2Int pre, bool clearPastEdge){
        if(energy<curLand.hp) return false;
        Neighbor tmp = curLand.neighbor;
        if(clearPastEdge) curLand.neighbor = 0;
        if(!TryConnect(curLand,preLand,cur,pre)){
            curLand.neighbor = tmp;
            return false;
        }
        energy -= curLand.hp;
        if(curLand.owner != -1 && clearPastEdge) s.ChangeNeighborOfNeighbor(cur.x,cur.y,tmp);
        curLand.owner = pid;
        curLand.ChangeImg();
        preLand.ChangeImg();
        return true;
    }
    void Conflict()
    {
        //Debug.Log("shit " + curpos + " " + s.players[1 - pid].curpos);
        for (int i = 0; i < s.PlayerNumber; ++i)
        {
            if (i != pid && curpos == s.players[i].curpos)
            {
                s.players[i].energy = 0;
                s.players[i].transform.localPosition = s.bornPos[i];
                s.players[i].curpos = s.PosToCell(s.bornPos[i]);
                s.LBmap[s.players[i].curpos.x][s.players[i].curpos.y].hp -= s.players[i].energy;
            }
        }
        s.UpdateMap();
    }
}
