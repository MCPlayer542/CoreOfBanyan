using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClickScript : MonoBehaviour
{
    // Start is called before the first frame update

    public bool isVisible = true;
    public bool isActive = false;
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
            Debug.Log(transform.GetComponent<TMP_Text>().text);
            transform.GetComponent<TMP_Text>().text = "";
            return;
        }
        if (isActive)
        {
            var p = Input.mousePosition;
            if (isCollision(p))
            {
                transform.GetComponent<TMP_Text>().text = "> " + txt + "< ";
                if (Input.GetMouseButtonDown(0))
                {
                    Camera.main.GetComponent<ManageGameManager>().ChangeDisplayStatus(-1);
                }
            }
            else
            {
                transform.GetComponent<TMP_Text>().text = txt;
            }
        }
    }

    bool isCollision(Vector2 p)
    {
        return p.x <= transform.GetComponent<TMP_Text>().transform.position.x + transform.GetComponent<TMP_Text>().fontSize / 2
        && p.x >= transform.GetComponent<TMP_Text>().transform.position.x - transform.GetComponent<TMP_Text>().fontSize / 2
        && p.y <= transform.GetComponent<TMP_Text>().transform.position.y + transform.GetComponent<TMP_Text>().text.Length * transform.GetComponent<TMP_Text>().fontSize / 2
        && p.y >= transform.GetComponent<TMP_Text>().transform.position.y - transform.GetComponent<TMP_Text>().text.Length * transform.GetComponent<TMP_Text>().fontSize / 2;
    }
}
