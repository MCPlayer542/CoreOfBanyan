using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeysetNumScript : MonoBehaviour
{
    public string txt = "";

    public int minNum = 0, maxNum = 100;

    public List<TMP_Text> listLimit = new();

    public int minLimit = 0, maxLimit = 1000;


    public bool isJ = true;
    public List<TMP_Text> k = new() { };

    void Start()
    {
        txt = transform.GetComponent<TMP_Text>().text;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCollision())
        {
            transform.GetComponent<TMP_Text>().text = "n\n" + txt + "\nv";
        }
        else
        {
            transform.GetComponent<TMP_Text>().text = txt;
        }
        int pid = Convert.ToInt32(txt);

        if (isJ)
        {
            //ManageGameManager.listKeySet[pid]=new(k[0].get,k[1])
        }
        else
        {

        }

        if (isCollision(1) && Input.GetMouseButtonDown(0))
        {
            var t = Math.Max(Math.Min(Convert.ToInt32(txt) - 1, maxNum), minNum);
            txt = t.ToString();
            int tot = 0;
            foreach (var i in listLimit)
            {
                tot += Convert.ToInt32(i.GetComponent<CustomNumBehavior>().txt);
            }
            if (tot < minLimit || tot > maxLimit) txt = (t + 1).ToString();
        }
        else if (isCollision(-1) && Input.GetMouseButtonDown(0))
        {
            var t = Math.Max(Math.Min(Convert.ToInt32(txt) + 1, maxNum), minNum);
            txt = t.ToString();
            int tot = 0;
            foreach (var i in listLimit)
            {
                tot += Convert.ToInt32(i.GetComponent<CustomNumBehavior>().txt);
            }
            if (tot < minLimit || tot > maxLimit) txt = (t - 1).ToString();
        }

    }

    public bool isCollision(int status = 0)
    {
        var p = Input.mousePosition;
        if (status == 0)
            return p.x <= transform.GetComponent<TMP_Text>().transform.position.x + txt.Length * transform.GetComponent<TMP_Text>().fontSize / 2
            && p.x >= transform.GetComponent<TMP_Text>().transform.position.x - txt.Length * transform.GetComponent<TMP_Text>().fontSize / 2
            && p.y <= transform.GetComponent<TMP_Text>().transform.position.y + 3 * transform.GetComponent<TMP_Text>().fontSize / 2
            && p.y >= transform.GetComponent<TMP_Text>().transform.position.y - 3 * transform.GetComponent<TMP_Text>().fontSize / 2;
        else
            return p.x <= transform.GetComponent<TMP_Text>().transform.position.x + txt.Length * transform.GetComponent<TMP_Text>().fontSize / 2
            && p.x >= transform.GetComponent<TMP_Text>().transform.position.x - txt.Length * transform.GetComponent<TMP_Text>().fontSize / 2
            && (p.y + transform.GetComponent<TMP_Text>().fontSize * status) <= transform.GetComponent<TMP_Text>().transform.position.y + transform.GetComponent<TMP_Text>().fontSize / 2
            && (p.y + transform.GetComponent<TMP_Text>().fontSize * status) >= transform.GetComponent<TMP_Text>().transform.position.y - transform.GetComponent<TMP_Text>().fontSize / 2;
    }
}
