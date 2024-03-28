using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class KeysetBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public KeyCode k;

    bool shit = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.gameObject.GetComponent<ClickScript>().txt = new() { k.ToString() };
        if (transform.gameObject.GetComponent<ClickScript>().isCollision() && Input.GetMouseButtonDown(0))
        {
            shit = true;
        }
        if (shit && Input.anyKeyDown && (!Input.GetMouseButtonDown(0)))
        {
            for (int i = 1; i < 510; ++i)
            {
                if (Input.GetKeyDown((KeyCode)(i)))
                {
                    k = (KeyCode)i;
                    shit = false;
                    break;
                }
            }
        }
        if (shit)
        {
            transform.GetComponent<TMP_Text>().color = Color.black;
        }
        else
        {
            transform.GetComponent<TMP_Text>().color = Color.white;
        }
    }
}
