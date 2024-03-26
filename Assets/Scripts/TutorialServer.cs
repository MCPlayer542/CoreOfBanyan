using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class TutorialServer : GameServer
{
    ManageGameManager gm = null;

    int level, stage;
    TutorialTextBehavior text = null;
    new public void Awake()
    {
        gm = GetComponent<ManageGameManager>();
        level = gm.tutorial_level;

        n = 3;
        PlayerNumber = level == 4 ? 1 : 2;

        LandBehaviour.s = this;
        PlayerBehaviour.s = this;
        PestAndFruitProducer.mGameServer = this;
        VJoystickBehavior.s = this;
        RobotBehaviourHJQ.s = this;
        RobotBehaviourLYK.s = this;

        FruitBehavior.life_time = 50 * game_pace;
        transform.position = new(n, 0, -10);
        GetComponent<Camera>().orthographicSize = (n + 1) * 0.866025f;

        bornPos.Clear();
        bornPos.Add(new(0, 0, -4));
        bornPos.Add(new(2 * n, 0, -4));

        keySet.Clear();
        ControlType = 0;
        UpdateControlKeyCode();

        colors.Add(Color.green);
        colors.Add(Color.red);
        //for(int i=0;i<PlayerNumber;++i)
        //colors.Add(new(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f)));

        map.Clear();
        LBmap.Clear();
        for (int i = 0; i <= 2 * n; ++i)
        {
            map.Add(new List<GameObject>());
            LBmap.Add(new List<LandBehaviour>());
            for (int j = 0; j <= 2 * n; ++j)
            {
                if (i - j <= n && j - i <= n)
                {
                    map[i].Add(Instantiate(Resources.Load("Land") as GameObject));
                    LBmap[i].Add(map[i][j].GetComponent<LandBehaviour>());
                    map[i][j].transform.localPosition = new((i + j) * 0.5f, (i - j) * 0.866025f, 0);
                }
                else
                {
                    map[i].Add(null);
                    LBmap[i].Add(null);
                }
            }
        }

        VJoystickAnchor anchor = new();
        anchor.prepos = map[n][n].transform.position;
        for (int i = 0; i < 6; ++i) anchor.curpos.Add(map[n + NeighborPos.Seek[i].x][n + NeighborPos.Seek[i].y].transform.position);
        anchor.curpos.Add(map[n][n].transform.position);
        VJoystickBehavior.anchor = anchor;
        players.Clear();
        for (int i = 0; i < PlayerNumber; ++i)
        {
            players.Add(Instantiate(Resources.Load("Player") as GameObject).GetComponent<PlayerBehaviour>());
            vjoysticks.Add(Instantiate(Resources.Load("VJoystick") as GameObject).GetComponent<VJoystickBehavior>());
            players[i].pid = i;
            vjoysticks[i].pid = i;
            vjoysticks[i].player = players[i];
            vjoysticks[i].transform.position = map[n][n].transform.position;
        }
        for (int i = 0; i < PlayerNumber; ++i)
        {
            var p = PosToCell(bornPos[i]);
            var sr = map[p.x][p.y].transform.GetChild(6).GetComponent<SpriteRenderer>();
            var sqrt = Resources.Load<Sprite>("Textures/SquareRoot");
            sr.sprite = sqrt;
        }
        Camera.main.AddComponent<PestAndFruitProducer>();

        wallList = new() { };
        foreach (var p in wallList)
        {
            LBmap[p.x][p.y].isWall = true;
            map[p.x][p.y].SetActive(false);
        }
        text = Instantiate(Resources.Load("UI/TutorialTextElement") as GameObject).GetComponent<TutorialTextBehavior>();
        TutorialInit();
    }
    void Update()
    {
        if (TutorialFinished()) TutorialStart();
    }
    void TutorialInit()
    {
        switch (level)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
        stage = 0;
        TutorialStart();
    }
    void TutorialStart()
    {
        ++stage;
        switch (level * 10 + stage)
        {
            case 11:
                text.SetText("按住A和D来进行左右移动，吃掉场地中间的苹果");
                break;
            case 12:
                text.SetText("你有没有注意到吃掉苹果时飘起的数字？核心上方的深蓝色数字代表你的创造力，吃苹果时会增加");
                break;
            case 13:
                text.SetText("现在同时按住D和W，进行斜向移动，吃掉场地角落的苹果；你也可以用WASD的其他组合来进行类似的斜向移动");
                break;
            case 14:
                text.SetText("苹果在生成后一段时间会闪烁，不及时吃掉的话会消失");
                break;
            case 15:
                gm.NewTutorial();
                break;
            case 25:
                gm.NewTutorial();
                break;
            case 36:
                gm.NewTutorial();
                break;
            case 44:
                gm.NewTutorial();
                break;
        }
    }
    bool TutorialFinished()
    {
        switch (level * 10 + stage)
        {
            default:
                return Input.GetKeyDown(KeyCode.Space);
        }
    }
    void UpdateControlKeyCode()
    {
        keySet.Add(new(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Alpha1, KeyCode.Alpha2));
        keySet.Add(new(0, 0, 0, 0, 0, 0));
        keySet.Add(new(0, 0, 0, 0, 0, 0));
        keySet.Add(new(0, 0, 0, 0, 0, 0));
        keySet.Add(new(0, 0, 0, 0, 0, 0));
        keySet.Add(new(0, 0, 0, 0, 0, 0));
    }
}