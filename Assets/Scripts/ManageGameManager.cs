using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class InitialStatus
{
    public int size;
    public int playerNumber;
    public List<int> robotStatus;

    public int wallWeight;

    public float gamePace;
    public InitialStatus(int s, int p, List<int> r, int w = 0, float g = 1.0f / 3)
    {
        size = s;
        playerNumber = p;
        robotStatus = r;
        wallWeight = w;
        gamePace = g;
    }
}



public class ManageGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool gameStatus = false;
    public static bool isTutorial = false;

    static MKeySetClass
        k0 = new(0, 0, 0, 0, 0, 0),
        defaultk1 = new(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Alpha1, KeyCode.Alpha2),
        defaultk2 = new(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.Comma, KeyCode.Period),
        defaultk3 = new(KeyCode.T, KeyCode.G, KeyCode.F, KeyCode.H, KeyCode.Alpha4, KeyCode.Alpha5),
        defaultk4 = new(KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L, KeyCode.Alpha7, KeyCode.Alpha8),
        presetk1 = new(KeyCode.W, KeyCode.E, KeyCode.A, KeyCode.D, KeyCode.Z, KeyCode.X, KeyCode.Alpha1, KeyCode.Alpha2);
    static MKeySetClass k1 = defaultk1, k2 = defaultk2, k3 = defaultk3, k4 = defaultk4, k5 = k0, k6 = k0;

    public static List<MKeySetClass> listKeySet = new List<MKeySetClass>() { k1, k2, k3, k4, k0, k0 };

    public static float music_volume = 0.5f, sound_effects_volume = 0.3f;

    public static InitialStatus init = new(5, 2, new() { 0, 0, 0, 0, 0, 0 });



    public static GameServer s = null;
    public static bool isPause = true;
    public List<GameObject> displayObjects = new();
    public AudioSource maintheme = null, ingame = null, end_game = null;
    public int tutorial_level = 0;

    void Start()
    {
        UpdateVolume();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameServer.GameOverFlag)
        {
            isPause = true;
        }

        maintainUIStatus();

    }

    void maintainUIStatus()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (displayObjects[3].transform.GetChild(0).GetComponent<UIElementBehavior>().isVisible)
            {
                ChangeDisplayStatus(new() { 0 });
            }
            if (displayObjects[6].transform.GetChild(0).GetComponent<UIElementBehavior>().isVisible)
            {
                ChangeDisplayStatus(new() { 0, 3 });
            }
            if (displayObjects[7].transform.GetChild(0).GetComponent<UIElementBehavior>().isVisible)
            {
                //displayObjects[7].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<KeysetNumScript>().shitChange();
                ChangeDisplayStatus(new() { 0 });
            }
            if (GameServer.GameOverFlag == false && (!displayObjects[0].transform.GetChild(0).GetComponent<UIElementBehavior>().isVisible))
            {
                if (displayObjects[1].transform.GetChild(0).GetComponent<UIElementBehavior>().isVisible)
                {
                    ResumeGame();
                    withdrawDisplayStatus(new() { 1 });
                }
                else
                {
                    PauseGame();
                    DisplayStatus(new() { 1 });
                }
            }

            if (displayObjects[4].transform.GetChild(0).GetComponent<UIElementBehavior>().isVisible)
            {
                withdrawDisplayStatus(new() { 4 });
                if (isTutorial) DisplayStatus(new() { 5 });
                else DisplayStatus(new() { 1 });
            }
        }
    }


    public void NewGame()
    {
        isTutorial = false;
        tutorial_level = 0;
        EndGame();
        maintheme.Stop();
        ingame.Play();
        GameServer.n = init.size - 1;
        GameServer.PlayerNumber = init.playerNumber;
        GameServer.keySet = new(listKeySet);
        GameServer.end_game = end_game;
        GameServer.GameOverFlag = false;
        GameServer.gamePace = init.gamePace;
        GameServer.wallweight = 0.1f * init.wallWeight;
        //Debug.Log(init.wallWeight);
        Debug.Log(listKeySet[0].vjoystick);
        isPause = false;
        s = Camera.main.AddComponent<GameServer>();
    }
    public void NewTutorial()
    {
        isTutorial = true;
        ++tutorial_level;
        EndGameButMusic();
        maintheme.Stop();
        if (!ingame.isPlaying) ingame.Play();
        GameServer.end_game = end_game;
        GameServer.GameOverFlag = false;
        isPause = false;
        s = Camera.main.AddComponent<TutorialServer>();
    }

    public void EndGame()
    {
        ingame.Stop();
        EndGameButMusic();
    }
    void EndGameButMusic()
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
                    //s.LBmap[i][j].hp = 1145141919810;
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
            for (int i = 0; i < GameServer.PlayerNumber; ++i)
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

    public void DisplayStatus(List<int> lsid)
    {
        if (lsid == null)
        {
            //Debug.Log(lsid);
            if (isTutorial)
            {
                //Debug.Log("lsid");
                --tutorial_level;
                NewTutorial();
            }
            else NewGame();
            maintheme.Stop();
            return;
        }
        foreach (var sid in lsid)
        {
            if (sid < 0)
            {
                return;
            }
            if (sid == 0)
            {
                if (!maintheme.isPlaying) maintheme.Play();
                var t = displayObjects[0].transform;
                t.GetChild(2).GetChild(0).GetChild(0).GetComponent<ClickScript>().isActive = true;
                t.GetChild(3).GetChild(0).GetChild(0).GetComponent<ClickScript>().isActive = true;
            }
            var R = displayObjects[sid].transform;
            for (int i = 0; i < R.childCount; ++i)
            {
                R.GetChild(i).GetComponent<UIElementBehavior>().isVisible = true;
            }
        }
    }

    public void ChangeDisplayStatus(List<int> lsid)
    {
        foreach (var d in displayObjects)
        {
            var r = d.transform;
            for (int i = 0; i < r.childCount; ++i)
            {
                r.GetChild(i).GetComponent<UIElementBehavior>().isVisible = false;
            }
        }
        DisplayStatus(lsid);
    }

    public void withdrawDisplayStatus(List<int> lsid)
    {
        foreach (var sid in lsid)
        {
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
    public void ChangeMusicVolume(float v)
    {
        music_volume = v;
        UpdateVolume();
    }
    public void ChangeSoundEffectsVolume(float v)
    {
        sound_effects_volume = v;
        UpdateVolume();
    }
    void UpdateVolume()
    {
        maintheme.volume = music_volume;
        ingame.volume = music_volume;
        end_game.volume = sound_effects_volume;
        if (s != null)
        {
            foreach (PlayerBehaviour p in s.players)
            {
                p.UpdateVolume();
            }
            int n = GameServer.n;
            for (int i = 0; i <= 2 * n; ++i)
            {
                for (int j = 0; j <= 2 * n; ++j)
                {
                    if (i - j <= n && j - i <= n)
                    {
                        s.LBmap[i][j].UpdateVolume();
                    }
                }
            }
        }
    }

}
