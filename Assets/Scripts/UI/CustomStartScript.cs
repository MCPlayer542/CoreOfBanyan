using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomStartScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    double curtime = -114;

    // Update is called once per frame
    void Update()
    {
        var t = Camera.main.GetComponent<ManageGameManager>().displayObjects[6].transform;

        int A = Convert.ToInt32(t.GetChild(1).GetChild(0).GetChild(0).GetComponent<CustomNumBehavior>().txt);
        int B = Convert.ToInt32(t.GetChild(2).GetChild(0).GetChild(0).GetComponent<CustomNumBehavior>().txt);
        int C = Convert.ToInt32(t.GetChild(3).GetChild(0).GetChild(0).GetComponent<CustomNumBehavior>().txt);

        InitialStatus init =
        new(
            Convert.ToInt32(t.GetChild(0).GetChild(0).GetChild(0).GetComponent<CustomNumBehavior>().txt)
        , A + B + C
        , new() { 0, 0, 0, 0, 0, 0 }
        , Convert.ToInt32(t.GetChild(4).GetChild(0).GetChild(0).GetComponent<CustomNumBehavior>().txt)
        , 1.0f / Convert.ToInt32(t.GetChild(5).GetChild(0).GetChild(0).GetComponent<CustomNumBehavior>().txt));

        for (int i = A; i < A + B; ++i) init.robotStatus[i] = 1;
        for (int i = A + B; i < A + B + C; ++i) init.robotStatus[i] = 2;


        if (transform.gameObject.GetComponent<ClickScript>().isCollision() && Input.GetMouseButtonDown(0))
        {
            curtime = Time.timeAsDouble;
        }
        if (Time.timeAsDouble - curtime < 0.3)
        {
            ManageGameManager.gameStatus = false;
            ManageGameManager.init = init;
            Camera.main.GetComponent<ManageGameManager>().EndGame();
            ManageGameManager.isTutorial = false;
            Camera.main.GetComponent<ManageGameManager>().ChangeDisplayStatus(null);
            ManageGameManager.gameStatus = true;
            Camera.main.GetComponent<ManageGameManager>().ChangeDisplayStatus(new() { });
        }
    }
}