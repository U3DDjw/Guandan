using UnityEngine;
using System.Collections;
using MsgContainer;
using System;
using ZhiWa;
using UnityEngine.UI;
using HaoyunFramework;
public class TotalBureauOverItemContext : BaseContext
{
    public TotalBureauOverItemContext()
    {
        ViewType = UIType.TotalBureauOverItem;
    }
}
public class UITotalBureauOverItem : BasesView
{

    [SerializeField]
    Image creater;
    [SerializeField]
    Image foreBg;

    [SerializeField]
    Text nameLab;
    [SerializeField]
    Text jfOrDadao;
    [SerializeField]
    RawImage headTex;
    [SerializeField]
    Text scoreLab;

    public void SetData(ulong playerId, int score)
    {
        //获取名字
        var data = RoomInfo.Instance.GetPlayerInfoById(playerId);
        string forBgName = playerId == PlayerInfo.Instance.mPlayerPid ? "record_list_self" : "record_list";
        foreBg.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, forBgName);
        //传过来的ID是否和创建房间的Id一样
        creater.enabled = RoomInfo.Instance.mRoom.creater_id == playerId ? true : false;

        nameLab.text = data.name + string.Format("\nID:{0}", playerId);//玩家的名字
        jfOrDadao.text = "总积分:";
        //scoreLab.color = score >= 0 ? Color.yellow : Color.gray;
        scoreLab.text = score > 0 ? "+" + score : score.ToString();
        if (playerId != PlayerInfo.Instance.mPlayerPid)
        {
            string url = RoomInfo.Instance.GetPlayerInfoById(playerId).head_portrait;
            StartCoroutine(GlobalData.GetHeadTextureByIdx(headTex, url));
        }
        else
        {
            headTex.texture = DataManager.Instance.GetTextureByName(ETextureName.EHeadTex1);
        }
    }
    //掼蛋
    public void SetData(MsgGameOverInfo msgGameOverInfo, EPlayersTeam winTeam)
    {

        ulong playerId = msgGameOverInfo.player_id;
        var data = RoomInfo.Instance.GetPlayerInfoById(playerId);
        string forBgName = playerId == PlayerInfo.Instance.mPlayerPid ? "record_list_self" : "record_list";
        foreBg.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, forBgName);
        creater.enabled = RoomInfo.Instance.mRoom.creater_id == playerId ? true : false;

        nameLab.text = data.name + string.Format("\nID:{0}", playerId);//玩家的名字
        jfOrDadao.text = data.teamType == (int)winTeam ? "打过:" : "打到:";
        bool isEnd = RoomInfo.Instance.mNextIndex != RoomInfo.Instance.GetEndgradOfTGuanDanGameType();
        if (PlayerInfo.Instance.mTeamEnemyIndex == PlayerInfo.Instance.mTeamSelfIndex || isEnd)
        {
            jfOrDadao.text = "打到:";
        }
        if (playerId != PlayerInfo.Instance.mPlayerPid)
        {
            StartCoroutine(GlobalData.GetHeadTextureByIdx(headTex, data.head_portrait));
        }
        else
            headTex.texture = DataManager.Instance.GetTextureByName(ETextureName.EHeadTex1);
        scoreLab.text = GlobalData.StringUpgradeIndex(msgGameOverInfo.upgrade_card.ToString());

    }
}
