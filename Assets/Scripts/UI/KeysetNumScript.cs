using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer.Internal;
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
            ManageGameManager.listKeySet[pid - 1].vjoystick = true;
            ManageGameManager.listKeySet[pid - 1] = new(k[0].transform.gameObject.GetComponent<KeysetBehavior>().k
            , k[1].transform.gameObject.GetComponent<KeysetBehavior>().k
            , k[2].transform.gameObject.GetComponent<KeysetBehavior>().k
            , k[3].transform.gameObject.GetComponent<KeysetBehavior>().k
            , k[4].transform.gameObject.GetComponent<KeysetBehavior>().k
            , k[5].transform.gameObject.GetComponent<KeysetBehavior>().k);
        }
        else
        {
            ManageGameManager.listKeySet[pid - 1].vjoystick = false;
            ManageGameManager.listKeySet[pid - 1] = new(k[6].transform.gameObject.GetComponent<KeysetBehavior>().k
            , k[7].transform.gameObject.GetComponent<KeysetBehavior>().k
            , k[8].transform.gameObject.GetComponent<KeysetBehavior>().k
            , k[9].transform.gameObject.GetComponent<KeysetBehavior>().k
            , k[10].transform.gameObject.GetComponent<KeysetBehavior>().k
            , k[11].transform.gameObject.GetComponent<KeysetBehavior>().k
            , k[12].transform.gameObject.GetComponent<KeysetBehavior>().k
            , k[13].transform.gameObject.GetComponent<KeysetBehavior>().k);
        }

        bool shit = false;

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

            shit = true;
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

            shit = true;
        }

        pid = Convert.ToInt32(txt);

        if (shit)
        {
            shitChange();
        }
    }


    public void shitChange()
    {
        int pid = Convert.ToInt32(txt);
        isJ = ManageGameManager.listKeySet[pid - 1].vjoystick;
        var p = ManageGameManager.listKeySet[pid - 1];
        if (isJ)
        {
            k[0].transform.gameObject.GetComponent<KeysetBehavior>().k = p.Up;
            k[1].transform.gameObject.GetComponent<KeysetBehavior>().k = p.Down;
            k[2].transform.gameObject.GetComponent<KeysetBehavior>().k = p.Left;
            k[3].transform.gameObject.GetComponent<KeysetBehavior>().k = p.Right;
            k[4].transform.gameObject.GetComponent<KeysetBehavior>().k = p.Back;
            k[5].transform.gameObject.GetComponent<KeysetBehavior>().k = p.Reinforce;
        }
        else
        {
            k[6].transform.gameObject.GetComponent<KeysetBehavior>().k = p.LUp;
            k[7].transform.gameObject.GetComponent<KeysetBehavior>().k = p.RUp;
            k[8].transform.gameObject.GetComponent<KeysetBehavior>().k = p.Left;
            k[9].transform.gameObject.GetComponent<KeysetBehavior>().k = p.Right;
            k[10].transform.gameObject.GetComponent<KeysetBehavior>().k = p.LDown;
            k[11].transform.gameObject.GetComponent<KeysetBehavior>().k = p.RDown;
            k[12].transform.gameObject.GetComponent<KeysetBehavior>().k = p.Back;
            k[13].transform.gameObject.GetComponent<KeysetBehavior>().k = p.Reinforce;
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
