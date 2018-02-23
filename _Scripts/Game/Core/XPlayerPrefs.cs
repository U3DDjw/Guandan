using MsgContainer;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


/// <summary>
/// 缓存数据
/// </summary>
public class XPlayerPrefs : MonoBehaviour
{
    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        InitLocalCatch();
    }
    //加载本地缓存
    void InitLocalCatch()
    {
        AudioManager.Instance.ChangeEffectVolume(XPlayerPrefs.Instance.mAudioEffectVolume);
        AudioManager.Instance.ChangeBgVolume(XPlayerPrefs.Instance.mAudioBgVolume);
    }
    //是否加载公告，刚进游戏加载一次
    public static bool IsLoadNotic
    {
        get; set;
    }


    private static XPlayerPrefs instance;

    public static XPlayerPrefs Instance
    {
        get
        {
            return instance;
        }
    }


    private string KeyPlayerToken = "Key_Player_Token";
    /// <summary>
    /// 玩家Token缓存
    /// </summary>
    public string mPlayerToken
    {
        get
        {
            if (PlayerPrefs.HasKey(KeyPlayerToken))
            {
                return PlayerPrefs.GetString(KeyPlayerToken);
            }

            return "";
        }

        set
        {
            PlayerPrefs.SetString(KeyPlayerToken, value);
        }
    }


    private string KeyAudioBgVolume = "key_audio_bg_volume";
    /// <summary>
    /// 背景音乐音量
    /// </summary>
    public float mAudioBgVolume
    {
        get
        {
            if (PlayerPrefs.HasKey(KeyAudioBgVolume))
            {
                return PlayerPrefs.GetFloat(KeyAudioBgVolume);
            }
            return 1;
        }

        set
        {
            PlayerPrefs.SetFloat(KeyAudioBgVolume, value);
        }
    }

    private string KeyAudioEffectVolume = "key_audio_effect_volume";
    /// <summary>
    /// 音效音量
    /// </summary>
    public float mAudioEffectVolume
    {
        get
        {
            if (PlayerPrefs.HasKey(KeyAudioEffectVolume))
            {
                return PlayerPrefs.GetFloat(KeyAudioEffectVolume);
            }
            return 1;
        }

        set
        {
            PlayerPrefs.SetFloat(KeyAudioEffectVolume, value);
        }
    }
    private string KeyAudioTypeVolume = "key_audio_type";
    /// <summary>
    ///音效类型
    /// </summary>
    public string mAudioType
    {
        get
        {
            if (PlayerPrefs.HasKey(KeyAudioTypeVolume))
            {
                return PlayerPrefs.GetString(KeyAudioTypeVolume);
            }
            return "普通话";
        }
        set
        {
            PlayerPrefs.SetString(KeyAudioTypeVolume, value);
        }
    }
    private string KeyGameType = "key_game_type";
    /// <summary>
    ///游戏类型
    /// </summary>
    public int mGameType
    {
        get
        {
            if (PlayerPrefs.HasKey(KeyGameType))
            {
                return PlayerPrefs.GetInt(KeyGameType);
            }
            return 1;//转蛋
        }
        set
        {
            PlayerPrefs.SetInt(KeyGameType, value);
        }
    }
    private string KeyGameNum = "key_game_num";
    /// <summary>
    ///游戏局数
    /// </summary>
    public int mGameNum
    {
        get
        {
            if (PlayerPrefs.HasKey(KeyGameNum))
            {
                return PlayerPrefs.GetInt(KeyGameNum);
            }
            return 4;//四局
        }
        set
        {
            PlayerPrefs.SetInt(KeyGameNum, value);
        }
    }
    private string KeyGamePay = "key_game_pay";
    /// <summary>
    ///付费方式
    /// </summary>
    public int mGamePay
    {
        get
        {
            if (PlayerPrefs.HasKey(KeyGamePay))
            {
                return PlayerPrefs.GetInt(KeyGamePay);
            }
            return 1;//AA
        }
        set
        {
            PlayerPrefs.SetInt(KeyGamePay, value);
        }
    }
    private string KeyPlaySingleOrMulity = "key_play_single_mulity";
    /// <summary>
    ///打牌方式，选择单张或者多张，默认单张 (0为单张)
    /// </summary>
    public int mGamePlaySingleOrMulity
    {
        get
        {
            if (PlayerPrefs.HasKey(KeyPlaySingleOrMulity))
            {
                return PlayerPrefs.GetInt(KeyPlaySingleOrMulity);
            }
            return 0;//单张.默认值
        }
        set
        {
            PlayerPrefs.SetInt(KeyPlaySingleOrMulity, value);
        }
    }
    //缓存聊天常用
    private string Key_ChatMessageType = "key_ChatMessageType";
    public int mChatMessgaeType
    {
        get
        {
            if (PlayerPrefs.HasKey(Key_ChatMessageType))
            {
                return PlayerPrefs.GetInt(Key_ChatMessageType);
            }
            return 0;//表示语音输入信息 1表示打字输入信息
        }
        set
        {
            PlayerPrefs.SetInt(Key_ChatMessageType, value);
        }
    }
    /// <summary>
    /// 战绩查询常用
    /// </summary>
    private string Key_RecordType = "key_RecordType";
    public int mRecordType
    {
        get
        {
            if (PlayerPrefs.HasKey(Key_ChatMessageType))
            {
                return PlayerPrefs.GetInt(Key_ChatMessageType);
            }
            return 0;//没有
        }
        set
        {
            PlayerPrefs.SetInt(Key_ChatMessageType, value);
        }
    }
    /// <summary>
    /// 初次加载
    /// </summary>
    private string key_FristLoad = "key_FristLoad";
    public int mFristLoad
    {
        get
        {
            if (PlayerPrefs.HasKey(key_FristLoad))
            {
                return PlayerPrefs.GetInt(key_FristLoad);
            }
            return 1;//true
        }
        set
        {
            PlayerPrefs.SetInt(key_FristLoad, value);
        }
    }

    /// <summary>
    /// 退出记录的时间 的类型 同ip提示_0，解散房间_1。。。
    /// </summary>
    private string keyRecordTimeType = "keyRecordTimeType";
    public int mKeyRecordTimeType
    {
        get
        {
            if (PlayerPrefs.HasKey(keyRecordTimeType))
            {
                return PlayerPrefs.GetInt(keyRecordTimeType);
            }
            return 0;
        }
        set
        {
            PlayerPrefs.SetInt(keyRecordTimeType, value);
        }
    }
    /// <summary>
    /// 退出房间时候的时间
    /// </summary>
    private string keyExitTime = "keyExitTime";
    public string mKey_ExitTime
    {
        get
        {
            if (PlayerPrefs.HasKey(keyExitTime))
            {
                return PlayerPrefs.GetString(keyExitTime);
            }
            else
            {
                return "";
            }

        }
        set
        {
            PlayerPrefs.SetString(keyExitTime, value);
        }
    }
    /// <summary>
    /// 退出时候的时间还剩多少秒
    /// </summary>
    private string key_ResidueTime = "key_SameIpExitTime";
    public float mkey_ResidueTime
    {
        get
        {
            if (PlayerPrefs.HasKey(key_ResidueTime))
            {
                return PlayerPrefs.GetFloat(key_ResidueTime);
            }
            return 0;//true
        }
        set
        {
            PlayerPrefs.SetFloat(key_ResidueTime, value);
        }
    }
    /// <summary>
    /// 退出房间时候的房间号
    /// </summary>
    private string key_RoomCode = "key_RoomCode";
    public int mKey_RoomCode
    {
        get
        {
            if (PlayerPrefs.HasKey(key_RoomCode))
            {
                return PlayerPrefs.GetInt(key_RoomCode);
            }
            return 0;//true
        }
        set
        {
            PlayerPrefs.SetInt(key_RoomCode, value);
        }
    }
    public void ClearSameIpTimeLinkPlayerPrefs()
    {
        mKeyRecordTimeType = 0;
        mKey_ExitTime = "";
        mKey_RoomCode = 0;
        mkey_ResidueTime = 0;
    }

    public void ClearAllInfo()
    {
        mPlayerToken = "";
        PlayerPrefs.DeleteAll();
    }
}
