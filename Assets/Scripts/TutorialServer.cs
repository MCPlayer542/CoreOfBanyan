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
        PlayerNumber = level == 4 ? 2 : 1;

        LandBehaviour.s = this;
        PlayerBehaviour.s = this;
        PestAndFruitProducer.mGameServer = this;
        VJoystickBehavior.s = this;
        RobotBehaviourHJQ.s = this;
        RobotBehaviourLYK.s = this;

        FruitBehavior.life_time = 50 * game_pace;
        if (level != 4) FruitBehavior.life_time = 1000000;

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
            var pl=players[i];
            pl.transform.position = bornPos[i];
            pl.curpos = PosToCell(bornPos[i]);
            LandBehaviour bornLand = map[pl.curpos.x][pl.curpos.y].GetComponent<LandBehaviour>();
            bornLand.owner=i;
            bornLand.hp = level==4?50:1000;
            bornLand.isRoot = true;
            bornLand.nearPlayer = true;
            bornLand.nearRoot = true;
            pl.energy = level==4?3:1000;
            bornLand.ChangeImg();
        }
        if (level == 4) Camera.main.AddComponent<PestAndFruitProducer>();

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
                LandBehaviour t = LBmap[1][1];
                t.hp=1000;
                t.owner=0;
                t.neighbor=(Neighbor)16;
                t.mPest = Instantiate(Resources.Load("Pest") as GameObject);
                Vector3 v = t.transform.localPosition;
                v.z = -3;
                v.y += 0.2f;
                t.mPest.transform.localPosition = v;
                t.ChangeImg();
                ++players[0].PestNumber;
                t=LBmap[0][1];
                t.hp=1000;
                t.owner=0;
                t.neighbor=(Neighbor)10;
                t.ChangeImg();
                t=LBmap[1][2];
                t.hp=1000;
                t.owner=0;
                t.neighbor=(Neighbor)9;
                t.ChangeImg();
                t=LBmap[2][2];
                t.hp=1000;
                t.owner=0;
                t.neighbor=(Neighbor)20;
                t.ChangeImg();
                t=LBmap[2][1];
                t.hp=1000;
                t.owner=0;
                t.neighbor=(Neighbor)33;
                t.ChangeImg();
                t=LBmap[1][0];
                t.hp=1000;
                t.owner=0;
                t.neighbor=(Neighbor)18;
                t.ChangeImg();
                LBmap[0][0].neighbor=(Neighbor)8;
                LBmap[0][0].ChangeImg();
                UpdateMap();
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
                LBmap[n][n].hp = 1000;
                LBmap[n][n].owner = 0;
                LandBehaviour t = LBmap[n][n];
                t.mFruit = Instantiate(Resources.Load("Fruit") as GameObject);
                t.mFruit.GetComponent<FruitBehavior>().owner = t;
                Vector3 v = t.transform.localPosition;
                v.z = -3;
                v.y += 0.3f;
                t.mFruit.transform.localPosition = v;
                t.ChangeImg();
                UpdateMap();
                text.SetText("按住A和D来进行左右移动，吃掉场地中间的苹果，注意每隔一段时间才能移动一次！");
                break;
            case 12:
                text.SetText("你有没有注意到吃掉苹果时飘起的数字？核心上方的深蓝色数字代表你的创造力，吃苹果时会增加！");
                break;
            case 13:
                players[0].curpos = new Vector2Int(n, n);
                players[0].transform.position = CellToPos(n, n);
                LBmap[n][n].hp = 1000;
                LBmap[n][n].owner = 0;
                LBmap[n][n].ChangeImg();
                LBmap[2 * n][n].hp = 1000;
                LBmap[2 * n][n].owner = 0;
                t = LBmap[2 * n][n];
                t.mFruit = Instantiate(Resources.Load("Fruit") as GameObject);
                t.mFruit.GetComponent<FruitBehavior>().owner = t;
                v = t.transform.localPosition;
                v.z = -3;
                v.y += 0.3f;
                t.mFruit.transform.localPosition = v;
                t.ChangeImg();
                UpdateMap();
                text.SetText("现在同时按住 D 和 W，进行斜向移动，吃掉场地角落的苹果；你也可以用 WASD 的其他组合来进行类似的斜向移动！");
                break;
            case 14:
                text.SetText("苹果在生成后一段时间会闪烁，不及时吃掉的话会消失！");
                break;
            case 15:
                gm.NewTutorial();
                break;
            case 21:
                text.SetText("去到树枝末端消灭害虫，注意树枝是不能长成回路的！");
                break;
            case 22:
                text.SetText("你有没有注意到被害虫侵袭的树枝上的数字减少了？树枝上的黑色数字代表坚固性，害虫会啃食你的枝干，减少到1后会断开，注意及时清理！");
                break;
            case 23:
                text.SetText("现在按下数字键1，使用“落叶归根”快速回到你的树根(方形结点)！");
                break;
            case 24:
                text.SetText("注意这并不是没有代价的，你失去了刚刚所在的树枝！");
                break;
            case 25:
                gm.NewTutorial();
                break;
            case 31:
                text.SetText("噢不！你现在和根断开了！你头上的创造力会变成红色并不再自动增加，脚下树枝的坚固值也变为红色并在逐渐流失！");
                break;
            case 32:
                text.SetText("你可以使用“落叶归根”快速回到根，使你的创造力恢复增长！");
                break;
            case 33:
                text.SetText("非常棒！现在移动回去接上树枝，使树枝的坚固性恢复增长，避免树枝消亡！");
                break;
            case 34:
                text.SetText("现在树枝有点脆弱，可以按数字键2来使用“固若金汤”，消耗一定能量加固脚下的和与你直接相连的树枝！");
                break;
            case 35:
                text.SetText("根非常重要，它是你的一切能量来源。和树根连通的树枝越多，你的创造力增长就越快，同时只有与根连通的地方才会结果或生虫！");
                break;
            case 36:
                gm.NewTutorial();
                break;
            case 41:
                text.SetText("现在我们进入实战！看到中间的那个榕树核心了吗？积累一定创造力后，我们可以移动到它的树枝上来占领它！");
                break;
            case 42:
                text.SetText("它被我们打回根了！现在我们去占领它的根，消灭它吧！");
                break;
            case 43:
                text.SetText("如果场上只有你一棵榕树，你就获得了胜利；也要当心不要被别人消灭了！");
                break;
        }
    }
    bool TutorialFinished()
    {
        switch (level * 10 + stage)
        {
            case 11:
                return LBmap[n][n].mFruit == null;
            case 13:
                return LBmap[2 * n][n].mFruit == null;
            case 21:
                return players[0].PestNumber==0;
            case 23:
                return Input.GetKey(KeyCode.Alpha1);
            case 44:
                return false;
            default:
                return text.isPressed();
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
    public override void GameOver()
    {
        if (level == 4) base.GameOver();
    }
    public override void EndGame()
    {
        base.EndGame();
        Destroy(text.gameObject);
        Destroy(Camera.main.GetComponent<TutorialServer>());
    }
}