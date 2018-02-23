using UnityEngine;
using System.Collections;
using Common;
using MsgContainer;
using DNotificationCenterManager;
using Net;
using System.IO;
using UnityEngine.UI;
using HaoyunFramework;

public class GameManager : SingleTon<GameManager>
{
    public bool mIsOpenDebugInMobile = false;
    public bool mIsHotUpdate = false;
    public EGameMode mGameMode = EGameMode.EDebug;
    public EGameChanel mGameChanel = EGameChanel.EHY;
    public bool mIsFormalResServer = false;

    public GameObject mGameObj;
    //当前游戏状态,游戏状态控制游戏模块，游戏模块控制游戏view的加载
    public bool mIsRequesting = false; //用于出牌请求
    private EGameStatus curGameStatus = EGameStatus.EInit;

    public EGameStatus mCurGameStatus
    {
        get
        {
            return curGameStatus;
        }
        set
        {
            curGameStatus = value;
        }
    }
    public bool mIsUseAI = true;

    public bool mIsLuaUpdate = false;


    public EGameServerUrl mGameUrlType = EGameServerUrl.ESCZ;
    public bool mIsBackToLoginModule = false;
    private BaseModule currentModule = new LoadingModule();
    public void SwitchGameStatus(EGameStatus status)
    {
     //   if (mCurGameStatus == status) { return; }
        mCurGameStatus = status;
        SwitchGameModule();
    }


    /// <summary>
    /// 清空当前开的游戏
    /// </summary>
    public void ClearCurGame()
    {
        RoomInfo.Instance.ClearInfo();
        PlayingGameInfo.Instance.ClearAllGameInfo();
        SelfCardsManager.Instance.ClearInfo();
        Debug.Log("清空游戏数据");
    }

    public void BackToHomeModule()
    {
        TweenManager.Instance.ClearEmoticonList();
        TweenManager.Instance.ClearPropList();
        ContextManager.Instance.Pop(UIType.PlayingGameView.Name);
        ClearCurGame();
        mCurGameStatus = EGameStatus.EHome;
        currentModule = new HomeModule();
        AudioManager.Instance.IsPlayBackgroundAudio(true);
        Debug.Log("返回主界面");

        //偶而出现消失主界面
      //  ContextManager.Instance.Push(new HomeContext());
    }

    void SwitchGameModule()
    {
        switch (mCurGameStatus)
        {
            case EGameStatus.ELoading:
                currentModule = new LoadingModule(); //这只是一个进度条
                break;
            case EGameStatus.ELogin:
                currentModule = new LoginModule();//登录模块
                break;
            case EGameStatus.EHome:
                currentModule = new HomeModule(); //主界面
                break;
            case EGameStatus.EPlaying:
                currentModule = new PlayingModule(); //战斗状态
                break;
            default:
                Debug.LogError("状态切换错误");
                break;
        }

        if (currentModule != null)
        {
            currentModule.LoadModule();
        }
    }


    ///设置射线机照射的纹理的大小
    public void SetRenderTexture()
    {
        string[] renderPaths = GlobalData.mRenderTexturePaths();
        for (int i = 0; i < renderPaths.Length; i++)
        {
            RenderTexture renderTex = ResourceManager.Instance.LoadAsset<RenderTexture>(renderPaths[i]);
            renderTex.width = Screen.width;
            renderTex.height = Screen.height;
        }
    }
 
}
