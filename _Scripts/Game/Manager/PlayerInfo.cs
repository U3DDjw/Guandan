using UnityEngine;
using System.Collections;
using Common;
using ZhiWa;
using DNotificationCenterManager;
using System.Collections.Generic;
using Umeng;
using MsgContainer;
using DG.Tweening;
using Spine.Unity;
using Haoyun.Utils;
using System;
/// <summary>
/// 玩家数据类，角色所有数据保存该类
/// </summary>
public class PlayerInfo : SingleTon<PlayerInfo>
{
    /// <summary>
    /// 亮牌动画播放完毕
    /// </summary>
    public bool IsSingleCardTweenEnd
    {
        set
        {
            if (value)
            {
                selfStatusContainer.SetActive(true);
            }
        }
    }
    public GameObject selfStatusContainer { get; set; }//己方的不出等按钮的容器
    /// <summary>
    /// 打几索引
    /// </summary>
    private string teamSelfIndex = "2";
    public string mTeamSelfIndex
    {
        get
        {
            teamSelfIndex = MsgContainer.GlobalData.StringUpgradeIndex(teamSelfIndex);
            return teamSelfIndex;
        }
        set
        {
            teamSelfIndex = value;
        }
    }
    private string teamEnemyIndex = "2";
    public string mTeamEnemyIndex
    {
        get
        {
            teamEnemyIndex = MsgContainer.GlobalData.StringUpgradeIndex(teamEnemyIndex);
            return teamEnemyIndex;
        }
        set { teamEnemyIndex = value; }
    }


    //纹理的url
    private TextureURLData urlData;
    public TextureURLData mUrlData
    {
        get { return urlData; }
        set { urlData = value; }
    }


    /// <summary>
    /// 是否进行身份验证过
    /// </summary>
    public bool mIsSfzFlg = false;

    //是否展示已验证身份
    public bool mIsSfzShow = false;

    private PlayerData playerData;
    public PlayerData mPlayerData
    {
        get
        {
            return playerData;
        }

        set
        {
            if (playerData == null)
            {
                playerData = new PlayerData();
            }
            playerData = value;
        }
    }


    public string GetServerIp
    {
        get
        {
            if(mFightServer!=null)
            {
                return mFightServer.host;
            }
            return "";
        }
    }

    public int GetServerPort
    {
        get
        {
            if(mFightServer!=null)
            {
                return mFightServer.port;
            }

            return 0;
        }
    }

    public string GetIp
    {
        get
        {
            if (mPlayerData != null)
            {
                return mPlayerData.ip;
            }
            return "error";
        }
    }

    public int GetPort
    {
        get
        {
            if(mPlayerData != null)
            {
                return mPlayerData.port;
            }
            return 0;
        }
    }

    public FightServerData mFightServer;
    public MsgGameInfo mGameInfo;


    public string mUuid
    {
        get
        {
            return mPlayerData.uuid;
        }
    }

    public bool GetSexIsMan
    {
        get
        {
            return mPlayerData.sex == 1;
        }
    }




    public ulong mPlayerPid
    {
        get
        {
            if (mPlayerData != null)
            {
                return mPlayerData.pid;
            }
            return 0;
        }
    }

    public string GetUDPServerHost
    {
        get
        {
            if (mFightServer != null)
            {
                //  return "hygg.3399hy.com";l;
                return mFightServer.host;
            }
            return "";
        }
    }

    public int GetUDPServerPort
    {
        get
        {
            if (mFightServer != null)
            {
                return mFightServer.port;
            }
            return 0;
        }
    }



    /// <summary>
    /// 金币数
    /// </summary>
    public int GetCurGold
    {
        get
        {
            if (mPlayerData.money < 0)
            {
                mPlayerData.money = 0;
            }
            return mPlayerData.money;
        }
    }

    public void UpdateGold(int num)
    {
        mPlayerData.money += num;
        RefreshPlayerInfoData();
    }

    public int GetRoomCard
    {
        get
        {
            if (mPlayerData.roomCardNum < 0)
            {
                mPlayerData.roomCardNum = 0;
            }
            return mPlayerData.roomCardNum;
        }
    }

    /// <summary>
    /// 增加游戏中货币，豆子
    /// </summary>
    /// <param name="num"></param>
    public void AddRoomCard(int num)
    {
        if (num > 0) //增加房卡
        {
            GA.Buy(GlobalData.mHostFkId, num, 1);
        }
        else if (num < 0) //消耗房卡
        {
            GA.Use(GlobalData.mHostFkId, -num, 1);// 友盟
            TDGAItem.OnPurchase(GlobalData.mHostFkId, -num, 1);
        }
        mPlayerData.roomCardNum += num;
        RefreshPlayerInfoData();
    }
    public void ServerRoomCardNum(uint num)
    {
        mPlayerData.roomCardNum = (int)num;
        RefreshPlayerInfoData();
    }
    public void ServerMoneyNum(uint num)
    {
        mPlayerData.money = (int)num;
        RefreshPlayerInfoData();
    }
    #region 
    private float intervalTime = 5;
    /// <summary>
    /// 是否可以发送表情()
    /// </summary>
    /// <param name="time">Time.startUpTime</param>
    public bool IsCanSendEmoticon(float time)
    {
        return false;
    }
    #endregion 
    private Dictionary<EPlayerPositionType, GameObject> dicEmoticonPos = new Dictionary<EPlayerPositionType, GameObject>();
    private Sequence se = null;
    public void AddNewEmoticon(EPlayerPositionType pos, GameObject emoticon, bool isEmoticon)
    {
        if (dicEmoticonPos.ContainsKey(pos))
        {
            if (dicEmoticonPos[pos] != null)
            {
                se.Kill();
                GameObject.Destroy(dicEmoticonPos[pos]);//删除当前的表情
                dicEmoticonPos[pos] = null;
            }
            dicEmoticonPos[pos] = emoticon;
        }
        else
        {
            dicEmoticonPos.Add(pos, emoticon);
        }
        if (isEmoticon)
            DestroyEmoticon(emoticon, pos);
    }
    public void DestroyEmoticon(GameObject emoticonTweenGm, EPlayerPositionType type)
    {
        se = DOTween.Sequence();
        float destroyTime = TweenManager.Instance.GetAnimatorTime(emoticonTweenGm, "play");
        se.AppendInterval(destroyTime * 2);
        se.AppendCallback(delegate
        {
            RemoveEmoticonGmFormDic(type);
            se = null;
        });

    }
    public void RemoveEmoticonGmFormDic(EPlayerPositionType pos)
    {
        if (dicEmoticonPos.ContainsKey(pos))
        {
            GameObject.Destroy(dicEmoticonPos[pos]);//删除当前的表情
            dicEmoticonPos[pos] = null;
        }
    }
    public float PlayerPrefsTime()
    {
        //再次进入房间 ，房间号肯定会改变，所以并不会出现问题
        if (XPlayerPrefs.Instance.mKeyRecordTimeType != 1 || XPlayerPrefs.Instance.mKey_RoomCode == 0)
        {
            XPlayerPrefs.Instance.ClearSameIpTimeLinkPlayerPrefs();//上一次
            return 0;
        }
        float hasUsedTime = 0;
        if (XPlayerPrefs.Instance.mKey_RoomCode == RoomInfo.Instance.mRoomCode)
        {
            long exitTimeBin;
            long.TryParse(XPlayerPrefs.Instance.mKey_ExitTime, out exitTimeBin);
            DateTime exitTime = System.DateTime.FromBinary(exitTimeBin);
            int totalSeconds = TimeUtils.GetDateTimeIntervalSeconds(exitTime, DateTime.Now);
            float hasSpendTime = GlobalData.mSameIpTotalTime - XPlayerPrefs.Instance.mkey_ResidueTime;
            hasUsedTime = totalSeconds + hasSpendTime;
        }  
        XPlayerPrefs.Instance.ClearSameIpTimeLinkPlayerPrefs();
        return hasUsedTime;
    }
    #region 跑马灯
    private List<HorseData> horseDataList = null;
    public List<HorseData> mHorseDataList
    {
        get
        {
            return horseDataList;
        }
    }

    public void UpdateHorseData(List<HorseData> list)
    {
        horseDataList = new List<HorseData>();
        horseDataList = list;
    }

    public void UpdateCurHorseData(HorseData data)
    {
        horseDataList.Add(data);
        mCurHorseDataIdx = horseDataList.Count - 1;
    }

    public int mCurHorseDataIdx = 0;
    public HorseData GetCurHorseData()
    {
        if (horseDataList != null && horseDataList.Count > 0)
        {
            return horseDataList[mCurHorseDataIdx];
        }
        return null;
    }

    public void RefreshPlayerInfoData()
    {
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EUpdatePlayerInfo);
    }
    #endregion

}
