using UnityEngine;
using System.Collections;
using DNotificationCenterManager;
using MsgContainer;
using UnityEngine.UI;
using HaoyunFramework;
public enum EDissmissRoomResult
{
    ENull = 0,
    EUnSelect = 1,//未选择
    EDisagree = 2,//拒绝
    EAgree = 3,//同意
}
public class VoteDismissItemContext : BaseContext
{
    public VoteDismissItemContext()
    {
        ViewType = UIType.VoteDismissItem;
    }
}
public class UIVoteDismissItem : BasesView
{
    [SerializeField]
    Text nameLab;
    [SerializeField]
    Image resultImg;
    [SerializeField]
    RawImage headTexture;
    //[SerializeField]
    //RawImage headTexture;
    //public ulong mPlayerId;
    public void InitData(ulong playerId, string name, EDissmissRoomResult result)
    {
        //mPlayerId = playerId;
        nameLab.text = name;
        // StartCoroutine(MsgContainer.GlobalData.GetHeadTextureByIdx(headTexture, RoomInfo.Instance.GetPlayerInfoById(playerId).head_portrait));
        if (playerId != PlayerInfo.Instance.mPlayerPid)
        {
            StartCoroutine(GlobalData.GetHeadTextureByIdx(headTexture, RoomInfo.Instance.GetPlayerInfoById(playerId).head_portrait));
        }
        else
            headTexture.texture = DataManager.Instance.GetTextureByName(ETextureName.EHeadTex1);
        UpdateData(result);
    }

    public void UpdateData(EDissmissRoomResult result)
    {
        string resultName = "";
        switch (result)
        {
            case EDissmissRoomResult.EAgree:
                resultName = "disbanded_agree";
                break;
            case EDissmissRoomResult.EDisagree:
                resultName = "disbanded_refuse";
                break;
            case EDissmissRoomResult.EUnSelect:
                resultName = "disbanded_unselected";
                break;
        }
        resultImg.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, resultName);
        resultImg.SetNativeSize();
    }
}
