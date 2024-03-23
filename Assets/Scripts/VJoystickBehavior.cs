using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Timeline;

public class VJoystickAnchor
{
  public Vector3 prepos;
  public List<Vector3> curpos=new(){};
}

public class VJoystickBehavior : MonoBehaviour
{
    const float speed=3,half=0.5f,S3_2=0.8660254f;
    public static GameServer s;
    public static VJoystickAnchor anchor=null;
    public PlayerBehaviour player=null;
    public int pid;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(s.GameOverFlag) return;
        Vector3 p=transform.position;
        int tx=0,ty=0;
        if (Input.GetKey(s.keySet[pid].Up)) ty++;
        if (Input.GetKey(s.keySet[pid].Down)) ty--;
        if (Input.GetKey(s.keySet[pid].Left)) tx--;
        if (Input.GetKey(s.keySet[pid].Right)) tx++;
        if (tx != 0 && ty != 0)
        {
            p.x+=half*tx*speed*Time.smoothDeltaTime;
            p.y+=S3_2*ty*speed*Time.smoothDeltaTime;
        }
        else if(tx!=0||ty!=0)
        {
            p.x+=tx*speed*Time.smoothDeltaTime;
            p.y+=ty*speed*Time.smoothDeltaTime;
        }
        else Reset();
        transform.position=p;
        MovementDetermination();
    }
    void Reset()
    {
        transform.position=anchor.prepos;
    }
    void MovementDetermination()
    {
        int dir=0;
        for(int i=1;i<7;++i) if((anchor.curpos[i]-transform.position).magnitude<(anchor.curpos[dir]-transform.position).magnitude) dir=i;
        if(dir<6)
        {
            player.TryMove(dir);
            Reset();
        }
    }
    public void EndGame()
    {
        Destroy(transform.gameObject);
    }
}
