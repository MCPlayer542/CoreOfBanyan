using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;


public class LandBehaviour : MonoBehaviour
{
    public enum Neighbor{
        Left = 1,
        Up = 2,
        Right = 4,
        Down = 8
    };
    public List<Color> colors;
    // Start is called before the first frame update
    public int owner;
    public int hp;
    public Neighbor neighbor;

    void Start()
    {
        owner = -1;
        hp = 3;
        colors = new(){Color.green, Color.red};
        neighbor = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool CanBeCapturedBy(float energy) {
        return energy >= hp;
    }
    public void ChangeImg() {
        SpriteRenderer s = GetComponent<SpriteRenderer>();
        Sprite t = Resources.Load<Sprite>(String.Format("Textures/{0}",(int)neighbor));
        s.color = colors[owner];
        s.sprite = t;
    }
}
