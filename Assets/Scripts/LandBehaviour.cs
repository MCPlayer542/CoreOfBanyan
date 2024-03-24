using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;

public enum Neighbor
{
    Left = 1,
    Right = 2,
    LUp = 4,
    RUp = 8,
    LDown = 16,
    RDown = 32,
    None = 0
};
public class NeighborPos
{
    static public Vector2Int Left = new(-1, -1), Right = new(1, 1), RUp = new(1, 0), LDown = new(-1, 0), RDown = new(0, 1), LUp = new(0, -1);
    static public List<Vector2Int> Seek = new() { new(-1, -1), new(1, 1), new(0, -1), new(1, 0), new(-1, 0), new(0, 1) };
}
public class LandBehaviour : MonoBehaviour
{
    const float k3 = 0.5f;
    public static GameServer s;
    public List<Color> colors;
    // Start is called before the first frame update
    public int owner;
    public double hp;
    public Neighbor neighbor;
    public bool nearPlayer, nearRoot;
    public GameObject mPest = null;
    public GameObject mFruit = null;

    void Awake()
    {
        owner = -1;
        hp = 1;
        colors = new() { Color.green, Color.red };
        neighbor = 0;
    }

    // Update is called once per frame
    readonly float Constant1 = 0.2f, Constant2 = 0.5f, Constant3 = 0.5f;
    void Update()
    {
        if (owner == -1) return;
        if (nearRoot && nearPlayer) s.players[owner].energy += k3 * Time.smoothDeltaTime;
        if (nearRoot) hp += Constant1 * Time.smoothDeltaTime;
        else hp -= Constant2 * Time.smoothDeltaTime;
        if (mPest != null) hp -= Constant3 * Time.smoothDeltaTime;

        if (hp <= 1)
        {
            Captured(-1, neighbor, 1);
            neighbor = 0;
        }

        Anchoring();
    }

    public void Captured(int new_owner, Neighbor new_neighbor, float new_hp)
    {
        var cur = s.PosToCell(transform.position);
        if (owner != -1) s.ChangeNeighborOfNeighbor(cur.x, cur.y, new_neighbor);
        owner = new_owner;
        Destroy(mPest);
        Destroy(mFruit);
        mPest = null;
        mFruit = null;
        hp = new_hp;
        ChangeImg();
        s.UpdateMap();
    }
    public void ChangeImg()
    {
        if (owner == -1) transform.GetChild(6).GetComponent<SpriteRenderer>().color = Color.white;
        else transform.GetChild(6).GetComponent<SpriteRenderer>().color = colors[owner];
        for (int i = 0; i < 6; ++i)
        {
            SpriteRenderer s = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (((int)neighbor & (1 << i)) == 0)
                s.color = Color.white;
            else
            {
                if (owner == -1) s.color = Color.white;
                else s.color = colors[owner];
            }
        }
    }
    public float GetFruitsEnergy() { return mFruit.GetComponent<FruitBehavior>().getEnergy(); }
    public float GetPestsEnergy() { return 0; }


    Dictionary<Neighbor, int> dict = new() { { Neighbor.Left, 3 }, { Neighbor.Right, 0 }, { Neighbor.LUp, 4 }, { Neighbor.RUp, 5 }, { Neighbor.LDown, 2 }, { Neighbor.RDown, 1 } };


    GameObject anchorObject = null;

    void Anchoring()
    {
        var t = anchorObject;
        anchorObject = null;
        Destroy(t);
        foreach (var i in s.players)
        {
            if (Time.time - i.last_move > s.game_pace * 0.7 && i.GetComponent<PlayerBehaviour>().curpos == s.PosToCell(transform.position) && s.keySet[i.pid].isKeyDown())
            {
                var a = i.anchoring;
                if (a == Neighbor.None) break;
                var g = Instantiate(Resources.Load("Neighbor") as GameObject);
                Color arrowColor=colors[i.pid];
                arrowColor*=0.6f;
                arrowColor.a=1f;
                g.GetComponent<SpriteRenderer>().color = arrowColor;
                var p = transform.localPosition;
                p.z = -4;
                g.transform.localPosition = p;
                g.transform.localScale = transform.localScale;
                g.transform.Rotate(new Vector3(0, 0, -1) * dict[a] * 60);
                anchorObject = g;
                break;
            }
        }

    }
    public void EndGame()
    {
        if (mFruit != null) Destroy(mFruit);
        if (mPest != null) Destroy(mPest);
        Destroy(transform.gameObject);
        if (anchorObject != null)
        {
            Destroy(anchorObject);
        }
    }
}
