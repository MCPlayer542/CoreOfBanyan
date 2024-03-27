using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpBehaviour : MonoBehaviour
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
            var R = Camera.main.GetComponent<ManageGameManager>().displayObjects[1].transform;
            for (int i = 0; i < R.childCount; ++i)
            {
                R.GetChild(i).GetComponent<UIElementBehavior>().isVisible = false;
            }
            R = Camera.main.GetComponent<ManageGameManager>().displayObjects[2].transform;
            for (int i = 0; i < R.childCount; ++i)
            {
                R.GetChild(i).GetComponent<UIElementBehavior>().isVisible = false;
            }
            Camera.main.GetComponent<ManageGameManager>().DisplayStatus(new() { 4 });
        }
    }
}
