using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

//using UnityEditor.Experimental.GraphView;
//using UnityEditorInternal;
using UnityEngine;

public class NodeInformation : IComparable<NodeInformation>
{
    public Vector2Int cur;
    public Vector2Int pre;
    public int Dist;
    public double Energy;
    public bool vis;
    public void Init()
    {
        cur.x = 0; cur.y = 0;
        pre.x = 0; pre.y = 0;
        Dist = 114514;
        Energy = 1e6;
        vis = false;
    }
    public int CompareTo(NodeInformation other)
    {
        if (other == null) return 1;
        return Energy.CompareTo(other.Energy);
    }
};
public class RobotBehaviourHJQ : MonoBehaviour
{
    int n;
    int pid;
    public static GameServer s;
    PlayerBehaviour mPlayer;
    public List<Vector2Int> targetList = new();
    public List<List<NodeInformation>> NodeMap = new();
    public List<Vector2Int> mSeek = new() { new(1, 1), new(1, 0), new(0, 1), new(-1, -1), new(0, -1), new(-1, 0) };
    float lastUpdate;
    float reinforceProbability = 0.1f;
    float cutProbability = 0.5f;
    int CountSetBits(int n)
    {
        int count = 0;
        while (n > 0)
        {
            count += n & 1;
            n >>= 1;
        }
        return count;
    }
    void Start()
    {
        n = GameServer.n;
        //s=Camera.main.GetComponent<GameServer>();
        mPlayer = gameObject.GetComponent<PlayerBehaviour>();
        mPlayer.isRobot=true;
        for (int i = 0; i <= 2 * n; ++i)
        {
            NodeMap.Add(new List<NodeInformation>());
            for (int j = 0; j <= 2 * n; ++j)
            {
                if (i - j <= n && j - i <= n)
                {
                    NodeMap[i].Add(new NodeInformation());
                }
                else NodeMap[i].Add(null);
            }
        }
        pid = mPlayer.pid;
        lastUpdate = Time.time + s.game_pace / s.PlayerNumber * pid;
    }
    void InitNodeMap()
    {
        for (int i = 0; i <= 2 * n; ++i)
        {
            for (int j = 0; j <= 2 * n; ++j)
            {
                if (i - j <= n && j - i <= n)
                {
                    NodeMap[i][j].Init();
                    NodeMap[i][j].cur.x = i;
                    NodeMap[i][j].cur.y = j;
                }
            }
        }
    }
    public OPQ q = new();
    void BFS(Vector2Int St, int pid)
    {
        InitNodeMap();
        q.Clear();
        q.PushBack(St);
        NodeMap[St.x][St.y].vis = true;
        NodeMap[St.x][St.y].Energy = 0;
        NodeMap[St.x][St.y].Dist = 0;
        while (q.Size() != 0)
        {
            Vector2Int x = q.PopFront();
            foreach (var dlt in mSeek)
            {
                Vector2Int y = x + dlt;
                if (s.OutOfScreen(y)) continue;
                if (s.LBmap[y.x][y.y].nearPlayer && s.LBmap[y.x][y.y].owner == pid && !mPlayer.IsNeighbor(s.LBmap[x.x][x.y], s.LBmap[y.x][y.y], x, y)) continue;
                if (!NodeMap[y.x][y.y].vis)
                {
                    NodeMap[y.x][y.y].vis = true;
                    NodeMap[y.x][y.y].Dist = NodeMap[x.x][x.y].Dist + 1;
                    NodeMap[y.x][y.y].pre = x;
                    q.PushBack(y);
                }
                if (NodeMap[y.x][y.y].Dist == NodeMap[x.x][x.y].Dist + 1)
                {
                    double val = NodeMap[x.x][x.y].Energy + Math.Max(s.LBmap[y.x][y.y].hp, 1.0f) * (s.LBmap[y.x][y.y].owner != pid ? 1 : 0);
                    if (s.LBmap[y.x][y.y].owner != -1 && s.players[s.LBmap[y.x][y.y].owner].curpos == y) val += s.players[s.LBmap[y.x][y.y].owner].energy;
                    if (s.LBmap[y.x][y.y].owner == pid && s.LBmap[y.x][y.y].mFruit != null) val -= s.LBmap[y.x][y.y].GetFruitsEnergy();
                    if (val < NodeMap[y.x][y.y].Energy)
                    {
                        NodeMap[y.x][y.y].Energy = val;
                        NodeMap[y.x][y.y].pre = x;
                    }
                }
            }
        }
    }
    Vector2Int GetConnect(double E)
    {
        Vector2Int pos = new(114514, 114514);
        int dis = E < 5 ? 1 : n / 3*2;
        for (int i = 0; i <= 2 * n; ++i)
        {
            for (int j = 0; j <= 2 * n; ++j)
            {
                if (i - j <= n && j - i <= n)
                {
                    if (!s.LBmap[i][j].nearRoot || s.LBmap[i][j].owner != pid) continue;
                    if (NodeMap[i][j].Dist < dis && NodeMap[i][j].Energy * 1.2 < E)
                    {
                        dis = NodeMap[i][j].Dist;
                        pos.x = i; pos.y = j;
                    }
                }
            }
        }
        return pos;
    }
    int GetConnect2()
    {
        Vector2Int p = new(114514, 114514);
        foreach (var dlt in NeighborPos.Seek)
        {
            Vector2Int t = mPlayer.curpos + dlt;
            if (s.OutOfScreen(t)) continue;
            if (s.LBmap[t.x][t.y].owner == pid && s.LBmap[t.x][t.y].nearRoot != s.LBmap[mPlayer.curpos.x][mPlayer.curpos.y].nearRoot){
                p = t;
            }
        }
        if (p.x == 114514) return -1;
        return GetDirection(p);
    }
    Vector2Int GetCapture(double E)
    {
        Vector2Int pos = new(114514, 114514);
        int dis = E < 5 ? 1 : n / 3;
        for (int i = 0; i <= 2 * n; ++i)
        {
            for (int j = 0; j <= 2 * n; ++j)
            {
                if (i - j <= n && j - i <= n)
                {
                    if (!s.LBmap[i][j].nearRoot || s.LBmap[i][j].owner == pid) continue;
                    if (NodeMap[i][j].Dist <= dis && NodeMap[i][j].Energy * 2f < E)
                    {
                        dis = NodeMap[i][j].Dist;
                        pos.x = i; pos.y = j;
                    }
                }
            }
        }
        return pos;
    }
    Vector2Int GetNearestBlank(int num)
    {
        Vector2Int pos = new(114514, 114514);
        for (int i = 0; i <= 2 * n; ++i)
        {
            for (int j = 0; j <= 2 * n; ++j)
            {
                if (i - j <= n && j - i <= n)
                {
                    if (s.LBmap[i][j].owner == -1)
                    {
                        if (num > NodeMap[i][j].Dist)
                        {
                            pos.x = i; pos.y = j;
                            num = NodeMap[i][j].Dist;
                        }
                        else if (num == NodeMap[i][j].Dist && (pos.x == 114514 || UnityEngine.Random.Range(0f, 1f) < 0.33f))
                        {//Magic Number
                            pos.x = i; pos.y = j;
                        }
                    }
                }
            }
        }
        return pos;
    }
    Vector2Int GetTarget()
    {
        Vector2Int pos = new(114514, 114514);
        int num = 0;
        for (int i = 0; i <= 2 * n; ++i)
        {
            for (int j = 0; j <= 2 * n; ++j)
            {
                if (i - j <= n && j - i <= n)
                {
                    if (s.LBmap[i][j].owner == pid && !s.LBmap[i][j].nearPlayer && NodeMap[i][j].Dist <= n && mPlayer.energy > NodeMap[i][j].Energy * 1.2f)
                    {
                        pos.x = i; pos.y = j;
                    }
                }
            }
        }
        if (pos.x != 114514) return pos;

        pos = GetNearestBlank(2);
        if (pos.x != 114514) return pos;

        for (int i = 0; i <= 2 * n; ++i)
        {
            for (int j = 0; j <= 2 * n; ++j)
            {
                if (i - j <= n && j - i <= n)
                {
                    if (s.LBmap[i][j].owner == pid) continue;
                    bool flag = false;
                    for (int k = 0; k < s.PlayerNumber; k++)
                    {
                        Vector2Int p = s.PosToCell(s.bornPos[pid]);
                        if (p.x == i && p.y == j) flag = true;
                    }
                    if (flag) continue;
                    if (s.LBmap[i][j].nearRoot && CountSetBits((int)s.LBmap[i][j].neighbor) > num)
                    {
                        pos.x = i; pos.y = j;
                        num = CountSetBits((int)s.LBmap[i][j].neighbor);
                    }
                }
            }
        }
        if (num >= 3) return pos;

        pos = GetNearestBlank(114514);
        return pos;
    }
    int GetDirection(Vector2Int p)
    {
        Vector2Int pnow = mPlayer.curpos;
        while (NodeMap[p.x][p.y].pre != pnow) p = NodeMap[p.x][p.y].pre;
        if (pnow + NeighborPos.Left == p) return (int)DirID.LeftID;
        if (pnow + NeighborPos.Right == p) return (int)DirID.RightID;
        if (pnow + NeighborPos.LUp == p) return (int)DirID.LUpID;
        if (pnow + NeighborPos.LDown == p) return (int)DirID.LDownID;
        if (pnow + NeighborPos.RUp == p) return (int)DirID.RUpID;
        if (pnow + NeighborPos.RDown == p) return (int)DirID.RDownID;
        return -1;
    }
    Vector2Int TryCut()
    {
        Vector2Int p = new(114514, 114514);
        for (int i = 0; i <= 2 * n; ++i)
        {
            for (int j = 0; j <= 2 * n; ++j)
            {
                if (i - j <= n && j - i <= n)
                {
                    if (s.LBmap[i][j].owner == pid) continue;
                    if (s.LBmap[i][j].owner != -1 && s.LBmap[i][j].nearRoot && NodeMap[i][j].Dist <= 4 && NodeMap[i][j].Energy * 1.2f <= mPlayer.energy)
                    {
                        p.x = i; p.y = j;
                    }
                }
            }
        }
        return p;
    }
    int TryCut2()
    {
        Vector2Int p = new(114514, 114514);
        int num = 3;
        foreach (var dlt in NeighborPos.Seek)
        {
            Vector2Int t = mPlayer.curpos + dlt;
            if (s.OutOfScreen(t)) continue;
            int deg = CountSetBits((int)s.LBmap[t.x][t.y].neighbor);
            if (s.LBmap[t.x][t.y].owner != -1 && s.LBmap[t.x][t.y].owner != pid && s.LBmap[t.x][t.y].nearRoot && deg >= num && mPlayer.energy > s.LBmap[t.x][t.y].hp)
            {
                num = deg;
                p = t;
            }
        }
        if (p.x == 114514) return -1;
        return GetDirection(p);
    }

    int IfFaceDanger()
    {
        Vector2Int bp = s.PosToCell(s.bornPos[pid]);
        if (mPlayer.energy > 10 && NodeMap[bp.x][bp.y].Dist <= 1) mPlayer.Reinforce();
        for (int i = 0; i < s.PlayerNumber; ++i)
        {
            if (i == pid) continue;
            Vector2Int p = s.players[i].curpos;
            int dis1=Math.Abs(bp.x-p.x)+Math.Abs(bp.y-p.y);
            int dis2=Math.Abs(mPlayer.curpos.x-bp.x)+Math.Abs(mPlayer.curpos.y-bp.y);
            if (dis1<= 2 && s.players[i].energy > s.LBmap[bp.x][bp.y].hp)
            {
                if(mPlayer.curpos==bp)return GetDirection(p);
                if(dis2<=2)return GetDirection(bp);
                return -1;
            }
        }
        return 114514;
    }

    int GetDir()
    {
        BFS(mPlayer.curpos, pid);
        int res = IfFaceDanger();
        if (res != 114514) return res;

        res = TryCut2();
        if (res != -1) return res;

        res = GetConnect2();
        if (res != -1) return res;

        double easyTargetEnergy = 1e6;
        double E = mPlayer.energy;
        int x = mPlayer.curpos.x, y = mPlayer.curpos.y, targetID = -1;
        for (int i = 0; i < s.PlayerNumber; i++)
        {
            if (i == pid) continue;
            if(!s.players[i].alive)continue;
            Vector2Int p = s.PosToCell(s.bornPos[i]);
            bool NearRoot = s.LBmap[mPlayer.curpos.x][mPlayer.curpos.y].nearRoot;
            double eNeed=s.players[i].energy+NodeMap[p.x][p.y].Energy;
            if ((!NearRoot && E > eNeed * 1.5f) || (NearRoot && E > eNeed))
            {
                if(Math.Max(x-p.x,y-p.y)<=n/2)return GetDirection(s.PosToCell(s.bornPos[i]));
                if (easyTargetEnergy >= NodeMap[p.x][p.y].Energy)
                {
                    easyTargetEnergy = NodeMap[p.x][p.y].Energy;
                    targetID = i;
                }
            }
        }
        if (targetID != -1) return GetDirection(s.PosToCell(s.bornPos[targetID]));

        Vector2Int bp = s.PosToCell(s.bornPos[pid]);
        if (!s.LBmap[x][y].nearRoot)
        {
            Vector2Int p1 = GetConnect(E);
            Vector2Int p2 = GetCapture(E);
            if (p1.x == 114514 && p2.x == 114514) return -1;
            if (p1.x == 114514) return GetDirection(p2);
            if (p2.x == 114514) return GetDirection(p1);
            if (NodeMap[p1.x][p1.y].Dist < NodeMap[p2.x][p2.y].Dist) return GetDirection(p1);
            if (Math.Max(p2.x-bp.x,p2.y-bp.y)<n)return GetDirection(p2);
            return -1;
        }
        else
        {
            if (mPlayer.energy > 20 && UnityEngine.Random.Range(0f, 1f) < reinforceProbability) mPlayer.Reinforce();

            Vector2Int cutP = TryCut();
            if (cutP.x != 114514 && (Math.Max(cutP.x-bp.x,cutP.y-bp.y)<=n/2||UnityEngine.Random.Range(0f, 1f) < cutProbability)) return GetDirection(cutP);

            Vector2Int p = GetTarget();
            if (p.x == 114514) return -1;
            return GetDirection(p);
        }
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
    void Update()
    {
        if (ManageGameManager.isPause) return;
        if (Time.time - lastUpdate < s.game_pace * 1.1f) return;
        lastUpdate = Time.time;
        int Status = GetDir();
        if (Status == -1) mPlayer.FastReturn();
        else mPlayer.TryMove(Status);
    }
}
