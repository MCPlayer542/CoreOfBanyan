using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNumberBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerBehaviour mPlayerBehaviour;
    public GameServer mGameServer;
    public TMP_Text mEnergyUI;
    public TMP_Text mEatFruitUI;
    float eatTime;
    float sizeOfFontEnergyUI = 0.5f;
    float sizeOfFontEatFruitNotice = 0.3f;
    void Start()
    {
        mPlayerBehaviour=gameObject.GetComponent<PlayerBehaviour>();
        mEnergyUI=transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        mEatFruitUI=transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        mGameServer=Camera.main.GetComponent<GameServer>();
        Color c=mEatFruitUI.color;
        c.a=0;
        mEatFruitUI.color=c;
    }
    public void EatFruitNotice(float E){
        var t = mEatFruitUI;
        eatTime=Time.time;
        t.text="+ "+(int)E;
        Vector3 p=mPlayerBehaviour.transform.position;
        p.y+=0.3f;
        mEatFruitUI.color=new(69,255,63,1);
        mEatFruitUI.transform.position=p;
        t.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, p);
        t.fontSize = sizeOfFontEatFruitNotice * Vector2.Distance(RectTransformUtility.WorldToScreenPoint(Camera.main, new(1, 0, 0)), RectTransformUtility.WorldToScreenPoint(Camera.main, new(0, 0, 0)));
    }
    void UpdateEatNoticeUI(){
        if(Time.time-eatTime>mGameServer.game_pace*2){
            Color c=mEatFruitUI.color;
            c.a=0;
            mEatFruitUI.color=c;
            return;
        }
        //Time.time-eatTime<=mGameServer.game_pace
        //
    }
    void UpdateEnergyUI(){
        var p = mPlayerBehaviour;
        var t = mEnergyUI;
        t.fontSize = sizeOfFontEnergyUI * Vector2.Distance(RectTransformUtility.WorldToScreenPoint(Camera.main, new(1, 0, 0)), RectTransformUtility.WorldToScreenPoint(Camera.main, new(0, 0, 0)));
        t.text = "E" + (p.pid + 1) + ": " + (long)p.energy;
        if (p.pid == 0)
        {
            t.rectTransform.localPosition = new Vector2(-Screen.width / 2 + t.fontSize * t.text.Length / 2.0f, Screen.height / 2 - 20);
        }
        else
        {
            t.rectTransform.localPosition = new Vector2(Screen.width / 2 - t.fontSize * t.text.Length / 2.0f, Screen.height / 2 - 20);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (ManageGameManager.isPause) return;
        if (mPlayerBehaviour != null)
        {
            UpdateEnergyUI();
            UpdateEatNoticeUI();
        }
    }
}
