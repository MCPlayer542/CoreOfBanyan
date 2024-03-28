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
    public TMP_Text mEnergyUI, mEatFruitUI, mIdentityUI;
    float sizeOfFontEnergyUI = 0.3f;
    float startTime=-114;
    void Start()
    {
        mPlayerBehaviour = gameObject.GetComponent<PlayerBehaviour>();
        mEnergyUI = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        mIdentityUI = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        mGameServer = PlayerBehaviour.s;
        mEnergyUI.color = Color.blue;
        mIdentityUI.color = Color.red;
        startTime=Time.time;
    }
    public void EatFruitNotice(Vector3 pos, float E)
    {
        //Debug.Log("test");
        Vector3 random_bias = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        FruitTextBehavior fruit_text = Instantiate(Resources.Load("FruitText") as GameObject).transform.GetChild(0).GetChild(0).GetComponent<FruitTextBehavior>();
        fruit_text.init(pos + random_bias, E);
    }
    void UpdateEnergyUI()
    {
        var p = mPlayerBehaviour;
        var t = mEnergyUI;
        t.color=mGameServer.LBmap[p.curpos.x][p.curpos.y].nearRoot?Color.blue:Color.red;
        t.text = "" + (long)p.energy;
        Vector3 v = mPlayerBehaviour.transform.position;
        v.y += 0.3f;
        t.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, v);
        t.fontSize = sizeOfFontEnergyUI * Vector2.Distance(RectTransformUtility.WorldToScreenPoint(Camera.main, new(1, 0, 0)), RectTransformUtility.WorldToScreenPoint(Camera.main, new(0, 0, 0)));
    }
    void UpdateIdentityUI(){
        var p = mPlayerBehaviour;
        var t = mIdentityUI;
        if(Time.time-startTime>2){Color c=t.color;c.a=0;t.color=c;}
        t.text = p.isRobot?("CPU" + (p.pid+1)):("P"+(p.pid+1));
        Vector3 v = mPlayerBehaviour.transform.position;
        v.y += 0.5f;
        t.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, v);
        t.fontSize = sizeOfFontEnergyUI * Vector2.Distance(RectTransformUtility.WorldToScreenPoint(Camera.main, new(1, 0, 0)), RectTransformUtility.WorldToScreenPoint(Camera.main, new(0, 0, 0)));
    }
    // Update is called once per frame
    void Update()
    {
        if (ManageGameManager.isPause) return;
        UpdateIdentityUI();
        UpdateEnergyUI();
    }
}
