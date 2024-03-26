using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButtonScript : MonoBehaviour
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
            Camera.main.GetComponent<ManageGameManager>().EndGame();
            Camera.main.GetComponent<ManageGameManager>().ChangeDisplayStatus(new() { 0 });
        }
    }
}
