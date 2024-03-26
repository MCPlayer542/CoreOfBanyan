using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialTextBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetText(string s, bool isEnterButton = false)
    {
        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = s;
        if (isEnterButton)
        {

        }
    }
    public string GetText()
    {
        return transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
    }

    void End()
    {
        Destroy(transform.gameObject);
    }
}
