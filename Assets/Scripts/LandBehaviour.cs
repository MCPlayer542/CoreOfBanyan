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
public enum DirID { LeftID, RightID, LUpID, RUpID, LDownID, RDownID };
public class NeighborPos
{
    static public Vector2Int Left = new(-1, -1), Right = new(1, 1), RUp = new(1, 0), LDown = new(-1, 0), RDown = new(0, 1), LUp = new(0, -1);
    static public List<Vector2Int> Seek = new() { new(-1, -1), new(1, 1), new(0, -1), new(1, 0), new(-1, 0), new(0, 1) };
}
public class LandBehaviour : MonoBehaviour
{
    const double k3 = 0.5f;
    public const double blink_time = 0.5f;
    public static GameServer s;
    // Start is called before the first frame update
    public int owner;
    public double hp;
    public Neighbor neighbor;
    public bool nearPlayer, nearRoot;
    public GameObject mPest = null;
    public GameObject mFruit = null;
    public bool isRoot = false;
    public bool isWall = false;
    public AudioSource capture_root;
    public double last_reinforce;

    void Awake()
    {
        owner = -1;
        hp = 1;
        neighbor = 0;
        last_reinforce = -blink_time;
        UpdateVolume();
    }

    // Update is called once per frame
    readonly double Constant1 = 0.2f, Constant2 = 0.5f, Constant3 = 0.5f;
    void Update()
    {
        if (ManageGameManager.isPause) return;
        if (isWall) return;
        Anchoring();
        if (owner == -1) return;
        if (nearRoot && nearPlayer) s.players[owner].energy += k3 * Time.smoothDeltaTime * (1 + s.players[owner].KillCount / 3f);
        if (nearRoot) hp += Constant1 * Time.smoothDeltaTime * (1 + s.players[owner].KillCount / 3f);
        else hp -= Constant2 * Time.smoothDeltaTime;
        if (mPest != null) hp -= Constant3 * Time.smoothDeltaTime;

        if (hp <= 1)
        {
            Captured(-1, neighbor, 1);
            neighbor = 0;
        }
    }

    public void Captured(int new_owner, Neighbor new_neighbor, double new_hp)
    {
        if (isRoot) capture_root.Play();
        var cur = s.PosToCell(transform.position);
        if (owner != -1) s.ChangeNeighborOfNeighbor(cur.x, cur.y, new_neighbor);
        if (mPest != null) s.players[owner].PestNumber--;
        Destroy(mPest);
        Destroy(mFruit);
        mPest = null;
        mFruit = null;
        owner = new_owner;
        hp = new_hp;
        ChangeImg();
        s.UpdateMap();
    }
    public void ChangeImg()
    {
        if (owner == -1) transform.GetChild(6).GetComponent<SpriteRenderer>().color = Color.white;
        else
        {
            Color PointColor = s.colors[owner];
            if (isRoot)
            {
                PointColor *= 0.8f;
                PointColor.a = 1f;
            }
            transform.GetChild(6).GetComponent<SpriteRenderer>().color = PointColor;
        }
        for (int i = 0; i < 6; ++i)
        {
            SpriteRenderer SR = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (((int)neighbor & (1 << i)) == 0)
                SR.color = Color.white;
            else
            {
                if (owner == -1) SR.color = Color.white;
                else SR.color = s.colors[owner];
            }
        }
    }
    public double GetFruitsEnergy() { return mFruit.GetComponent<FruitBehavior>().getEnergy(); }
    public double GetPestsEnergy() { return 0; }


    Dictionary<Neighbor, int> dict = new() { { Neighbor.Left, 3 }, { Neighbor.Right, 0 }, { Neighbor.LUp, 4 }, { Neighbor.RUp, 5 }, { Neighbor.LDown, 2 }, { Neighbor.RDown, 1 } };


    public GameObject anchorObject = null;

    void Anchoring()
    {
        var t = anchorObject;
        anchorObject = null;
        Destroy(t);
        foreach (var i in s.players)
        {
            if (i.alive && owner == i.pid && Time.time - i.last_move > s.game_pace * 0.7 && s.PosToCell(i.GetComponent<PlayerBehaviour>().transform.position) == s.PosToCell(transform.position) && GameServer.keySet[i.pid].isKeyDown())
            {
                var a = i.anchoring;
                if (a == Neighbor.None) break;
                var g = Instantiate(Resources.Load("Neighbor") as GameObject);
                Color arrowColor = s.colors[i.pid];
                arrowColor *= 0.4f;
                arrowColor.a = 1f;
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
    public void Reinforce(double amount)
    {
        hp += amount;
        last_reinforce = Time.time;
    }
    public void UpdateVolume()
    {
        capture_root.volume = ManageGameManager.sound_effects_volume;
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
