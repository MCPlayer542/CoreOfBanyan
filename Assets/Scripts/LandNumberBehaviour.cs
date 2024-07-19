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
        t.color = Color.black;
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(7).GetChild(0).GetComponent<Transform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        t.fontSize = sizeOfFont * Vector2.Distance(RectTransformUtility.WorldToScreenPoint(Camera.main, new(1, 0, 0)), RectTransformUtility.WorldToScreenPoint(Camera.main, new(0, 0, 0)));
        if (ManageGameManager.isPause) return;
        t.text = string.Format("{0}", (long)(Math.Min(b.hp, 9999.0)));
        t.color = b.owner == -1 || b.nearRoot && b.mPest == null ? Color.black : Color.red;
        if (Time.time - b.last_reinforce <= LandBehaviour.blink_time)
        {
            double d = (Time.time - b.last_reinforce) * 5f;
            if (d - System.Math.Floor(d) < 0.5) t.color = Color.blue;
        }
    }
}
