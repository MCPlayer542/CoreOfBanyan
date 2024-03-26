using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIElementBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    static float Width = 600, Height = 400;

    static float cursorFloat = 30;

    public Vector3 P1, P2;

    public Vector3 PP1, PP2;
    float sizeOfFont;
    bool UIStatus = false;

    public bool isBG = false;

    public bool isVisible = true;

    // Update is called once per frame
    void Update()
    {
        var t1 = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        var t2 = transform.GetChild(0).GetChild(1).GetComponent<RawImage>();
        if (!UIStatus)
        {
            sizeOfFont = t1.fontSize;
            PP1 = P1 = t1.rectTransform.localPosition;
            PP2 = P2 = t2.rectTransform.localPosition;

            UIStatus = true;
        }

        if (!isVisible)
        {
            t1.rectTransform.localPosition = new(-114514, -114514, 0);
            t2.rectTransform.localPosition = new(-114514, -114514, 0);
            PP1 = P1;
            PP2 = P2;
            return;
        }

        Vector3 p1 = PP1, p2 = PP2;

        float scale = Math.Min(Screen.width / Width, Screen.height / Height);
        if (isBG)
        {
            scale = Math.Max(Screen.width / Width, Screen.height / Height);
            p1 = new(p1.x + cursorFloat * Input.mousePosition.x / Screen.width, p1.y + cursorFloat * Input.mousePosition.y / Screen.height, p1.z);
            p2 = new(p2.x + cursorFloat * Input.mousePosition.x / Screen.width, p2.y + cursorFloat * Input.mousePosition.y / Screen.height, p2.z);
        }


        t1.fontSize = sizeOfFont * scale;
        t1.rectTransform.localPosition = new(p1.x * scale, p1.y * scale, p1.z);
        t2.rectTransform.localPosition = new(p2.x * scale, p2.y * scale, p2.z);
        t2.rectTransform.localScale = new(scale, scale, 0);
    }
}
