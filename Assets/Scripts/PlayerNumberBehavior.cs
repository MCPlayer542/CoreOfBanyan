using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNumberBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    float sizeOfFont = 0.5f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var p = transform.gameObject.GetComponent<PlayerBehaviour>();
        Debug.Log(p);
        if (p != null)
        {
            var t = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            if (p.pid == 0)
            {
                t.rectTransform.localPosition = new Vector2(-Screen.width / 2 + t.fontSize * t.text.Length / 2.0f, Screen.height / 2 - 20);
            }
            else
            {
                t.rectTransform.localPosition = new Vector2(Screen.width / 2 - t.fontSize * t.text.Length / 2.0f, Screen.height / 2 - 20);
            }
            t.fontSize = sizeOfFont * (RectTransformUtility.WorldToScreenPoint(Camera.main, new(1, 0, 0)).x - RectTransformUtility.WorldToScreenPoint(Camera.main, new(0, 0, 0)).x);
            t.text = "E" + (p.pid + 1) + ": " + (int)p.energy;
        }
    }
}
