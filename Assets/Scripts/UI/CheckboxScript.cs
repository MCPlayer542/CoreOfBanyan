using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CheckboxScript : MonoBehaviour
{
    public TMP_Text js, njs;
    public bool isJ = true;
    public static bool isJS = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isJS)
        {
            js.color = Color.black;
            njs.color = Color.white;
        }
        else
        {
            njs.color = Color.black;
            js.color = Color.white;
        }
        if (transform.gameObject.GetComponent<ClickScript>().isCollision() && Input.GetMouseButtonDown(0))
        {
            isJS = isJ;
        }
    }
}
