using UnityEngine;
using System.Collections;
using MsgContainer;
using DNotificationCenterManager;
using UnityEngine.UI;
using HaoyunFramework;
public class EditPlayerInfoContext : BaseContext
{
    public EditPlayerInfoContext()
    {
        ViewType = UIType.EditPlayerInfoView;
    }
}
public class UIEditPlayerInfoView : BasesView
{

    //[SerializeField]
    //GameObject headBtn;
    //[SerializeField]
    //GameObject nameBtn;
    [SerializeField]
    GameObject closeBtn;
    [SerializeField]
    GameObject copyBtn;
    [SerializeField]
    Text nameLab;
    [SerializeField]
    RawImage headtexture;
    [SerializeField]
    Text id;
    [SerializeField]
    GameObject maskBg;
    [SerializeField]
    Image sexSpr;
    [SerializeField]
    Text ipAds;
    [SerializeField]
    Text ip;
    string manSpritename = "texture_head_1";
    string womanSpriteName = "texture_head_2";
    private void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EUpdatePlayerInfo, UpdatePlayerInfo);
    }

    private void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EUpdatePlayerInfo, UpdatePlayerInfo);

    }
    void UpdatePlayerInfo(LocalNotification e)
    {
        InitData();
    }
    // Use this for initialization
    void Start()
    {
        //UIEventListener.Get(headBtn).onClick = OnHeadClick;
        //UIEventListener.Get(nameBtn).onClick = OnNameClick;
        EventTriggerListener.Get(maskBg).onClick = OnCloseClick;
        EventTriggerListener.Get(copyBtn).onClick = OnCopyClick;
        EventTriggerListener.Get(closeBtn).onClick = OnCloseClick;
        InitData();
    }
    void InitData()
    {
        nameLab.text = PlayerInfo.Instance.mPlayerData.name;
        headtexture.texture = DataManager.Instance.GetTextureByName(ETextureName.EHeadTex1);
        id.text = /*"ID: " +*/ PlayerInfo.Instance.mPlayerPid.ToString();
        ipAds.text = PlayerInfo.Instance.mPlayerData.ipAds;
        ip.text = PlayerInfo.Instance.GetIp;
        Debug.Log("玩家的性别：" + PlayerInfo.Instance.mPlayerData.sex);
        sexSpr.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, PlayerInfo.Instance.GetSexIsMan ? "porfile_man" : "porfile_woman");
    }

    void OnCopyClick(GameObject g)
    {
        string idText = id.text;

        idText = idText.Substring(idText.LastIndexOf(":") + 1);
        GlobalData.CopyTextFromLab(idText.Trim());
    }
    void OnCloseClick(GameObject g)
    {
        ContextManager.Instance.Pop(curContext);
    }

    //void OnHeadClick(GameObject g)
    //{
    //    UIManager.Instance.LoadView("EditHeadView");
    //}

    //void OnNameClick(GameObject g)
    //{
    //    UIManager.Instance.LoadView("EditNameView");
    //}
    #region 备用
    //private void SendHeadSpriteName()
    //{
    //    //headSpr.spriteName = selectOne.mSelectTeam == ESelectType.EA ? manSpritename : womanSpriteName;
    //    StartCoroutine(GlobalData.GetHeadTextureByIdx(headtexture, PlayerInfo.Instance.mPlayerData.headPortrait));
    //    int sex = selectOne.mSelectTeam == ESelectType.EA ? 1 : 2;
    //    string curHeadPortrait = sex.ToString();
    //    PlayerInfo.Instance.mPlayerData.headPortrait = curHeadPortrait;


    //    WWWForm form = new WWWForm();
    //    form.AddField("appId", "HY_NJ_GD");
    //    form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
    //    form.AddField("headPortrait", curHeadPortrait);
    //    string url = GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_EditHead;
    //    StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
    //    {
    //        Debug.Log("刷新头像成功");

    //        //Update data
    //        PlayerInfo.Instance.mPlayerData.headPortrait = sex.ToString();
    //        PlayerInfo.Instance.mPlayerData.sex = sex;
    //        PlayerInfo.Instance.RefreshPlayerInfoData();

    //        UIManager.Instance.UnLoadView(mViewKeyName);


    //    }));
    //}
    #endregion
}
