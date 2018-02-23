using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using HaoyunFramework;
using DNotificationCenterManager;

public class SettingContext : BaseContext
{
    public SettingContext()
    {
        ViewType = UIType.SettingView;
    }
}
public class UISettingView : BasesView
{

    //[SerializeField]
    //GameObject exitBtn;
    [SerializeField]
    GameObject closeBtn;
    [SerializeField]
    GameObject maskBg;

    [SerializeField]
    Image soundBtn;
    [SerializeField]
    Image musicBtn;

    [SerializeField]
    Slider sliderSound;
    [SerializeField]
    Slider sliderMusic;

    //[SerializeField]
    //Text defaultLab;
    [SerializeField]
    Button exitBtn;
    private void Start()
    {
        EventTriggerListener.Get(closeBtn).onClick = OnCloseClick;
        EventTriggerListener.Get(maskBg).onClick = OnCloseClick;
        EventTriggerListener.Get(exitBtn.gameObject).onClick = OnExitLoginClick;

        EventTriggerListener.Get(musicBtn.gameObject).onClick = OnMusicClick;
        EventTriggerListener.Get(soundBtn.gameObject).onClick = OnSoundClick;

        Slider.SliderEvent musicEvent = new Slider.SliderEvent();
        musicEvent.AddListener(OnMusicValueChange);
        sliderMusic.onValueChanged = musicEvent;

        Slider.SliderEvent soundEvent = new Slider.SliderEvent();
        soundEvent.AddListener(OnSoundValueChange);
        sliderSound.onValueChanged = soundEvent;

        Init();
    }
    public void OnSoundClick(GameObject g)
    {
        bool isSlice = XPlayerPrefs.Instance.mAudioEffectVolume == 0;
        float value = isSlice ? 1 : 0;
        ShowSound(value);
    }
    public void OnMusicClick(GameObject g)
    {
        bool isSlice = XPlayerPrefs.Instance.mAudioBgVolume == 0;
        float value = isSlice ? 1 : 0;
        ShowMusic(value);
    }
    private void OnDestroy()
    {
        XPlayerPrefs.Instance.mAudioEffectVolume = sliderSound.value;
        XPlayerPrefs.Instance.mAudioBgVolume = sliderMusic.value;
        ContextManager.Instance.Pop(curContext);
        //XPlayerPrefs.Instance.mAudioType = defaultLab.text;
        //EventDelegate.Remove(popList.onChange, OnpopListValueChange);
    }
    public void Init()
    {
        ShowSound(XPlayerPrefs.Instance.mAudioEffectVolume);
        ShowMusic(XPlayerPrefs.Instance.mAudioBgVolume);
        string s = XPlayerPrefs.Instance.mAudioType;
        //popList.value = s;
        //defaultLab.text = s;
        //OnpopListValueChange();//为了赋值音效类型
        if (GameManager.Instance.mCurGameStatus == EGameStatus.EPlaying)
        {
            exitBtn.gameObject.SetActive(false);
        }
    }
    void ShowSound(float value)
    {
        AudioManager.Instance.ChangeEffectVolume(sliderSound.value);
        XPlayerPrefs.Instance.mAudioEffectVolume = sliderSound.value;

        sliderSound.value = value;
        Sprite soundSpr = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, value == 0 ? "setup_sound_no" : "setup_sound");
        soundBtn.sprite = soundSpr;
    }
    void ShowMusic(float value)
    {
        AudioManager.Instance.ChangeBgVolume(sliderMusic.value);
        XPlayerPrefs.Instance.mAudioBgVolume = sliderMusic.value;

        sliderMusic.value = value;
        Sprite soundSpr = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, value == 0 ? "setup_music_no" : "setup_music");
        musicBtn.sprite = soundSpr;
    }
    void OnExitLoginClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Pop(curContext);//需要先释放掉当前的view
        RoomInfo.Instance.SendExitRoomServer();
        XPlayerPrefs.Instance.mPlayerToken = "";
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EBackToLoginScene);
    }
    public void OnSoundValueChange(float value)
    {
        ShowSound(sliderSound.value);
        //AudioManager.Instance.ChangeEffectVolume(sliderSound.value);
    }
    public void OnMusicValueChange(float value)
    {
        ShowMusic(sliderMusic.value);
        //AudioManager.Instance.ChangeBgVolume(sliderMusic.value);

    }

    //void OnpopListValueChange()
    //{
    //    defaultLab.text = popList.value;
    //    //  Debug.Log(defaultLab.text);
    //    switch (defaultLab.text)
    //    {

    //        case "普通话":
    //            AudioManager.Instance.mAudioStyle = PlayerInfo.Instance.GetSexIsMan ? EAudioStyle.ENormalMan : EAudioStyle.ENormalWoman;
    //            break;

    //        case "南京方言":
    //            AudioManager.Instance.mAudioStyle = PlayerInfo.Instance.GetSexIsMan ? EAudioStyle.ENJLocalMan : EAudioStyle.ENJLocalWoMan;
    //            break;
    //    }
    //}

    void OnCloseClick(GameObject gm)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        Destroy(this.gameObject);
    }
}
