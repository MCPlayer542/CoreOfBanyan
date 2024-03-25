using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;

public class NodeInformation{
    public Vector2Int cur;
    public Vector2Int pre;
    public int Dist;
    public double Energy;
    public bool vis;
    public void Init(){
        cur=new();
        Dist=0;
        vis=false;
    }
};
public class RobotBehaviourHJQ : MonoBehaviour
{
    int n;
    GameServer s;
    PlayerBehaviour mPlayer;
    public List<List<NodeInformation>> NodeMap=new();
    float lastUpdate;
    float ReinforceProbability=0.05f;
    int CountSetBits(int n){
        int count = 0;
        while (n > 0){
            count += n & 1;
            n >>= 1;
        }
        return count;
    }
    void Start()
    {
        n=GameServer.n;
        s=Camera.main.GetComponent<GameServer>();
        for (int i = 0; i <= 2 * n; ++i){
            NodeMap.Add(new List<NodeInformation>());
            for (int j = 0; j <= 2 * n; ++j){
                if (i - j <= n && j - i <= n){
                    NodeMap[i].Add(new NodeInformation());
                }
                else NodeMap[i].Add(null);
            }
        }
    }
    void InitBFS(){
        for(int i=0;i<=2*n;++i){
            for(int j=0;j<=2*n;++j){
                if(i-j<=n&&j-i<=n){
                    NodeMap[i][j].Init();
                }
            }
        }
    }
    void BFS(Vector2Int St,int pid){
        InitBFS();
        Queue<Vector2Int> q = new();
        q.Enqueue(St);
        NodeMap[St.x][St.y].vis=true;
        while(q.Count!=0){
            Vector2Int x=q.Dequeue();
            foreach(var dlt in NeighborPos.Seek){
                Vector2Int y=x+dlt;
                if(s.OutOfScreen(y))continue;
                if(s.LBmap[y.x][y.y].nearPlayer&&!mPlayer.IsNeighbor(s.LBmap[x.x][x.y],s.LBmap[y.x][y.y],x,y))continue;
                if(!NodeMap[y.x][y.y].vis){
                    NodeMap[y.x][y.y].vis=true;
                    NodeMap[y.x][y.y].Dist=NodeMap[x.x][x.y].Dist+1;
                }
                double val=NodeMap[x.x][x.y].Energy+s.LBmap[y.x][y.y].hp*(s.LBmap[y.x][y.y].owner==pid?1:0);
                if(val<NodeMap[y.x][y.y].Energy){
                    NodeMap[y.x][y.y].Energy=val;
                    NodeMap[y.x][y.y].pre=x;
                }
            }
        }
    }
    Vector2Int GetConnect(double E){
        Vector2Int pos=new();
        int dis=114514;
        for(int i=0;i<=2*n;++i){
            for(int j=0;j<=2*n;++j){
                if(i-j<=n&&j-i<=n){
                    if(!s.LBmap[i][j].nearRoot||s.LBmap[i][j].owner!=mPlayer.pid)continue;
                    if(NodeMap[i][j].Dist<dis&&NodeMap[i][j].Energy<E){
                        dis=NodeMap[i][j].Dist;
                        pos=new(i,j);
                    }
                }
            }
        }
        return pos;
    }
    Vector2Int GetCapture(double E){
        Vector2Int pos=new(114514,114514);
        int dis=114514;
        for(int i=0;i<=2*n;++i){
            for(int j=0;j<=2*n;++j){
                if(i-j<=n&&j-i<=n){
                    if(!s.LBmap[i][j].nearRoot||s.LBmap[i][j].owner==mPlayer.pid)continue;
                    if(NodeMap[i][j].Dist<dis&&NodeMap[i][j].Energy<E){
                        dis=NodeMap[i][j].Dist;
                        pos=new(i,j);
                    }
                }
            }
        }
        return pos;
    }
    Vector2Int GetTarget(){
        Vector2Int pos=new(114514,114514);
        int num=0;
        for(int i=0;i<=2*n;++i){
            for(int j=0;j<=2*n;++j){
                if(i-j<=n&&j-i<=n){
                    if(s.LBmap[i][j].owner==mPlayer.pid)continue;
                    if(s.LBmap[i][j].nearRoot&&CountSetBits((int)s.LBmap[i][j].neighbor)>num){
                        pos=new(i,j);
                        num=CountSetBits((int)s.LBmap[i][j].neighbor);
                    }
                }
            }
        }
        return pos;
    }
    int GetDirection(Vector2Int p){
        Vector2Int pnow=mPlayer.curpos;
        int x=pnow.x,y=pnow.y;
        LandBehaviour a=s.LBmap[x][y];
        List<Vector2> b=new();
        while(NodeMap[p.x][p.y].pre!=pnow)p=NodeMap[p.x][p.y].pre;
        if (pnow + NeighborPos.Left == p) return (int)DirID.LeftID;
        if (pnow + NeighborPos.Right == p) return (int)DirID.RightID;
        if (pnow + NeighborPos.LUp == p) return (int)DirID.LUpID;
        if (pnow + NeighborPos.LDown == p) return (int)DirID.LDownID;
        if (pnow + NeighborPos.RUp == p) return (int)DirID.RUpID;
        if (pnow + NeighborPos.RDown == p) return (int)DirID.RDownID;
        return -1;
    }
    int GetDir(){
        for(int i=0;i<s.PlayerNumber;i++){
            if(i==mPlayer.pid)continue;
            BFS(s.players[i].curpos,i);
            Vector2Int p=s.PosToCell(s.bornPos[mPlayer.pid]);
            if(s.players[i].energy>NodeMap[p.x][p.y].Energy)return -1;
        }
        BFS(mPlayer.curpos,mPlayer.pid);
        double E=mPlayer.energy;
        int x=mPlayer.curpos.x,y=mPlayer.curpos.y;
        for(int i=0;i<s.PlayerNumber;i++){
            if(i==mPlayer.pid)continue;
            Vector2Int p=s.PosToCell(s.bornPos[i]);
            if(E>NodeMap[p.x][p.y].Energy*2f)return GetDirection(p);
        }
        if(!s.LBmap[x][y].nearRoot){
            Vector2Int p1=GetConnect(E);
            Vector2Int p2=GetCapture(E);
            if(p1.x==114514&&p2.x==114514)return -1;
            if(p2.x==114514||NodeMap[p1.x][p1.y].Dist<NodeMap[p2.x][p2.y].Dist)return GetDirection(p1);
            return GetDirection(p2);
        }
        else{
            if(UnityEngine.Random.Range(0f,1f)<ReinforceProbability)mPlayer.Reinforce();
            Vector2Int p=GetTarget();
            return GetDirection(p);
        }
    }
    void Update()
    {
        if(ManageGameManager.isPause)return;
        if(Time.time-lastUpdate<s.game_pace*0.5f)return;
        lastUpdate=Time.time;
        int Status=GetDir();
        if(Status==-1)s.BackHome(mPlayer.pid);
        else mPlayer.TryMove(Status);
    }
}
