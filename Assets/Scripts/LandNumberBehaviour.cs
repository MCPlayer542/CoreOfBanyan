using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LandNumberShower : MonoBehaviour
{
    TMP_Text t;
    LandBehaviour b;

    float sizeOfFont = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        b = GetComponent<LandBehaviour>();
        t = transform.GetChild(7).GetChild(0).GetComponent<TMP_Text>();
        t.rectTransform.sizeDelta = new(233, 0);
        t.color=Color.black;
    }

    // Update is called once per frame
    void Update()
    {
        if (ManageGameManager.isPause) return;
        transform.GetChild(7).GetChild(0).GetComponent<Transform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        t.text = string.Format("{0}", Math.Min((long)b.hp, 9999));
        t.color=b.owner==-1||b.nearRoot&&b.mPest==null?Color.black:Color.red;
        if(Time.time-b.last_reinforce<=LandBehaviour.blink_time)
        {
            float d=(Time.time-b.last_reinforce)*5f;
            if(d-Mathf.Floor(d)<0.5) t.color=Color.blue;
        }
        t.fontSize = sizeOfFont * Vector2.Distance(RectTransformUtility.WorldToScreenPoint(Camera.main, new(1, 0, 0)), RectTransformUtility.WorldToScreenPoint(Camera.main, new(0, 0, 0)));
    }
}
