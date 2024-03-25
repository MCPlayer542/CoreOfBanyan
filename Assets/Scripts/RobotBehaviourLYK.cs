using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBehaviourLYK : MonoBehaviour
{
    GameServer s;
    PlayerBehaviour self;
    int Left=0,Right=1,LUp=2,RUp=3,LDown=4,RDown=5;
    // Start is called before the first frame update
    void Start()
    {
        s = Camera.main.GetComponent<GameServer>();
        self = GetComponent<PlayerBehaviour>();
    }

    // Update is called once per frame
    float last_move = 0;
    void Update()
    {
        if (Time.time - last_move <= s.game_pace)
            return;
        if(!s.LBmap[self.curpos.x][self.curpos.y].nearRoot){
            s.BackHome(self.pid);
            return;
        }
        int dir = Left;
        // find_near_min_dir();
        if(self.TryMove(dir))
            last_move = Time.time;
    }
}
