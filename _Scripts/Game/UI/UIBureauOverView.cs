using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ZhiWa;
using Net;
using MsgContainer;
using HaoyunFramework;
using UnityEngine.UI;

public class BureauOverContext : BaseContext
{
    public BureauOverContext()
    {
        ViewType = UIType.BureauOverView;
    }
}
public class UIBureauOverView : BasesView
{

    [SerializeField]
    Transform parentContainer;
    GameObject bureauPrefabItem;

    [SerializeField]
    GameObject gameExitBtn;
   
    [SerializeField]
    GameObject closeBtn;
    string nextCard;//下一局要打几


    [SerializeField]
    Text autoOverLab;
    private void Start()
    {
       // EventTriggerListener.Get(gameExitBtn).onClick = OnGameExitClick;
       // EventTriggerListener.Get(closeBtn).onClick = OnGameExitClick;
    }

    bool isOver = false;
    bool isTotalOver = false;
    /// <summary>
    /// 重新开始下一局
    /// </summary>
    /// <param name="g"></param>
    void OnGameExitClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (GoldFiledManager.Instance.mIsGoldFiled)
        {
            GoldFiledExitMode();
        }
        else
        {
            RoomCardMode();
        }

        ContextManager.Instance.Pop(curContext);
    }


    void GoldFiledExitMode()
    {
        DNotificationCenterManager.NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGoldOverBureau);
    }

   

    void RoomCardMode()
    {
        if (RoomInfo.Instance.IsZhuandanGameType || RoomInfo.Instance.IsGuandan2GameType)
        {
            //转蛋
            Debug.Log(string.Format("打完第{0}局，总共，{1}局", PlayingGameInfo.Instance.mGameInfo.game_index, RoomInfo.Instance.mRoom.game_num));
            if (!isTotalOver)
            {
                PlayingGameInfo.Instance.EnterNextBureauGame();
            }
            else
            {
                MsgGlobal mG = new MsgGlobal();
                Debug.Log("Send Total roomId:" + RoomInfo.Instance.mRoomId);
                mG.room_id = RoomInfo.Instance.mRoomId;
                TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_TOTAL_GOAL, mG);
            }
        }
        else
        {
            Debug.Log("掼蛋非打二进入下一局");
            //如果结束就不会有这个界面
            if (isOver)
            {
                GameManager.Instance.BackToHomeModule();
            }
            else
            {
                PlayingGameInfo.Instance.EnterNextBureauGame();
            }
        }
    }
    /// <summary>
    /// 通过头游，获取下一玩的打几
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    uint GetNextCommonCard(List<MsgGameOverInfo> list)
    {
        IEnumerator<MsgGameOverInfo> iList = list.GetEnumerator();
        while (iList.MoveNext())
        {
            if (iList.Current.rank == MsgGuandanGameRank.MsgGuandanGameRankFirst)
            {
                return iList.Current.upgrade_card;
            }
        }
        return 2;
    }
    EPlayersTeam winTeam = EPlayersTeam.ENull;
    public void SetItems(List<MsgGameOverInfo> list, bool totalOver)
    {
        uint nextPlayCommonCardId = GetNextCommonCard(list);
        isTotalOver = totalOver;
        //RefreshViewInfo(nextPlayCommonCardId);
        bureauPrefabItem = ResourceManager.Instance.LoadAsset<GameObject>(UIType.BureauOverItem.Path);
        int overGoal = 0;
        for (int i = 0; i < list.Count; i++)
        {
            GameObject item = GameObject.Instantiate(bureauPrefabItem);
            item.transform.SetParent(parentContainer);
            RawImage tex = parentContainer.GetComponent<RawImage>();
            item.transform.localPosition = new Vector3(0, -30 - 140 * i, 0);
            item.transform.localScale = Vector3.one;
            if (winTeam == EPlayersTeam.ENull)
                winTeam = list[i].rank == MsgGuandanGameRank.MsgGuandanGameRankFirst ? (EPlayersTeam)RoomInfo.Instance.GetPlayerInfoById(list[i].player_id).teamType : EPlayersTeam.ENull;
            item.GetComponent<UIBureauOverItem>().SetData(list[i], winTeam);
            //if (list[i].player_id == PlayerInfo.Instance.mPlayerPid)
            //{
            //    overGoal = (int)list[i].goal;
            //    lightSpr.spriteName = overGoal > 0 ? "settlement_light1" : "settlement_light4";
            //}
            isOver = list[0].over == 1;
            Debug.Log("Over:" + list[i].over);
        }
        CheckPlayAudio(overGoal);
    }
    void CheckPlayAudio(int overGoal)
    {
        string fileName = "";

        if (overGoal < 0)
        {
            fileName = GlobalData.AudioNameGameOverLose;
        }
        else if (overGoal == 0)
        {
            fileName = GlobalData.AudioNameGameOverPingju;
        }
        else
        {
            fileName = GlobalData.AudioNameGameOverWin;
        }
        AudioManager.Instance.PlayCommonAudio(fileName);
    }

    float curTime = 0;
    private void Update()
    {
        curTime += Time.deltaTime;
        int endS = Mathf.Clamp((int)(GlobalData.mGameBureauOverWaitTime - curTime), 0, 10);
        autoOverLab.text = string.Format("{0}秒后，自动点击", endS);
        if (curTime > GlobalData.mGameBureauOverWaitTime)
        {
            //Destroy(this.gameObject);
            OnGameExitClick(null);
        }
    }
}
