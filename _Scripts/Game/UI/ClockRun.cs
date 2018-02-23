using UnityEngine;
using System.Collections;
using DNotificationCenterManager;
using UnityEngine.UI;

public class ClockRun : MonoBehaviour {

    [SerializeField]
    Text timeLab;

    EPlayerPositionType curPosTYpe = EPlayerPositionType.ENull;


    //加一个轮旋的自动刷新

    private void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EInitLicense, ResetClock);
    }

    private void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EInitLicense, ResetClock);
    }
    void ResetClock(LocalNotification e)
    {
         curPosTYpe = EPlayerPositionType.ENull;
    }

    public void ResetStartRun(EPlayerPositionType type)
    {
        if (this.curPosTYpe != type)
        {
            curTime = 0;
            startRunId = 1;
            frameCount = 0;
        }
        this.curPosTYpe = type;
    }
    int startRunId = 0;//0,默认，1计时结束，2开始轮旋
    float curTime;
    float totalTime = 10f;

    int frameCount = 0;
    int eachFrameCount = 100 * 5;//10S(0.02秒一帧，500*0.02 == 10s )
    void FixedUpdate()
    {
        if(startRunId ==1)
        {
            curTime += Time.deltaTime;
            timeLab.text = ((int)(totalTime - curTime)).ToString();
            if(curTime>totalTime+0.1f)
            {
                startRunId = 2;
            }
        }
        else if(startRunId ==2)
        {
            //开始每隔10s，主动刷新一次
            frameCount++;
            if(frameCount% eachFrameCount ==0 || frameCount ==1) //每10s刷新一次
            { 
                if (PlayingGameInfo.Instance.mPlayingStatus == EPlayingStatus.ETribute)
                {
                    return;
                }
                PlayingGameInfo.Instance.SendRefreshGameServer();
            }

        }

    }
}
