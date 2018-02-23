using UnityEngine;
using System.Collections;
using ZhiWa;
using MsgContainer;
using HaoyunFramework;
using UnityEngine.UI;

public class BureauOverItemContext : BaseContext
{
    public BureauOverItemContext()
    {
        ViewType = UIType.BureauOverItem;
    }
}
public class UIBureauOverItem : MonoBehaviour
{

    [SerializeField]
    Image chaperSpr;
    [SerializeField]
    RawImage headTexture;
    [SerializeField]
    Image foreBg;
    //[SerializeField]
    //UISprite head;
    [SerializeField]
    Text nameAndId;
    [SerializeField]
    Text scorelab;
    [SerializeField]
    Image awardNumSpr;
    [SerializeField]
    Text jiFengOrDadao;

    [SerializeField]
    Text achieveGoldNum;
    public void SetData(MsgGameOverInfo msg, EPlayersTeam winTeam)
    {
        this.awardNumSpr.sprite = GlobalData.GetGameCleanCpr((int)msg.rank);
        string forbgName = msg.player_id == PlayerInfo.Instance.mPlayerPid ? "record_list_self" : "record_list";
        foreBg.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, forbgName);

        awardNumSpr.sprite = GlobalData.GetGameCleanCpr((int)msg.rank);
        awardNumSpr.gameObject.SetActive(awardNumSpr.sprite != null);

        var data = RoomInfo.Instance.GetPlayerInfoById(msg.player_id);
        chaperSpr.gameObject.SetActive(msg.player_id == RoomInfo.Instance.mRoom.creater_id);
        if (msg.player_id != PlayerInfo.Instance.mPlayerPid)
        {
            StartCoroutine(GlobalData.GetHeadTextureByIdx(headTexture, data.head_portrait));
        }
        else
            headTexture.texture = DataManager.Instance.GetTextureByName(ETextureName.EHeadTex1);

        awardNumSpr.SetNativeSize();
        var player = RoomInfo.Instance.GetPlayerInfoById(msg.player_id);

        nameAndId.text = player.name + "\n" + "ID:" + msg.player_id;


        if (RoomInfo.Instance.IsZhuandanGameType || RoomInfo.Instance.IsGuandan2GameType)
        {
            jiFengOrDadao.text = "积分：";
            //scorelab.color = (int)msg.goal >= 0 ? Color.yellow : Color.gray;
            int goal = (int)msg.goal;
            this.scorelab.text = goal > 0 ? "+" + goal : goal.ToString();
        }
        else //含金币场结算
        {
            jiFengOrDadao.text = "打到:";
            this.scorelab.text = GlobalData.StringUpgradeIndex(msg.upgrade_card.ToString());
            bool isGold = GoldFiledManager.Instance.mIsGoldFiled;
            UpdateGoldModeData((int)msg.goal,PlayerInfo.Instance.mPlayerPid == msg.player_id);
        }
    }


    void UpdateGoldModeData(int goal,bool isSelf)
    {
        int totalData = goal * (int)GoldFiledManager.Instance.GetBaseCost;
        string strGetInfo = string.Format("金币 {0}{1}",goal>0?"+":"" ,totalData);
        achieveGoldNum.text = strGetInfo;

        if (isSelf)
        {
            PlayerInfo.Instance.mPlayerData.money += totalData; //结算该玩家数据
        }
    }




}


