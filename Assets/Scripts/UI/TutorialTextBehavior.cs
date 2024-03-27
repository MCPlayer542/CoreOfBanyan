using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTextBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    void Update()
    {
        if ((int)(Time.timeAsDouble * 2) % 2 == 0)
        {
            var R = transform.GetChild(2).GetComponent<UIElementBehavior>();
            R.PP1 = R.P1 + new Vector3(0, 5, 0);
            R.PP2 = R.P2 + new Vector3(0, 5, 0);
        }
        else
        {
            var R = transform.GetChild(2).GetComponent<UIElementBehavior>();
            R.PP1 = R.P1;
            R.PP2 = R.P2;
        }
    }

    int lineLength = 25;

    public void SetText(string s, bool isEnterButton = true)
    {
        string t = "";
        int lines = (s.Length + lineLength - 1) / lineLength;
        int wordsOfLine = (s.Length + lines - 1) / lines;
        while (s.Length > wordsOfLine)
        {
            t = t + s.Substring(0, wordsOfLine) + "\n";
            s = s.Substring(wordsOfLine, s.Length - wordsOfLine);
        }
        t = t + s;
        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = t;
        transform.GetChild(2).GetComponent<UIElementBehavior>().isVisible = isEnterButton;
        transform.GetChild(3).GetComponent<UIElementBehavior>().isVisible = isEnterButton;
    }
    public string GetText()
    {
        return transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
    }

    public void End()
    {
        Destroy(transform.gameObject);
    }

    public bool isPressed()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || (Input.GetMouseButtonDown(0) && isCollision());
    }

    bool isCollision()
    {
        var p = Input.mousePosition;
        var s = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<RawImage>();
        return p.x <= s.rectTransform.position.x + s.rectTransform.rect.xMax * s.transform.localScale.x
        && p.x >= s.rectTransform.position.x + s.rectTransform.rect.xMin * s.transform.localScale.x
        && p.y <= s.rectTransform.position.y + s.rectTransform.rect.yMax * s.transform.localScale.y
        && p.y >= s.rectTransform.position.y + s.rectTransform.rect.yMin * s.transform.localScale.y;
    }
}
