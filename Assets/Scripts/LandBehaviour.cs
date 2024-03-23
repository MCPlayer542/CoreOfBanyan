using System;
using System.Collections;
using System.Collections.Generic;
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
        hp = 3;
        colors = new(){Color.green, Color.red};
        neighbor = 0;
    }

    // Update is called once per frame
    readonly float Constant1 = 1,Constant2 = 1;
    void Update()
    {
        if(owner == -1) return;

        if(nearRoot) hp += Constant1 * Time.smoothDeltaTime;
        else hp -= Constant2 * Time.smoothDeltaTime;
        if(mPest != null) hp -= Constant1 * Time.smoothDeltaTime;
        
        if(hp<0){
            owner = -1;
            hp = 3;
            neighbor = 0;
            var p = s.PosToCell(transform.position);
            s.ChangeNeighborOfNeighbor(p.x,p.y,0);
            nearPlayer = false;
            nearRoot = false;
            mPest = null;
            mFruit = null;
        }
    }
    public bool CanBeCapturedBy(float energy) {
        return energy >= hp;
    }
    public void ChangeImg() {
        transform.GetChild(6).GetComponent<SpriteRenderer>().color = colors[owner];
        for(int i=0;i<6;++i){
            SpriteRenderer s = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if(((int)neighbor&(1<<i))==0)
                s.color = Color.white;
            else
                s.color = colors[owner];
        }
    }
    public float GetFruitsEnergy(){return 10;}
    public float GetPestsEnergy(){return 10;}
}
