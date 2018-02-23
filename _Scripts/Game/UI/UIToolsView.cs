using MsgContainer;
using Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZhiWa;
using UnityEngine.UI;
using HaoyunFramework;
//     //StartCoroutine(GlobalData.GetHeadTextureByIdx(topHeadSpr,
//    roomInfo.GetPlayerInfoById((uint)dic[EPlayerPositionType.ETop].player_id).head_portrait));
public class ToolsViewContext : BaseContext
{
    public ToolsViewContext()
    {
        ViewType = UIType.PropView;
    }
}
public class UIToolsView : BasesView
{
    [SerializeField]
    Transform toolsContainer;
    [SerializeField]
    Text coinNum;
    [SerializeField]
    RawImage headTexture;
    [SerializeField]
    Text playerNameLab;
    [SerializeField]
    Text ipLab;
    [SerializeField]
    Text ipAds;//玩家的具体的地址
    [SerializeField]
    Text idLab;
    [SerializeField]
    Image sexSpr;

    [SerializeField]
    Image maskBg;
    [SerializeField]
    Image closeBtn;

    List<GameObject> btnList = new List<GameObject>();
    ulong targetId;
    //Vector3 startPos;
    //Vector3 endPos;
    void Start()
    {
        EventTriggerListener.Get(maskBg.gameObject).onClick = OnCloseClick;
        EventTriggerListener.Get(closeBtn.gameObject).onClick = OnCloseClick;
        InitProps();
        InitMoney();
    }
    void InitProps()
    {
        List<string> enumValueList = GlobalData.EnumToListInfo(typeof(MsgProps), -1);
        GameObject propPre = ResourceManager.Instance.LoadAsset<GameObject>(UIType.PropItem.Path);
        for (int i = 0; i < enumValueList.Count; i++)
        {
            if (enumValueList[i] == "58" || enumValueList[i] == "59")
            {
                continue;
            }
            GameObject propBtn = GameObject.Instantiate(propPre);
            propBtn.transform.SetParent(toolsContainer);
            propBtn.transform.localScale = Vector3.one;
            propBtn.transform.localPosition = Vector3.zero;
            propBtn.name = enumValueList[i];
            propBtn.transform.GetChild(0).GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "prop_" + enumValueList[i]);
            propBtn.GetComponentInChildren<Text>().gameObject.SetActive(PlayerInfo.Instance.mPlayerData.hideFlag != 0);
            EventTriggerListener.Get(propBtn).onClick = OnToolsClick;
            btnList.Add(propBtn);
        }
    }
    //参考：邮箱的金币增加
    //玩家金币使用后,发送到服务器后,修改名字哪里类似
    //成功扣除，播放动画
    void OnToolsClick(GameObject g)
    {
        if (btnList.Count == 0) { return; }
        RequestServer(g);
    }
    void RequestServer(GameObject g)
    {
        //int index = btnList.IndexOf(g.GetComponent<UIButton>());
        int price = int.Parse(g.name);
        MsgGlobal mGl = new MsgGlobal();
        mGl.props_info = new MsgPropsInfo();
        MsgPropsInfo msg = mGl.props_info;
        msg.action_id = PlayerInfo.Instance.mPlayerPid;
        msg.target_id = targetId;
        msg.propsId = (MsgProps)price;

        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_USE_PROPS, mGl);
        //TweenManager.Instance.SetToolsType((MsgProps)index /*GameObject.Find("UI Root").transform, startPos, endPos*/);
        Debug.Log("玩家发送使用道具的消息：" + msg.propsId);
        Debug.Log("目标ID" + msg.target_id);
        Debug.Log("发起者Id" + msg.action_id);
        OnCloseClick(g);
    }
    void OnCloseClick(GameObject g)
    {
        ContextManager.Instance.Pop(curContext);
    }
    void InitMoney()
    {
        coinNum.transform.parent.gameObject.SetActive(PlayerInfo.Instance.mPlayerData.hideFlag != 0);
        coinNum.text = PlayerInfo.Instance.mPlayerData.money.ToString();
    }
    public void SetData(MsgPlayerInfo data, ulong targetId/*, Vector3 startpos, Vector3 endpos*/)
    {
        this.targetId = targetId;
        playerNameLab.text = data.name;
        idLab.text = /*"ID: " +*/ data.player_id.ToString();

        if (data.player_id == PlayerInfo.Instance.mPlayerPid)
        {
            ipLab.text =/* "IP: " + */PlayerInfo.Instance.GetIp;
            ipAds.text = PlayerInfo.Instance.mPlayerData.ipAds;
            string sprName = PlayerInfo.Instance.GetSexIsMan ? "porfile_man" : "porfile_woman";
            sexSpr.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, sprName);
            headTexture.texture = DataManager.Instance.GetTextureByName(ETextureName.EHeadTex1);
        }
        else
        {
            ipAds.text = data.ipAds;
            ipLab.text = /*"IP: " + */data.ip;
            string sprName = data.sex == 1 ? "porfile_man" : "porfile_woman";
            sexSpr.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, sprName);
            //  headTexture.mainTexture = DataManager.Instance.GetOtherPlayerHead(data.head_portrait);
            StartCoroutine(GlobalData.GetHeadTextureByIdx(headTexture, data.head_portrait));
        }
    }
}
