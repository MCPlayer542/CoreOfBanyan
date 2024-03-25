using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using JetBrains.Annotations;
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
        pre=new();
        Dist=0;
        Energy=1e6;
        vis=false;
    }
    public int CompareTo(NodeInformation other){
        if (other == null) return 1;
        return Energy.CompareTo(other.Energy);
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
        mPlayer=gameObject.GetComponent<PlayerBehaviour>();
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
    void InitNodeMap(){
        for(int i=0;i<=2*n;++i){
            for(int j=0;j<=2*n;++j){
                if(i-j<=n&&j-i<=n){
                    NodeMap[i][j].Init();
                    NodeMap[i][j].cur=new(i,j);
                }
            }
        }
    }
    void BFS(Vector2Int St,int pid){
        InitNodeMap();
        Queue<Vector2Int> q = new();
        q.Enqueue(St);
        NodeMap[St.x][St.y].vis=true;
        NodeMap[St.x][St.y].Energy=0;
        while(q.Count!=0){
            Vector2Int x=q.Dequeue();
            foreach(var dlt in NeighborPos.Seek){
                Vector2Int y=x+dlt;
                if(s.OutOfScreen(y))continue;
                if(s.LBmap[y.x][y.y].nearPlayer&&s.LBmap[y.x][y.y].owner==pid&&!mPlayer.IsNeighbor(s.LBmap[x.x][x.y],s.LBmap[y.x][y.y],x,y))continue;
                if(!NodeMap[y.x][y.y].vis){
                    NodeMap[y.x][y.y].vis=true;
                    NodeMap[y.x][y.y].Dist=NodeMap[x.x][x.y].Dist+1;
                    NodeMap[y.x][y.y].pre=x;
                    q.Enqueue(y);
                }
                if(NodeMap[y.x][y.y].Dist==NodeMap[x.x][x.y].Dist+1){
                    double val=NodeMap[x.x][x.y].Energy+Math.Max(s.LBmap[y.x][y.y].hp,1.0f)*(s.LBmap[y.x][y.y].owner!=pid?1:0);
                    if(s.LBmap[y.x][y.y].owner!=-1&&s.players[s.LBmap[y.x][y.y].owner].curpos==y)val+=s.players[s.LBmap[y.x][y.y].owner].energy;
                    if(val<NodeMap[y.x][y.y].Energy){
                        NodeMap[y.x][y.y].Energy=val;
                        NodeMap[y.x][y.y].pre=x;
                    }
                }
            }
        }
    }
    void Dijkstra(Vector2Int St,int pid){
        InitNodeMap();
        SortedSet<NodeInformation> q = new();
        NodeMap[St.x][St.y].Energy=0;
        q.Add(NodeMap[St.x][St.y]);
        while(q.Count!=0){
            Vector2Int x=q.Min.cur;
            q.Remove(q.Min);
            if(NodeMap[x.x][x.y].vis)continue;
            Debug.Log("Dijkstra: "+pid+" "+x.x+" "+x.y);
            NodeMap[x.x][x.y].vis=true;
            foreach(var dlt in NeighborPos.Seek){
                Vector2Int y=x+dlt;
                if(s.OutOfScreen(y))continue;
                if(s.LBmap[y.x][y.y].nearPlayer&&s.LBmap[y.x][y.y].owner==pid&&!mPlayer.IsNeighbor(s.LBmap[x.x][x.y],s.LBmap[y.x][y.y],x,y))continue;
                Debug.Log("!1 Dijkstra: "+pid+" "+y.x+" "+y.y);
                double val=NodeMap[x.x][x.y].Energy+Math.Max(s.LBmap[y.x][y.y].hp,1.0f)*(s.LBmap[y.x][y.y].owner!=pid?1:0);
                Debug.Log("!2 Dijkstra: "+pid+" "+y.x+" "+y.y);
                if(s.LBmap[y.x][y.y].owner!=-1&&s.players[s.LBmap[y.x][y.y].owner].curpos==y)val+=s.players[s.LBmap[y.x][y.y].owner].energy;
                Debug.Log("!3 Dijkstra: "+pid+" "+y.x+" "+y.y);
                if(NodeMap[y.x][y.y].Energy>val){
                    Debug.Log("!4 Dijkstra: "+pid+" "+y.x+" "+y.y);
                    NodeMap[y.x][y.y].Energy=val;
                    Debug.Log("!5 Dijkstra: "+pid+" "+y.x+" "+y.y);
                    NodeMap[y.x][y.y].pre=x;
                    NodeInformation tmp=NodeMap[y.x][y.y];
                    Debug.Log("!6 Dijkstra: "+pid+" "+y.x+" "+y.y);
                    q.Add(tmp);
                    Debug.Log("!7 Dijkstra: "+pid+" "+y.x+" "+y.y);
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
                    if(s.LBmap[i][j].owner==mPlayer.pid&&!s.LBmap[i][j].nearPlayer&&NodeMap[i][j].Dist<=5&&mPlayer.energy>NodeMap[i][j].Energy){
                        pos=new(i,j);
                    }                    
                }
            }
        }
        if(pos.x!=114514)return pos;
        for(int i=0;i<=2*n;++i){
            for(int j=0;j<=2*n;++j){
                if(i-j<=n&&j-i<=n){
                    if(s.LBmap[i][j].owner==mPlayer.pid)continue;
                    bool flag=false;
                    for(int k=0;k<s.PlayerNumber;k++){
                        Vector2Int p=s.PosToCell(s.bornPos[mPlayer.pid]);
                        if(p.x==i&&p.y==j)flag=true;
                    }
                    if(flag)continue;
                    if(s.LBmap[i][j].nearRoot&&CountSetBits((int)s.LBmap[i][j].neighbor)>num){
                        pos=new(i,j);
                        num=CountSetBits((int)s.LBmap[i][j].neighbor);
                    }
                }
            }
        }
        if(num>=3)return pos;
        num=114514;
        for(int i=0;i<=2*n;++i){
            for(int j=0;j<=2*n;++j){
                if(i-j<=n&&j-i<=n){
                    if(s.LBmap[i][j].owner==mPlayer.pid)continue;
                    if(s.LBmap[i][j].owner==-1){
                        if(num>NodeMap[i][j].Dist){
                            pos=new(i,j);
                            num=NodeMap[i][j].Dist;
                        }
                        else if(num==NodeMap[i][j].Dist&&UnityEngine.Random.Range(0f,1f)<1/6f){
                            pos=new(i,j);
                        }
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
            if(s.players[i].energy>NodeMap[p.x][p.y].Energy*2f||(NodeMap[p.x][p.y].Dist<=3&&s.players[i].energy>NodeMap[p.x][p.y].Energy))return -1;
        }
        BFS(mPlayer.curpos,mPlayer.pid);
        double E=mPlayer.energy;
        int x=mPlayer.curpos.x,y=mPlayer.curpos.y;
        for(int i=0;i<s.PlayerNumber;i++){
            if(i==mPlayer.pid)continue;
            Vector2Int p=s.PosToCell(s.bornPos[i]);
            if(E>NodeMap[p.x][p.y].Energy*1.2f)return GetDirection(p);
        }
        if(!s.LBmap[x][y].nearRoot){
            Dijkstra(mPlayer.curpos,mPlayer.pid);
            Vector2Int p1=GetConnect(E);
            Vector2Int p2=GetCapture(E);
            if(p1.x!=114514)Debug.Log(mPlayer.pid+" p1 ::: "+"("+p1.x+","+p1.y+") :: "+NodeMap[p1.x][p1.y].Energy);
            if(p2.x!=114514)Debug.Log(mPlayer.pid+" p2 ::: "+"("+p2.x+","+p2.y+") :: "+NodeMap[p2.x][p2.y].Energy);
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
    void ReturnRoot(int pid){
        Vector2Int p=s.players[pid].curpos;
        if (p != s.PosToCell(s.bornPos[pid]))
        {
            s.LBmap[p.x][p.y].Captured(-1, s.LBmap[p.x][p.y].neighbor, 1);
            s.LBmap[p.x][p.y].neighbor = 0;
        }
        if (s.LBmap[p.x][p.y].owner == -1)
        {
            s.BackHome(pid);
            s.UpdateMap();
        }
    }
    void Update()
    {
        if(ManageGameManager.isPause)return;
        if(Time.time-lastUpdate<s.game_pace*0.5f)return;
        lastUpdate=Time.time;
        int Status=GetDir();
        if(Status==-1)ReturnRoot(mPlayer.pid);
        else mPlayer.TryMove(Status);
    }
}
