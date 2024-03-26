using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClickScript : MonoBehaviour
{
    // Start is called before the first frame update

    public static bool isVisible = true;
    public bool isActive = true;
    string txt = "";
    void Awake()
    {
        txt = transform.GetComponent<TMP_Text>().text;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isVisible)
        {
            //Debug.Log("s");
            return;
        }
        if (isActive)
        {
            var p = Input.mousePosition;
            if (isCollision())
            {
                transform.GetComponent<TMP_Text>().text = "> " + txt + " <";
            }
            else
            {
                transform.GetComponent<TMP_Text>().text = txt;
            }
        }
    }

    public bool isCollision()
    {
        var p = Input.mousePosition;
        return p.x <= transform.GetComponent<TMP_Text>().transform.position.x + transform.GetComponent<TMP_Text>().text.Length * transform.GetComponent<TMP_Text>().fontSize / 2
        && p.x >= transform.GetComponent<TMP_Text>().transform.position.x - transform.GetComponent<TMP_Text>().text.Length * transform.GetComponent<TMP_Text>().fontSize / 2
        && p.y <= transform.GetComponent<TMP_Text>().transform.position.y + transform.GetComponent<TMP_Text>().fontSize / 2
        && p.y >= transform.GetComponent<TMP_Text>().transform.position.y - transform.GetComponent<TMP_Text>().fontSize / 2;
    }
}
