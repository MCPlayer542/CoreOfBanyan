using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public int pid;
    public float speed;
    public float energy;
    public float energyGrowthSpeed;
    GameServer s;
    // Start is called before the first frame update
    void Start()
    {
        s = Camera.main.GetComponent<GameServer>();
        transform.position = s.bornPos[pid]; 
        speed = 3.0f;
        energy = 0;
        energyGrowthSpeed = 5;
    }

    // Update is called once per frame
    void Update()
    {
        energy += energyGrowthSpeed * Time.smoothDeltaTime;
        Vector3 p = transform.position;
        if(Input.GetKey(s.keySet[pid].Up))      p.y += speed * Time.smoothDeltaTime;
        if(Input.GetKey(s.keySet[pid].Down))    p.y -= speed * Time.smoothDeltaTime;
        if(Input.GetKey(s.keySet[pid].Left))    p.x -= speed * Time.smoothDeltaTime;
        if(Input.GetKey(s.keySet[pid].Right))   p.x += speed * Time.smoothDeltaTime;

        if(s.OutOfScreen(p)) return;

        int index = (int)Math.Round(p.x), jndex = (int)Math.Round(p.y);
        LandBehaviour l = s.map[index][jndex].GetComponent<LandBehaviour>();
        if(l.owner != pid && !l.CanBeCapturedBy(energy)) return;
        if(l.owner != pid) Capture(l,index,jndex);
        if(l.owner == pid) {
            int preindex = (int)Math.Round(transform.position.x), prejndex = (int)Math.Round(transform.position.y);
            LandBehaviour pre = s.map[preindex][prejndex].GetComponent<LandBehaviour>();
            if(preindex+1 == index && (pre.neighbor&LandBehaviour.Neighbor.Right)==0) return;
            if(preindex-1 == index && (pre.neighbor&LandBehaviour.Neighbor.Left)==0) return;
            if(prejndex+1 == jndex && (pre.neighbor&LandBehaviour.Neighbor.Up)==0) return;
            if(prejndex-1 == jndex && (pre.neighbor&LandBehaviour.Neighbor.Down)==0) return;
        }

        transform.position = p;
    }
    void Capture(LandBehaviour l,int index, int jndex){
        energy -= l.hp;
        l.owner = pid;
        l.neighbor = 0;
        int preindex = (int)Math.Round(transform.position.x), prejndex = (int)Math.Round(transform.position.y);
        LandBehaviour pre = s.map[preindex][prejndex].GetComponent<LandBehaviour>();
        if(preindex+1 == index){
            pre.neighbor |= LandBehaviour.Neighbor.Right;
            l.neighbor |= LandBehaviour.Neighbor.Left;
        }
        else if(preindex-1 == index){
            pre.neighbor |= LandBehaviour.Neighbor.Left;
            l.neighbor |= LandBehaviour.Neighbor.Right;
        }
        else if(prejndex+1 == jndex){
            pre.neighbor |= LandBehaviour.Neighbor.Up;
            l.neighbor |= LandBehaviour.Neighbor.Down;
        }
        else if(prejndex-1 == jndex){
            pre.neighbor |= LandBehaviour.Neighbor.Down;
            l.neighbor |= LandBehaviour.Neighbor.Up;
        }
        pre.ChangeImg();
        l.ChangeImg();
    }
}
