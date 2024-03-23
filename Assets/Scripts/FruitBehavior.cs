using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBehavior : MonoBehaviour
{
    const float life_time=15,blink_time=12;
    public LandBehaviour owner;
    SpriteRenderer renderer;
    float profit,spawn_time;
    // Start is called before the first frame update
    void Start()
    {
        spawn_time=Time.time;
        profit=owner.hp;
        renderer=GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time-spawn_time>=life_time)
        {
            owner.mFruit=null;
            Destroy(gameObject);
        }
        if(Time.time-spawn_time>=blink_time)
        {
            Color v=renderer.color;
            float dt=Time.time-spawn_time;
            if(dt-Mathf.Floor(dt)<0.5) v.a=0;
            else v.a=255;
            renderer.color=v;
        }
    }
}
