using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DragScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    public bool isActive=true;

    bool dragStatus = false;
    Vector2 p;
    void Update()
    {
        dragStatus = isCollision() && Input.GetMouseButton(0);
        if (dragStatus && Input.GetMouseButton(0))
        {
            var t = transform.childCount;
            for (int i = 0; i < t; ++i)
            {
                var r = transform.GetChild(i);
                var R = r.GetComponent<UIElementBehavior>();
                float scale = Math.Min(Screen.width / UIElementBehavior.Width, Screen.height / UIElementBehavior.Height);
                R.PP1 = R.PP1 + (Vector3)((Vector2)(Input.mousePosition) - p)/scale;
                R.PP2 = R.PP2 + (Vector3)((Vector2)(Input.mousePosition) - p)/scale;
            }
        }
        p = Input.mousePosition;
    }

    public bool isCollision()
    {
        if(!isActive) return false;
        var t = transform.childCount;
        var p = Input.mousePosition;
        for (int i = 0; i < t; ++i)
        {
            var r = transform.GetChild(i);
            var R = r.GetChild(0).GetChild(1).GetComponent<RawImage>();
            if (p.x <= R.rectTransform.position.x+R.rectTransform.rect.xMax*R.transform.localScale.x && p.x >= R.rectTransform.position.x+R.rectTransform.rect.xMin*R.transform.localScale.x
            && p.y <= R.rectTransform.position.y+R.rectTransform.rect.yMax*R.transform.localScale.y && p.y >= R.rectTransform.position.y+R.rectTransform.rect.yMin*R.transform.localScale.y)
            {
                //Debug.Log("coll");
                return true;
            }
        }
        return false;
    }
}
