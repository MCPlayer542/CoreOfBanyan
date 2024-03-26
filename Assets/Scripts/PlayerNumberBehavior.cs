using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNumberBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerBehaviour mPlayerBehaviour;
    public GameServer mGameServer;
    public TMP_Text mOldEnergyUI, mEnergyUI, mEatFruitUI;
    float eatTime;
    float sizeOfFontOldEnergyUI = 0.5f, sizeOfFontEnergyUI = 0.3f;
    float sizeOfFontEatFruitNotice = 0.3f;
    void Start()
    {
        mPlayerBehaviour = gameObject.GetComponent<PlayerBehaviour>();
        mOldEnergyUI = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        mEnergyUI = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        mGameServer = Camera.main.GetComponent<GameServer>();
        mEnergyUI.color = Color.blue;
    }
    public void EatFruitNotice(Vector3 pos, float E)
    {
        //Debug.Log("test");
        Vector3 random_bias = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        Vector3 v = mPlayerBehaviour.transform.position;
        v.y += 0.3f;
        FruitTextBehavior fruit_text = Instantiate(Resources.Load("FruitText") as GameObject).transform.GetChild(0).GetChild(0).GetComponent<FruitTextBehavior>();
        fruit_text.init(v + random_bias, E);
        // var t = mEatFruitUI;
        // eatTime = Time.time;
        // t.text = "+" + (int)E;
        // Vector3 p = mPlayerBehaviour.transform.position;
        // p.y += 0.3f;
        // Color c = mEatFruitUI.color;
        // c.a = 1;
        // mEatFruitUI.color = c;
        // transform.GetChild(0).GetChild(1).GetComponent<Transform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, p);
        // t.fontSize = sizeOfFontEatFruitNotice * Vector2.Distance(RectTransformUtility.WorldToScreenPoint(Camera.main, new(1, 0, 0)), RectTransformUtility.WorldToScreenPoint(Camera.main, new(0, 0, 0)));
    }
    /*void UpdateEatNoticeUI()
    {
        // if (Time.time - eatTime > mGameServer.game_pace * 1.5f)
        // {
        //     Color c = mEatFruitUI.color;
        //     c.a = 0;
        //     mEatFruitUI.color = c;
        //     return;
        // }
        // Vector3 p = mPlayerBehaviour.transform.position;
        // p.y += 0.3f;
        // transform.GetChild(0).GetChild(1).GetComponent<Transform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, p);
    }*/
    void UpdateEnergyUI()
    {
        var p = mPlayerBehaviour;
        var t = mEnergyUI;
        t.text = "" + (long)p.energy;
        Vector3 v = mPlayerBehaviour.transform.position;
        v.y += 0.3f;
        t.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, v);
        t.fontSize = sizeOfFontEnergyUI * Vector2.Distance(RectTransformUtility.WorldToScreenPoint(Camera.main, new(1, 0, 0)), RectTransformUtility.WorldToScreenPoint(Camera.main, new(0, 0, 0)));
        /*if (p.pid == 0)
        {
            t.rectTransform.localPosition = new Vector2(-Screen.width / 2 + t.fontSize * t.text.Length / 2.0f, Screen.height / 2 - 20);
        }
        else
        {
            t.rectTransform.localPosition = new Vector2(Screen.width / 2 - t.fontSize * t.text.Length / 2.0f, Screen.height / 2 - 20);
        }*/
    }
    // Update is called once per frame
    void Update()
    {
        if (ManageGameManager.isPause) return;

        UpdateEnergyUI();
    }
}
