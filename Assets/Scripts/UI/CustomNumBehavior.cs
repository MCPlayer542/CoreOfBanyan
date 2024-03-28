using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CustomNumBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    string txt = "";
    void Start()
    {
        txt = transform.GetComponent<TMP_Text>().text;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCollision())
        {
            transform.GetComponent<TMP_Text>().text = "N\n" + txt + "\nV";
        }
        else
        {
            transform.GetComponent<TMP_Text>().text = txt;
        }
    }

    public bool isCollision()
    {
        var p = Input.mousePosition;
        return p.x <= transform.GetComponent<TMP_Text>().transform.position.x + txt.Length * transform.GetComponent<TMP_Text>().fontSize / 2
        && p.x >= transform.GetComponent<TMP_Text>().transform.position.x - txt.Length * transform.GetComponent<TMP_Text>().fontSize / 2
        && p.y <= transform.GetComponent<TMP_Text>().transform.position.y + transform.GetComponent<TMP_Text>().fontSize / 2
        && p.y >= transform.GetComponent<TMP_Text>().transform.position.y - transform.GetComponent<TMP_Text>().fontSize / 2;
    }
}
