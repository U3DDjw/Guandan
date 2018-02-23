using Common;
using DNotificationCenterManager;
using MsgContainer;
using Net;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZhiWa;

public class EffectAudio
{
    public AudioSource source = null;
    public string id;
}

public enum EAudioStyle
{
    ENull = 0,
    ENormalMan = 1,//普通话（男）
    ENormalWoman = 2,//普通话（女）
    ENJLocalMan = 3,//南京方言（男）
    ENJLocalWoMan = 4,//南京方言(女)
}
public class AudioManager : SingleTon<AudioManager>
{
    private EAudioStyle audioStyle = EAudioStyle.ENull;
    public EAudioStyle mAudioStyle //当前声音种类
    {
        get
        {
            if (audioStyle != EAudioStyle.ENull)
            {
                return audioStyle;
            }
            return PlayerInfo.Instance.GetSexIsMan ? EAudioStyle.ENormalMan : EAudioStyle.ENormalWoman;
        }
        set
        {
            audioStyle = value;
        }
    }

    private float curEffectVolume = 1.0f;
    public float mCurEffectVolume   //当前声效音量
    {
        get
        {
            return curEffectVolume;
        }

        set
        {
            curEffectVolume = Mathf.Clamp(value, 0, 1);
        }
    }



    private float curBgVolume = 1.0f;
    public float mCurBgVolume   //当前背景音量
    {
        get
        {
            return curBgVolume;
        }

        set
        {
            curBgVolume = Mathf.Clamp(value, 0, 1);
        }
    }

    const int AUDIO_STYLE_COUNT = 4;        //同时兼容声效最大数量 
    const string AUDIO_COMMMON_PATH = "Audio/Common/";    //公共音乐路径
    const string AUDIO_PROPS_PATH = "Audio/PropsAudio/";//道具音效路径
    const string AUDIO_CRAEDTWEEN_PATH = "Audio/CardTweenAudio/";//炸弹等音效
    string AUDIO_EFFECT_PATH
    {
        get
        {
            switch (mAudioStyle)
            {
                case EAudioStyle.ENJLocalMan:
                    return "Audio/";
                case EAudioStyle.ENJLocalWoMan:
                    return "Audio/";
                case EAudioStyle.ENormalMan:
                    return "Audio/";
                case EAudioStyle.ENormalWoman:
                    return "Audio/";
            }
            return "Audio/Sound_Woman/";
        }
    }
    private AudioSource mBackgroundSource = null;
    private List<EffectAudio> mEffectList = null;
    private int mCurrentIndex = 0; //当前播放id

    private bool mEffectState = true;
    public bool EffectState
    {
        set { mEffectState = value; }
        get { return mEffectState; }
    }

    private bool mBackgroundState = true;
    public bool BackgroundState
    {
        set { mBackgroundState = value; }
        get { return mBackgroundState; }
    }

    Dictionary<string, AudioClip> hasLoadAudioClip = new Dictionary<string, AudioClip>();
    //获取audioClip从之前加载过的
    AudioClip GetAudioClip(string fullPath)
    {
        if (hasLoadAudioClip.ContainsKey(fullPath))
        {
            return hasLoadAudioClip[fullPath];
        }
        else
        {
            AudioClip clip = ResourceManager.Instance.LoadAsset<AudioClip>(fullPath);
            hasLoadAudioClip.Add(fullPath, clip);
            return clip;
        }
    }
    public void Load()
    {
        mBackgroundSource = GameManager.Instance.mGameObj.AddComponent<AudioSource>();
        mEffectList = new List<EffectAudio>(AUDIO_STYLE_COUNT);
        for (int i = 0; i < AUDIO_STYLE_COUNT; i++)
        {
            EffectAudio audio = new EffectAudio();
            audio.source = GameManager.Instance.mGameObj.AddComponent<AudioSource>();
            mEffectList.Add(audio);
        }
    }


     void InitPlayBackgroundAudio(string fileName)
    {
        if (null == mBackgroundSource || string.IsNullOrEmpty(fileName)) return;
        //AudioClip clip = ResourceManager.Instance.LoadAsset<AudioClip>(AUDIO_COMMMON_PATH + fileName);
        AudioClip clip = GetAudioClip(AUDIO_COMMMON_PATH + fileName);
        if (null == clip) return;

        if (mBackgroundSource.isPlaying)
        {
            mBackgroundSource.Stop();
        }
        mBackgroundSource.clip = clip;
        mBackgroundSource.loop = true;
        mBackgroundSource.volume = mCurBgVolume;
        mBackgroundSource.playOnAwake = false;
        if (!BackgroundState) return;
        mBackgroundSource.Play(0);
    }

     void PlayBackgroundAudio()
    {
        if (null == mBackgroundSource || !BackgroundState) return;

        if (mBackgroundSource.clip == null) {
            InitPlayBackgroundAudio("audio_bg_main");
        }

        if (!mBackgroundSource.isPlaying)
        {
            mBackgroundSource.Play();
        }
    }

     void PauseBackgroundAudio()
    {
        if (null == mBackgroundSource || null == mBackgroundSource.clip) return;

        if (mBackgroundSource.isPlaying)
        {
            mBackgroundSource.Pause();
        }
    }


    public void PlayCardTweenAudio(string fileName)
    {
        string path = AUDIO_CRAEDTWEEN_PATH + fileName;
        Debug.Log(path);
        PlayAudio(path);
    }
    /// <summary>
    /// 表情音效
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="emotionId"></param>
    public void PlayerEmotionAudio(ulong playerId, uint emotionId)
    {
        EAudioStyle style = RoomInfo.Instance.GetPlayerAudioStyleByPlayerPos(RoomInfo.Instance.GetPlayerPosById(playerId));
        string prefixAudioName = RoomInfo.Instance.GetPlayerInfoById(playerId).sex == 1 ? "mcj" : "wcj";
        string path = AUDIO_EFFECT_PATH + GlobalData.GetAudioBasePathByStyle(style) + prefixAudioName + emotionId.ToString();
        PlayAudio(path);
    }
    /// <summary>
    /// 道具音效
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="toolsIndex"></param>
    public void PlayPropsAudio(uint toolsIndex)
    {
        string path = AUDIO_PROPS_PATH + "m_props_" + toolsIndex.ToString();
        PlayAudio(path);
    }
    public void PlayEffectAudio(string fileName)
    {
        string path = AUDIO_EFFECT_PATH + fileName;
        PlayAudio(path);
    }
    //播放音效
    void PlayAudio(string fileName)
    {
        ulong delay = 0;
        bool loop = false;
        if (null == mBackgroundSource || string.IsNullOrEmpty(fileName)) return;

        //AudioClip clip = ResourceManager.Instance.LoadAsset<AudioClip>(AUDIO_EFFECT_PATH + fileName);
        AudioClip clip = GetAudioClip(fileName);
        if (null == clip) return;

        mCurrentIndex++;
        if (mCurrentIndex >= AUDIO_STYLE_COUNT)
        {
            mCurrentIndex = 0;
        }
        AudioSource audio = mEffectList[mCurrentIndex].source;
        mEffectList[mCurrentIndex].id = fileName;
        if (audio.isPlaying)
        {
            audio.Stop();
        }
        audio.clip = clip;
        audio.loop = loop;
        audio.volume = mCurEffectVolume;
        audio.playOnAwake = false;
        if (!EffectState) return;
        audio.Play(delay);
    }
    public void ChangeEffectVolume(float volume)
    {

        for (int i = 0; i < mEffectList.Count; i++)
        {
            if (mEffectList[i].source.isPlaying)
                mEffectList[i].source.volume = volume;
        }
        mCurEffectVolume = volume;
    }

    public void ChangeBgVolume(float volume)
    {
        mBackgroundSource.volume = volume;
        mCurBgVolume = volume;
    }
    public void PlayEffectAudio()
    {
        if (!EffectState) return;

        for (int i = 0; i < mEffectList.Count; i++)
        {
            if (!mEffectList[i].source.isPlaying)
                mEffectList[i].source.Play();
        }
    }

    public void PauseEffectAudio()
    {
        for (int i = 0; i < mEffectList.Count; i++)
        {
            if (mEffectList[i].source.isPlaying)
                mEffectList[i].source.Pause();
        }
    }

    public void StopEffectAudio()
    {
        for (int i = 0; i < mEffectList.Count; i++)
        {
            if (mEffectList[i].source.isPlaying)
                mEffectList[i].source.Stop();
        }
    }

    public void StopEffectAudio(string id)
    {
        for (int i = 0; i < mEffectList.Count; i++)
        {
            if (mEffectList[i].id == null) { continue; }
            string[] str = mEffectList[i].id.Split('/');
            if (id == str[str.Length - 1])
            {
                if (mEffectList[i].source.isPlaying)
                    mEffectList[i].source.Stop();
            }
        }
    }

    // 根据BOOL值 暂停打开 背景音乐
    public void IsPlayBackgroundAudio(bool isPause)
    {
        if (isPause)
        {
            PlayBackgroundAudio();
        }
        else
        {
            PauseBackgroundAudio();
        }
    }

    /// <summary>
    /// 暂停/播放 当前所有音效
    /// </summary>
    /// <param name="isPause"></param>
    public void IsPauseAllEffectAudio(bool isPause)
    {
        if (false == isPause)
        {
            // PlayEffectAudio();
        }
        else
        {
            // PauseEffectAudio();
            StopEffectAudio();
        }
    }

    /// <summary>
    /// 点击按钮音效
    /// </summary>
    public void PlayClickBtnAudio()
    {
        PlayCommonAudio(GlobalData.AudioNameClickBtn);
    }
    public void PlayCommonAudio(string titleName)
    {
        string fileName = AUDIO_COMMMON_PATH + titleName;
        ulong delay = 0;
        bool loop = false;
        if (null == mBackgroundSource || string.IsNullOrEmpty(fileName)) return;

        //AudioClip clip = ResourceManager.Instance.LoadAsset<AudioClip>(fileName);
        AudioClip clip = GetAudioClip(fileName);
        if (null == clip) return;

        mCurrentIndex++;
        if (mCurrentIndex >= AUDIO_STYLE_COUNT)
        {
            mCurrentIndex = 0;
        }
        AudioSource audio = mEffectList[mCurrentIndex].source;
        mEffectList[mCurrentIndex].id = fileName;
        if (audio.isPlaying)
        {
            audio.Stop();
        }
        audio.clip = clip;
        audio.loop = loop;
        audio.volume = mCurEffectVolume;
        audio.playOnAwake = false;
        if (!EffectState) return;
        audio.Play(delay);
    }
    public void PlayCommonAudio(string titleName, bool isLoop)
    {
        string fileName = AUDIO_COMMMON_PATH + titleName;
        ulong delay = 0;
        bool loop = isLoop;
        if (null == mBackgroundSource || string.IsNullOrEmpty(fileName)) return;

        AudioClip clip = ResourceManager.Instance.LoadAsset<AudioClip>(fileName);

        if (null == clip) return;

        mCurrentIndex++;
        if (mCurrentIndex >= AUDIO_STYLE_COUNT)
        {
            mCurrentIndex = 0;
        }
        AudioSource audio = mEffectList[mCurrentIndex].source;
        mEffectList[mCurrentIndex].id = fileName;
        if (audio.isPlaying)
        {
            audio.Stop();
        }
        audio.clip = clip;
        audio.loop = loop;
        audio.volume = mCurEffectVolume;
        audio.playOnAwake = false;
        if (!EffectState) return;
        audio.Play(delay);
    }


    public void PlayWarningAudio()
    {
        PlayCommonAudio(GlobalData.AudioNameWarning);
    }

    /// <summary>
    /// 检查是否有常用语
    /// </summary>
    /// <param name="text">玩家发送的文本</param>
    /// <returns></returns>
    public void CheckHaveCommon(ulong id, string text)
    {
        string[] usefulExpres = GlobalData.mGetUsefulExpressions();
        for (int i = 0; i < usefulExpres.Length; i++)
        {
            if (text.Contains(usefulExpres[i]))
            {
                PlayerEmotionAudio(id, (uint)(i + 1));
                break;
            }
        }
    }

    public void SendTalkServer(string voiceName, uint time)
    {
        MsgGlobal mGl = new MsgGlobal();
        MsgGuandanRoom msg = mGl.guandan_room;
        msg.voiceName = voiceName;
        msg.msg_talk = new MsgTalk();
        msg.msg_talk.time = time;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_TALK, mGl);
    }


    /// <summary>
    /// 语音队列
    /// </summary>
    public Queue<ArgsTalk> mTalkQueues = new Queue<ArgsTalk>();
    /// <summary>
    /// 玩家语音正在播放
    /// </summary>
    float curTime = 0;
    float curTargetTime = 0;
    bool isTalkPlaying = false; //语音播报时。
    public void OnUpdate()
    {
        //if (mTalkQueues.Count > 0)
        //{
        //    if (curTime == 0)
        //    {
        //        ExecuteTalk(mTalkQueues.Peek());
        //        curTargetTime = mTalkQueues.Peek().time;
        //        mTalkQueues.Dequeue();
        //    }
        //    else
        //    {
        //        curTime += Time.deltaTime;
        //        if (curTime > curTargetTime)
        //        {
        //            curTime = 0;
        //            curTargetTime = 0;
        //        }
        //    }
        //}

        if (!isTalkPlaying)
        {
            if (mTalkQueues.Count > 0)
            {
                ExecuteTalk(mTalkQueues.Peek());
                curTargetTime = mTalkQueues.Peek().time;
                mTalkQueues.Dequeue();
                isTalkPlaying = true;
                //关闭其他音效

                IsPauseAllEffectAudio(true);
            }
        }

        if (isTalkPlaying)
        {
            curTime += Time.deltaTime;
            if (curTime > curTargetTime)
            {
                isTalkPlaying = false;
                curTime = 0;
                IsPauseAllEffectAudio(false);

                //恢复其他音效
            }
        }
    }



    void ExecuteTalk(ArgsTalk info)
    {
        ArgsTalk args = new ArgsTalk();
        args.voiceName = info.voiceName;
        args.talkPid = (uint)info.talkPid;
        SDKManager.Instance.yayaPlay(info.voiceName);

        // 表现谁在说话
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ETalkNotify, args);
    }
}

