using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    double curtime = -114;

    // Update is called once per frame
    void Update()
    {
        if (transform.gameObject.GetComponent<ClickScript>().isCollision() && Input.GetMouseButtonDown(0))
        {
            curtime = Time.timeAsDouble;
            Camera.main.GetComponent<ManageGameManager>().EndGame();
        }
        while (Time.timeAsDouble - curtime < 0.5 && ManageGameManager.s == null)
        {
            ManageGameManager.gameStatus = false;
            Camera.main.GetComponent<ManageGameManager>().EndGame();
            //ManageGameManager.isTutorial = false;
            Camera.main.GetComponent<ManageGameManager>().ChangeDisplayStatus(null);
            ManageGameManager.gameStatus = true;
            Camera.main.GetComponent<ManageGameManager>().ChangeDisplayStatus(new() { });
        }
    }
}
