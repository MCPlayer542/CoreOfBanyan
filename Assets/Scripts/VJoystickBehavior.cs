using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
//using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Timeline;

public class VJoystickAnchor
{
    public Vector3 prepos;
    public List<Vector3> curpos = new() { };
}

public class VJoystickBehavior : MonoBehaviour
{
    static double speed = 5, half = 0.5, S3_2 = Math.Sqrt(3);
    public static GameServer s;
    public static VJoystickAnchor anchor = null;
    public PlayerBehaviour player = null;
    public int pid;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameServer.GameOverFlag) return;
        if (!s.players[pid].alive) return;
        if(!GameServer.keySet[pid].vjoystick)
        {
            if(ManageGameManager.GetKey(GameServer.keySet[pid].Left)) player.TryMove((int)DirID.LeftID);
            if(ManageGameManager.GetKey(GameServer.keySet[pid].Right)) player.TryMove((int)DirID.RightID);
            if(ManageGameManager.GetKey(GameServer.keySet[pid].LUp)) player.TryMove((int)DirID.LUpID);
            if(ManageGameManager.GetKey(GameServer.keySet[pid].RUp)) player.TryMove((int)DirID.RUpID);
            if(ManageGameManager.GetKey(GameServer.keySet[pid].LDown)) player.TryMove((int)DirID.LDownID);
            if(ManageGameManager.GetKey(GameServer.keySet[pid].RDown)) player.TryMove((int)DirID.RDownID);
            return;
        }
        Vector3 p = transform.position;
        int tx = 0, ty = 0;
        if (ManageGameManager.GetKey(GameServer.keySet[pid].Up)) ty++;
        if (ManageGameManager.GetKey(GameServer.keySet[pid].Down)) ty--;
        if (ManageGameManager.GetKey(GameServer.keySet[pid].Left)) tx--;
        if (ManageGameManager.GetKey(GameServer.keySet[pid].Right)) tx++;
        if (tx != 0 && ty != 0)
        {
            p.x += (float)(half * tx * speed * Time.smoothDeltaTime);
            p.y += (float)(S3_2 * ty * speed * Time.smoothDeltaTime);
            transform.position = p;
        }
        else if (tx != 0 || ty != 0)
        {
            p.x += (float)(tx * speed * Time.smoothDeltaTime);
            p.y += (float)(ty * speed * Time.smoothDeltaTime);
            transform.position = p;
        }
        else Reset();
        LandAnchoring();
        MovementDetermination();
    }
    void Reset()
    {
        transform.position = anchor.prepos;
    }

    void LandAnchoring()
    {
        if (player != null) player.anchoring = GetPotentialMovementDeter();
    }

    Neighbor GetPotentialMovementDeter()
    {
        Tuple<float, int> dir = new(Vector2.Distance(anchor.curpos[6], transform.position) * 100, 6);
        for (int i = 0; i < 6; ++i)
        {
            var t = Vector2.Distance(anchor.curpos[i], transform.position);
            if (t < dir.Item1) dir = new(t, i);
        }
        if (dir.Item2 < 6) return (Neighbor)(1 << dir.Item2);
        return Neighbor.None;
    }
    void MovementDetermination()
    {
        int dir = 0;
        for (int i = 1; i < 7; ++i) if (Vector2.Distance(anchor.curpos[i], transform.position) < Vector2.Distance(anchor.curpos[dir], transform.position)) dir = i;
        if (dir < 6)
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
