using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class FruitTextBehavior : MonoBehaviour
{
    const float size = 0.3f,speed_rate=0.2f,expire_speed=2;
    public TMP_Text fruit_text;
    float move_speed;
    Vector3 virpos;
    // Start is called before the first frame update
    void Start()
    {
        move_speed=GameServer.n*speed_rate;
    }
    public void init(Vector3 pos,float val)
    {
        fruit_text.color=Color.green;
        fruit_text.text="+"+(long)val;
        virpos=pos;
        transform.position=RectTransformUtility.WorldToScreenPoint(Camera.main, virpos);
    }

    // Update is called once per frame
    void Update()
    {
        virpos.y+=move_speed*Time.smoothDeltaTime;
        transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, virpos);
        fruit_text.fontSize = size * Vector2.Distance(RectTransformUtility.WorldToScreenPoint(Camera.main, new(1, 0, 0)), RectTransformUtility.WorldToScreenPoint(Camera.main, new(0, 0, 0)));
        Color c=fruit_text.color;
        c.a-=expire_speed*Time.smoothDeltaTime;
        fruit_text.color=c;
        if(c.a<=0) Destroy(gameObject);
    }
}
