using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LandNumberShower : MonoBehaviour
{
    TMP_Text t;
    LandBehaviour b;
    // Start is called before the first frame update
    void Start()
    {
        b = GetComponent<LandBehaviour>();
        t = transform.GetChild(7).GetChild(0).GetComponent<TMP_Text>();
        transform.GetChild(7).GetChild(0).GetComponent<Transform>().position = Camera.main.WorldToScreenPoint(transform.position);
        t.text = string.Format("{0}",(int)b.hp);
    }

    // Update is called once per frame
    void Update()
    {
        t.text = string.Format("{0}",(int)b.hp);
    }
}
