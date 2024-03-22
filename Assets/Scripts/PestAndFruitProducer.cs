using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PestAndFruitProducer : MonoBehaviour
{
    public float PestsProbability=0.002f;
    public float FruitsProbability=0.01f;
    public GameServer mGameServer=null;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(mGameServer!=null);
    }

    // Update is called once per frame
    void Update()
    {
        int n=mGameServer.n;
        for(int i=0;i<=2*n;++i){
            for(int j=0;j<=2*n;++j){
                if(i-j<=0&&j-i<=n){
                    if(!mGameServer.LBmap[i][j].nearRoot)continue;
                    if(mGameServer.LBmap[i][j].mPest!=null||mGameServer.LBmap[i][j].mFruit!=null)continue;
                    if(Random.Range(0f,1f)>FruitsProbability*Time.smoothDeltaTime){
                        mGameServer.LBmap[i][j].mFruit=Instantiate(Resources.Load("Fruits")as GameObject);
                    }
                    else if(Random.Range(0f,1f)>PestsProbability*Time.smoothDeltaTime){
                        mGameServer.LBmap[i][j].mPest=Instantiate(Resources.Load("Pests")as GameObject);
                    }
                }
            }
        }
    }
}
