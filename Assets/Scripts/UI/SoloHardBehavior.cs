using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloHardBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    static MKeySetClass k0 = new(0, 0, 0, 0, 0, 0);

    static MKeySetClass k1 = new(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Alpha1, KeyCode.Alpha2);

    static MKeySetClass k2 = new(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.Comma, KeyCode.Period);

    InitialStatus init = new(5, 2, new() { 0, 2, 0, 0, 0, 0 }, new() { k1, k0, k0, k0, k0, k0 });
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.gameObject.GetComponent<ClickScript>().isCollision() && Input.GetMouseButtonDown(0))
        {
            ManageGameManager.gameStatus = false;
            ManageGameManager.init = init;
            Camera.main.GetComponent<ManageGameManager>().EndGame();
            ManageGameManager.isTutorial = false;
            Camera.main.GetComponent<ManageGameManager>().ChangeDisplayStatus(null);
            ManageGameManager.gameStatus = true;
        }
    }
}
