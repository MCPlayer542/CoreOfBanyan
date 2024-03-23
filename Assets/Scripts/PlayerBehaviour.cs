using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Collections;
//using UnityEditor.Build.Content;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

class OPQ
{
    private List<Vector2Int> data;
    private int head, tail;
    public void Clear()
    {
        data.Clear();
        head = 0;
        tail = 0;
    }
    public void PushBack(Vector2Int x)
    {
        data.Add(x);
        ++tail;
    }
    public Vector2Int PopFront()
    {
        if (head == tail) return new(0, 0);
        return data[head++];
    }
    public bool Empty()
    {
        return head == tail;
    }
    public OPQ()
    {
        data = new();
    }
}

public class PlayerBehaviour : MonoBehaviour
{
    public bool Movable;
    public int pid;
    public float speed;
    public double energy;
    public Vector2Int curpos;
    public static GameServer s;
    private OPQ opQueue;
    // Start is called before the first frame update
    void Start()
    {
        opQueue = new();
        transform.position = s.bornPos[pid];
        speed = 3.0f;
        curpos = s.PosToCell(s.bornPos[pid]);
        LandBehaviour bornLand = s.map[curpos.x][curpos.y].GetComponent<LandBehaviour>();
        energy = 114514; //big enough
        TryCapture(bornLand, bornLand, curpos, curpos);
        s.LBmap[curpos.x][curpos.y].hp = 50;
        s.LBmap[curpos.x][curpos.y].nearPlayer = true;
        s.LBmap[curpos.x][curpos.y].nearRoot = true;
        energy = 3;
    }
    private const float S3_2 = 0.8660254f;
    // Update is called once per frame
    void Update()
    {
        if (s.GameOverFlag) return;

        if (Input.GetKeyDown(s.keySet[pid].Back) && curpos != s.PosToCell(s.bornPos[pid]))
        {
            s.LBmap[curpos.x][curpos.y].Captured(-1, s.LBmap[curpos.x][curpos.y].neighbor, 1);
            s.LBmap[curpos.x][curpos.y].neighbor = 0;
        }
        if (s.LBmap[curpos.x][curpos.y].owner == -1)
        {
            s.BackHome(pid);
            s.UpdateMap();
            opQueue.Clear();
        }

        if (Input.GetKeyDown(s.keySet[pid].Reinforce))
        {
            Reinforce();
            opQueue.Clear();
        }
        if(Input.GetKey(s.keySet[pid].Left)&&Input.GetKey(s.keySet[pid].Up)&&!s.OutOfScreen(curpos+NeighborPos.LUp))
            opQueue.PushBack(NeighborPos.LUp);
        else if(Input.GetKey(s.keySet[pid].Left)&&Input.GetKey(s.keySet[pid].Down)&&!s.OutOfScreen(curpos+NeighborPos.LDown))
            opQueue.PushBack(NeighborPos.LDown);
        else if(Input.GetKey(s.keySet[pid].Right)&&Input.GetKey(s.keySet[pid].Up)&&!s.OutOfScreen(curpos+NeighborPos.RUp))
            opQueue.PushBack(NeighborPos.RUp);
        else if(Input.GetKey(s.keySet[pid].Right)&&Input.GetKey(s.keySet[pid].Down)&&!s.OutOfScreen(curpos+NeighborPos.RDown))
            opQueue.PushBack(NeighborPos.RDown);
        else if(Input.GetKey(s.keySet[pid].Left)&&!s.OutOfScreen(curpos+NeighborPos.Left))
            opQueue.PushBack(NeighborPos.Left);
        else if(Input.GetKey(s.keySet[pid].Right)&&!s.OutOfScreen(curpos+NeighborPos.Right))
            opQueue.PushBack(NeighborPos.Right);
        /*
        if(Input.GetKey(s.keySet[pid].Left) && !s.OutOfScreen(curpos+NeighborPos.Left))
            opQueue.PushBack(NeighborPos.Left);
        else if (Input.GetKey(s.keySet[pid].Right) && !s.OutOfScreen(curpos + NeighborPos.Right))
            opQueue.PushBack(NeighborPos.Right);
        else if (Input.GetKey(s.keySet[pid].LUp) && !s.OutOfScreen(curpos + NeighborPos.LUp))
            opQueue.PushBack(NeighborPos.LUp);
        else if (Input.GetKey(s.keySet[pid].RUp) && !s.OutOfScreen(curpos + NeighborPos.RUp))
            opQueue.PushBack(NeighborPos.RUp);
        else if (Input.GetKey(s.keySet[pid].LDown) && !s.OutOfScreen(curpos + NeighborPos.LDown))
            opQueue.PushBack(NeighborPos.LDown);
        else if (Input.GetKey(s.keySet[pid].RDown) && !s.OutOfScreen(curpos + NeighborPos.RDown))
            opQueue.PushBack(NeighborPos.RDown);
        */

        if (!Movable)
        {
            opQueue.Clear();
            return;
        }
        if (opQueue.Empty()) return;
        Movable = false;

        Vector2Int pre = curpos, cur = curpos + opQueue.PopFront();
        Vector3 p = s.LBmap[cur.x][cur.y].transform.position;
        p.z = -4;
        if (pre == cur)
        {
            transform.position = p;
            return;
        }
        LandBehaviour preLand = s.map[pre.x][pre.y].GetComponent<LandBehaviour>();
        LandBehaviour curLand = s.map[cur.x][cur.y].GetComponent<LandBehaviour>();
        if (curLand.owner == pid)
        {
            if (curLand.nearPlayer)
            {
                if (!IsNeighbor(curLand, preLand, cur, pre))
                    return;
            }
            else
            {
                if (!TryConnect(curLand, preLand, cur, pre)) return;
            }
        }
        else
        {
            if (!TryCapture(curLand, preLand, cur, pre)) return;
            Vector2Int p1 = new(0, 0), p2 = new(2 * s.n, 2 * s.n);
            if (cur == p1 || cur == p2) s.GameOverFlag = true;
        }


        if (!Conflict(cur)) return;
        transform.position = p;
        curpos = cur;
        s.UpdateMap();

        if (s.LBmap[cur.x][cur.y].mFruit != null)
        {
            Destroy(s.LBmap[cur.x][cur.y].mFruit);
            energy += s.LBmap[cur.x][cur.y].GetFruitsEnergy();
            s.LBmap[cur.x][cur.y].mFruit = null;
        }
        if (s.LBmap[cur.x][cur.y].mPest != null)
        {
            Destroy(s.LBmap[cur.x][cur.y].mPest);
            energy -= s.LBmap[cur.x][cur.y].GetPestsEnergy();
            s.LBmap[cur.x][cur.y].mPest = null;
        }
    }
    bool TryConnect(LandBehaviour curLand, LandBehaviour preLand, Vector2Int cur, Vector2Int pre)
    {
        if (cur + NeighborPos.RUp == pre)
        {
            curLand.neighbor |= Neighbor.RUp;
            preLand.neighbor |= Neighbor.LDown;
        }
        else if (cur + NeighborPos.LDown == pre)
        {
            curLand.neighbor |= Neighbor.LDown;
            preLand.neighbor |= Neighbor.RUp;
        }
        else if (cur + NeighborPos.RDown == pre)
        {
            curLand.neighbor |= Neighbor.RDown;
            preLand.neighbor |= Neighbor.LUp;
        }
        else if (cur + NeighborPos.LUp == pre)
        {
            curLand.neighbor |= Neighbor.LUp;
            preLand.neighbor |= Neighbor.RDown;
        }
        else if (cur + NeighborPos.Right == pre)
        {
            curLand.neighbor |= Neighbor.Right;
            preLand.neighbor |= Neighbor.Left;
        }
        else if (cur + NeighborPos.Left == pre)
        {
            curLand.neighbor |= Neighbor.Left;
            preLand.neighbor |= Neighbor.Right;
        }
        else if (cur == pre)
        {

        }
        else
        {
            return false;
        }
        s.UpdateMap();
        curLand.ChangeImg();
        preLand.ChangeImg();
        return true;
    }
    bool IsNeighbor(LandBehaviour a, LandBehaviour b, Vector2Int pa, Vector2Int pb)
    {
        if (pa + NeighborPos.Left == pb && (a.neighbor & Neighbor.Left) != 0) return true;
        if (pa + NeighborPos.Right == pb && (a.neighbor & Neighbor.Right) != 0) return true;
        if (pa + NeighborPos.LUp == pb && (a.neighbor & Neighbor.LUp) != 0) return true;
        if (pa + NeighborPos.LDown == pb && (a.neighbor & Neighbor.LDown) != 0) return true;
        if (pa + NeighborPos.RUp == pb && (a.neighbor & Neighbor.RUp) != 0) return true;
        if (pa + NeighborPos.RDown == pb && (a.neighbor & Neighbor.RDown) != 0) return true;
        return false;
    }
    bool TryCapture(LandBehaviour curLand, LandBehaviour preLand, Vector2Int cur, Vector2Int pre)
    {
        if (energy < curLand.hp) return false;
        if (curLand.owner != -1 && s.players[curLand.owner].curpos == cur && energy <= s.players[curLand.owner].energy + curLand.hp)
            return false;
        Neighbor tmp = curLand.neighbor;
        curLand.neighbor = 0;
        if (!TryConnect(curLand, preLand, cur, pre))
        {
            curLand.neighbor = tmp;
            return false;
        }
        energy -= curLand.hp;
        curLand.Captured(pid, tmp, 5);
        preLand.ChangeImg();
        return true;
    }
    bool Conflict(Vector2Int cur)
    {
        //Debug.Log("shit " + curpos + " " + s.players[1 - pid].curpos);
        for (int i = 0; i < s.PlayerNumber; ++i)
        {
            if (i != pid && cur == s.players[i].curpos)
            {
                if (s.players[i].energy < energy)
                {
                    energy -= s.players[i].energy;
                    s.players[i].energy = 0;
                    s.BackHome(i);
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }
    void Reinforce()
    {
        float amount = (float)energy * 0.025f;
        energy *= 0.9f;
        s.LBmap[curpos.x][curpos.y].hp += amount;
        for (int i = 0, n = (int)s.LBmap[curpos.x][curpos.y].neighbor; i < 6; ++i)
            if ((n >> i & 1) == 1)
                s.LBmap[curpos.x + NeighborPos.Seek[i].x][curpos.y + NeighborPos.Seek[i].y].hp += amount;
    }
    public void EndGame()
    {
        Destroy(transform.gameObject);
    }
}
