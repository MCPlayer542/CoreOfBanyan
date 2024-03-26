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

    int lineLength = 25;

    public void SetText(string s, bool isEnterButton = false)
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
    }
    public string GetText()
    {
        return transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
    }

    public void End()
    {
        Destroy(transform.gameObject);
    }
}
