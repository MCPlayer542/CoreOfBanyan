using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManageGameManager : MonoBehaviour
{
    // Start is called before the first frame update

    GameServer s = null;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewGame()
    {
        if (s != null) s.EndGame();
        s = Camera.main.AddComponent<GameServer>();
    }
}
