using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TutorialButtonBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.gameObject.GetComponent<ClickScript>().isCollision() && Input.GetMouseButtonDown(0))
        {
            var t = Camera.main.GetComponent<ManageGameManager>();
            t.ChangeDisplayStatus(new() { });
            t.tutorial_level = 0;
            t.NewTutorial();
        }
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}