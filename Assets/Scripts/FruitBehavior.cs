using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBehavior : MonoBehaviour
{
    const float blink_duration = 3;
    public static float life_time;
    public LandBehaviour owner;
    SpriteRenderer blink_renderer;
    float profit, spawn_time;
    // Start is called before the first frame update
    void Start()
    {
        spawn_time = Time.time;
        profit = (float)owner.hp;
        blink_renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ManageGameManager.isPause) spawn_time += Time.smoothDeltaTime;
        if (Time.time - spawn_time >= life_time)
        {
            owner.mFruit = null;
            Destroy(gameObject);
        }
        if (Time.time - spawn_time >= life_time-blink_duration)
        {
            Color v = blink_renderer.color;
            float dt = Time.time - spawn_time;
            if (dt - Mathf.Floor(dt) < 0.5) v.a = 0;
            else v.a = 1;
            blink_renderer.color = v;
        }
    }
    public float getEnergy() { return profit; }
}
