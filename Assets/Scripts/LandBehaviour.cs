using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine;

public enum Neighbor{
    Left = 1,
    Right = 2,
    LUp = 4,
    RUp = 8,
    LDown = 16,
    RDown = 32
};
public class NeighborPos{
    static public Vector2Int Left=new(-1,-1), Right=new(1,1), RUp=new(1,0), LDown=new(-1,0), RDown=new(0,1), LUp=new(0,-1);
}
public class LandBehaviour : MonoBehaviour
{
    const float k3=0.5f;
    public static GameServer s;
    public List<Color> colors;
    // Start is called before the first frame update
    public int owner;
    public float hp;
    public Neighbor neighbor;
    public bool nearPlayer, nearRoot;
    public GameObject mPest=null;
    public GameObject mFruit=null;

    void Awake()
    {
        owner = -1;
        hp = 1;
        colors = new(){Color.green, Color.red};
        neighbor = 0;
    }

    // Update is called once per frame
    readonly float Constant1 = 0.2f,Constant2 = 0.5f;
    void Update()
    {
        if(owner == -1)return;
        if(nearRoot&&nearPlayer) s.players[owner].energy+=k3*Time.smoothDeltaTime;
        if(nearRoot) hp += Constant1 * Time.smoothDeltaTime;
        else hp -= Constant2 * Time.smoothDeltaTime;
        if(mPest != null) hp -= Constant1 * Time.smoothDeltaTime;
        
        if(hp<=1){
            Captured(-1,neighbor,1);
            neighbor=0;
        }
    }
    public void Captured(int new_owner,Neighbor new_neighbor, float new_hp) {
        var cur = s.PosToCell(transform.position);
        if (owner != -1) s.ChangeNeighborOfNeighbor(cur.x, cur.y, new_neighbor);
        owner = new_owner;
        Destroy(mPest);
        Destroy(mFruit);
        mPest = null;
        mFruit = null;
        hp = new_hp;
        ChangeImg();
    }
    public void ChangeImg() {
        if(owner==-1) transform.GetChild(6).GetComponent<SpriteRenderer>().color = Color.white;
        else transform.GetChild(6).GetComponent<SpriteRenderer>().color = colors[owner];
        for(int i=0;i<6;++i){
            SpriteRenderer s = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if(((int)neighbor&(1<<i))==0)
                s.color = Color.white;
            else {
                if(owner == -1) s.color = Color.white;
                else s.color = colors[owner];
            }
        }
    }
    public float GetFruitsEnergy(){return mFruit.GetComponent<FruitBehavior>().getEnergy();}
    public float GetPestsEnergy(){return 0;}
}
