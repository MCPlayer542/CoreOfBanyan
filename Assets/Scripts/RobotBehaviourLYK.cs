using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO.Pipes;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class RobotBehaviourLYK : MonoBehaviour
{
    public static GameServer s;
    PlayerBehaviour self;
    int Left = 0, Right = 1, LUp = 2, RUp = 3, LDown = 4, RDown = 5;
    int n;
    double last_move = 0;
    // Start is called before the first frame update
    void Start()
    {
        //s = Camera.main.GetComponent<GameServer>();
        self = GetComponent<PlayerBehaviour>();
        self.isRobot = true;
        n = GameServer.n;
        GameServer.keySet[self.pid] = new(0, 0, 0, 0, 0, 0);
        last_move = Time.time - s.game_pace / GameServer.PlayerNumber * self.pid;
    }

    // Update is called once per frame
    void Update()
    {
        if (ManageGameManager.isPause) return;
        if (Time.time - last_move <= s.game_pace)
            return;
        if (!s.LBmap[self.curpos.x][self.curpos.y].nearRoot)
        {
            self.FastReturn();
            return;
        }
        Vector2Int p = new(0, 0);
        int dis = 114514;
        for (int i = 2 * n; i >= 0; --i)
        {
            for (int j = 2 * n; j >= 0; --j)
            {
                if (i - j > n || j - i > n) continue;
                if (s.LBmap[i][j].owner != self.pid) continue;
                for (int k = 0; k < 6; ++k)
                {
                    var tp = new Vector2Int(i, j) + NeighborPos.Seek[k];
                    if (s.OutOfScreen(tp)) continue;
                    if (s.LBmap[tp.x][tp.y].isWall) continue;
                    if (s.LBmap[tp.x][tp.y].owner == self.pid && s.LBmap[tp.x][tp.y].nearPlayer) continue;
                    var tdis = Calc_dis(self.curpos, tp);
                    if (tdis >= 110000) continue;
                    if (!MCmp(p, tp, dis, tdis))
                    {
                        dis = tdis;
                        p = tp;
                    }
                }
            }
        }
        int dir = 0;
        for (int i = 0; i < 6; ++i)
        {
            var taim = self.curpos + NeighborPos.Seek[i];
            if (s.OutOfScreen(taim)) continue;
            if (s.LBmap[taim.x][taim.y].owner == self.pid && s.LBmap[taim.x][taim.y].nearPlayer)
                if (((int)s.LBmap[self.curpos.x][self.curpos.y].neighbor & (1 << i)) == 0)
                    continue;
            if (Calc_dis(self.curpos, p) == Calc_dis(taim, p) + 1)
            {
                dir = i;
            }
        }
        //Debug.Log(p);
        //Debug.Log(self.curpos);
        // Debug.Log(s.LBmap[self.curpos.x][self.curpos.y].owner);
        //Debug.Log(Calc_dis(self.curpos, p));
        // Debug.Log(Calc_dis(p,self.curpos+NeighborPos.Seek[dir]));
        if (self.TryMove(dir))
            last_move = Time.time;
    }
    Queue<Vector2Int> q = new();
    Dictionary<Vector2Int, int> dis = new();
    int Calc_dis(Vector2Int a, Vector2Int b)
    {
        if (a == b) return 0;
        if (s.LBmap[a.x][a.y].owner != self.pid) return 114514;
        q.Clear();
        q.Enqueue(a);
        dis.Clear();
        dis.Add(a, 0);
        while (q.Count != 0)
        {
            var cur = q.Dequeue();
            for (int i = 0; i < 6; ++i)
            {
                var nxt = cur + NeighborPos.Seek[i];
                if (dis.ContainsKey(nxt)) continue;
                if (s.OutOfScreen(nxt)) continue;
                var nxtb = s.LBmap[nxt.x][nxt.y];
                if (nxt == b) return dis[cur] + 1;
                if (nxtb.owner == self.pid)
                {
                    if (((int)s.LBmap[cur.x][cur.y].neighbor & (1 << i)) == 0) continue;
                    dis[nxt] = dis[cur] + 1;
                    q.Enqueue(nxt);
                }
            }
        }
        return 114514;
    }

    double Value(Vector2Int p, int dis)
    {
        var lb = s.LBmap[p.x][p.y];
        if (lb.owner == self.pid)
        {
            if (lb.nearPlayer) return -114514;
            else return 114514 + lb.hp;
        }
        return -10 * dis - lb.hp;
    }
    bool MCmp(Vector2Int p, Vector2Int tp, int dis, int tdis)
    {
        return Value(p, dis) > Value(tp, tdis);
        // return tdis<dis||(tdis==dis&&s.LBmap[tp.x][tp.y].hp<s.LBmap[p.x][p.y].hp);
    }
    /*void ReturnRoot(int pid){
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
    }*/
}
