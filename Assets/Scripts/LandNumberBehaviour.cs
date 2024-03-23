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
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(7).GetChild(0).GetComponent<Transform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        t.text = string.Format("{0}", Math.Min((long)b.hp, 9999));
        t.fontSize = sizeOfFont * (RectTransformUtility.WorldToScreenPoint(Camera.main, new(1, 0, 0)).x - RectTransformUtility.WorldToScreenPoint(Camera.main, new(0, 0, 0)).x);
    }
}
