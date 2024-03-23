using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PestAndFruitProducer : MonoBehaviour
{
    private float PestsProbability;
    private float FruitsProbability;
    private GameServer mGameServer;
    // Start is called before the first frame update
    void Start()
    {
        mGameServer = Camera.main.GetComponent<GameServer>();
        PestsProbability=0.01f;
        FruitsProbability=0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        int n=mGameServer.n;
        for(int i=0;i<=2*n;++i){
            for(int j=0;j<=2*n;++j){
                if(i-j<=0&&j-i<=n){
                    bool spawn_disabled=false;
                    spawn_disabled|=!mGameServer.LBmap[i][j].nearRoot;
                    spawn_disabled|=mGameServer.LBmap[i][j].mPest!=null;
                    for(int k=0;k<mGameServer.PlayerNumber;k++){
                        Vector2Int p=new(i,j);
                        spawn_disabled|=mGameServer.players[k].curpos==p;
                    }
                    if(mGameServer.LBmap[i][j].owner!=-1) spawn_disabled|=new Vector2Int(i,j) == mGameServer.PosToCell(mGameServer.bornPos[mGameServer.LBmap[i][j].owner]);
                    if(!spawn_disabled&&Random.Range(0f,1f)<PestsProbability*Time.smoothDeltaTime){
                        var t = mGameServer.LBmap[i][j];
                        t.mPest=Instantiate(Resources.Load("Pest")as GameObject);
                        var v=t.transform.localPosition;
                        v.z=-3;
                        v.y+=0.2f;
                        t.mPest.transform.localPosition=v;
                        spawn_disabled=true;
                        if(mGameServer.LBmap[i][j].mFruit!=null)
                        {
                            Destroy(mGameServer.LBmap[i][j].mFruit);
                            mGameServer.LBmap[i][j].mFruit=null;
                        }
                    }
                    spawn_disabled|=mGameServer.LBmap[i][j].mFruit!=null;
                    if(!spawn_disabled&&Random.Range(0f,1f)<FruitsProbability*Time.smoothDeltaTime){
                        var t = mGameServer.LBmap[i][j];
                        t.mFruit=Instantiate(Resources.Load("Fruit")as GameObject);
                        t.mFruit.GetComponent<FruitBehavior>().owner=t;
                        var v=t.transform.localPosition;
                        v.z=-3;
                        v.y+=0.2f;
                        t.mFruit.transform.localPosition=v;
                    }
                }
            }
        }
    }
}
