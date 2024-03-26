using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class ManageGameManager : MonoBehaviour
{
    // Start is called before the first frame update

    GameServer s = null;
    public static bool isPause = true;
    public List<GameObject> displayObjects = new();
    public AudioSource maintheme = null, ingame = null;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GameServer.GameOverFlag)
        {
            isPause = true;
        }
        if (isPause && s != null)
        {
            for (int i = 0; i <= 2 * GameServer.n; ++i)
            {
                for (int j = 0; j <= 2 * GameServer.n; ++j)
                {
                    if (i - j <= GameServer.n && j - i <= GameServer.n)
                    {
                        s.LBmap[i][j].hp = 114514;
                    }
                }
            }
        }
    }

    public void NewGame()
    {
        EndGame();
        maintheme.Stop();
        ingame.Play();
        isPause = false;
        s = Camera.main.AddComponent<GameServer>();
        GameServer.GameOverFlag = false;
    }

    public void EndGame()
    {
        isPause = false;
        if (s != null) s.EndGame();
        s = null;
        isPause = true;
    }

    List<List<double>> hps = null;
    List<double> energys = null;

    public void PauseGame()
    {
        ingame.Pause();
        isPause = true;
        hps = new();
        energys = new();
        for (int i = 0; i <= 2 * GameServer.n; ++i)
        {
            hps.Add(new List<double>());
            for (int j = 0; j <= 2 * GameServer.n; ++j)
            {
                if (i - j <= GameServer.n && j - i <= GameServer.n)
                {
                    hps[i].Add(s.LBmap[i][j].hp);
                }
                else
                {
                    hps[i].Add(0);
                }
            }
        }
        foreach (var i in s.players)
        {
            energys.Add(i.energy);
        }
    }

    public void ResumeGame()
    {
        ingame.Play();
        if (hps != null)
        {
            for (int i = 0; i <= 2 * GameServer.n; ++i)
            {
                for (int j = 0; j <= 2 * GameServer.n; ++j)
                {
                    if (i - j <= GameServer.n && j - i <= GameServer.n)
                    {
                        //Debug.Log(s.LBmap[i][j]);
                        s.LBmap[i][j].hp = hps[i][j];
                    }
                }
            }
            for (int i = 0; i < s.PlayerNumber; ++i)
            {
                s.players[i].energy = energys[i];
            }
            hps = null;
        }
        isPause = false;
    }

    public void ChangePauseStatus()
    {
        if (isPause)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public static bool GetKey(KeyCode k)
    {
        if (isPause) return false;
        return Input.GetKey(k);
    }

    public static bool GetKeyDown(KeyCode k)
    {
        if (isPause) return false;
        if (k == 0) return false;
        return Input.GetKeyDown(k);
    }

    public void ChangeDisplayStatus(int sid)
    {
        foreach (var d in displayObjects)
        {
            var r = d.transform;
            for (int i = 0; i < r.childCount; ++i)
            {
                r.GetChild(i).GetComponent<UIElementBehavior>().isVisible = false;
            }
        }
        if (sid == -1)
        {
            NewGame();
        }
        if (sid < 0)
        {
            return;
        }
        var R = displayObjects[sid].transform;
        for (int i = 0; i < R.childCount; ++i)
        {
            R.GetChild(i).GetComponent<UIElementBehavior>().isVisible = false;
        }
    }


}
