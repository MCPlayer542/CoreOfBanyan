using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO.Pipes;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class RobotBehaviourLYK : MonoBehaviour
{
    GameServer s;
    PlayerBehaviour self;
    int Left=0,Right=1,LUp=2,RUp=3,LDown=4,RDown=5;
    int n;
    // Start is called before the first frame update
    void Start()
    {
        s = Camera.main.GetComponent<GameServer>();
        self = GetComponent<PlayerBehaviour>();
        n = GameServer.n;
    }

    // Update is called once per frame
    float last_move = 0;
    void Update()
    {
        if (Time.time - last_move <= s.game_pace)
            return;
        if(!s.LBmap[self.curpos.x][self.curpos.y].nearRoot){
            s.BackHome(self.pid);
            return;
        }
        Vector2Int p = new(0,0);
        int dis = 114514;
        for(int i=2*n;i>=0;--i){
            for(int j=2*n;j>=0;--j){
                if(i-j>n||j-i>n) continue;
                if(s.LBmap[i][j].owner!=self.pid) continue;
                for(int k=0;k<6;++k){
                    var tp = new Vector2Int(i,j)+NeighborPos.Seek[k];
                    if(s.OutOfScreen(tp)) continue;
                    if(s.LBmap[tp.x][tp.y].isWall) continue;
                    if(s.LBmap[tp.x][tp.y].owner==self.pid) continue;
                    var tdis = Calc_dis(self.curpos,tp);
                    if(tdis<dis||(tdis==dis&&s.LBmap[tp.x][tp.y].hp<s.LBmap[p.x][p.y].hp)){
                        dis=tdis;
                        p=tp;
                    }
                }
            }
        }
        int dir = 0;
        for(int i=0;i<6;++i){
            var taim = self.curpos+NeighborPos.Seek[i];
            if(s.OutOfScreen(taim)) continue;
            if(s.LBmap[taim.x][taim.y].owner == self.pid)
                if(((int)s.LBmap[self.curpos.x][self.curpos.y].neighbor&(1<<i))==0)
                    continue;
            if(Calc_dis(self.curpos,p)==Calc_dis(taim,p)+1){
                dir = i;
            }
        }
        Debug.Log(p);
        Debug.Log(self.curpos);
        // Debug.Log(s.LBmap[self.curpos.x][self.curpos.y].owner);
        Debug.Log(Calc_dis(self.curpos,p));
        // Debug.Log(Calc_dis(p,self.curpos+NeighborPos.Seek[dir]));
        if(self.TryMove(dir))
            last_move = Time.time;
    }
    Queue<Vector2Int> q = new();
    Dictionary<Vector2Int,int> dis = new();
    int Calc_dis(Vector2Int a, Vector2Int b){
        if(a==b) return 0;
        if(s.LBmap[a.x][a.y].owner!=self.pid) return 114514;
        q.Clear();
        q.Enqueue(a);
        dis.Clear();
        dis.Add(a,0);
        while(q.Count!=0){
            var cur = q.Dequeue();
            for(int i=0;i<6;++i){
                var nxt = cur + NeighborPos.Seek[i];
                if(dis.ContainsKey(nxt)) continue;
                if(s.OutOfScreen(nxt)) continue;
                var nxtb = s.LBmap[nxt.x][nxt.y];
                if(nxtb.owner != self.pid){
                    if(nxt==b) return dis[cur]+1;
                }
                else{
                    if(((int)nxtb.neighbor&(1<<i))==0)  continue;
                    dis[nxt]=dis[cur]+1;
                    q.Enqueue(nxt);
                }
            }
        }
        return 114514;
    }
}
