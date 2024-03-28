using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ClickScript : MonoBehaviour
{
    // Start is called before the first frame update

    public static bool isVisible = true;
    public bool isActive = true;
    public bool isDisplayAnchor = true;
    public List<string> txt = new() { };
    void Awake()
    {
        txt = transform.GetComponent<TMP_Text>().text.Split("\n").ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isVisible)
        {
            //Debug.Log("s");
            return;
        }
        if (isActive && isDisplayAnchor)
        {
            var p = Input.mousePosition;
            if (isCollision())
            {
                string t = "";
                for (int i = 0; i < txt.Count; ++i)
                {
                    if (i == 0) t = t + ">" + txt[i] + "<";
                    else t = t + txt[i];
                    if (i != txt.Count - 1) t = t + "\n";
                }
                transform.GetComponent<TMP_Text>().text = t;
            }
            else
            {
                string t = "";
                for (int i = 0; i < txt.Count; ++i)
                {
                    t = t + txt[i];
                    if (i != txt.Count - 1) t = t + "\n";
                }
                transform.GetComponent<TMP_Text>().text = t;
            }
        }
    }

    public bool isCollision()
    {
        if (!isActive) return false;
        var p = Input.mousePosition;
        return p.x <= transform.GetComponent<TMP_Text>().transform.position.x + txt[0].Length * transform.GetComponent<TMP_Text>().fontSize / 2
        && p.x >= transform.GetComponent<TMP_Text>().transform.position.x - txt[0].Length * transform.GetComponent<TMP_Text>().fontSize / 2
        && p.y <= transform.GetComponent<TMP_Text>().transform.position.y + txt.Count * transform.GetComponent<TMP_Text>().fontSize / 2
        && p.y >= transform.GetComponent<TMP_Text>().transform.position.y - txt.Count * transform.GetComponent<TMP_Text>().fontSize / 2;
    }
}
