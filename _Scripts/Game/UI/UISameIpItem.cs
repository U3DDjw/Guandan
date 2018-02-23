using UnityEngine;
using System.Collections;
using MsgContainer;
using UnityEngine.UI;

public class UISameIpItem : MonoBehaviour
{
    [SerializeField]
    Text nameLab;
    [SerializeField]
    Text ipLab;
    [SerializeField]
    RawImage headTex;
    [SerializeField]
    Image confirmIcon;
    [HideInInspector]
    public string ipColor;//色值 #00000
    [HideInInspector]
    public ulong mPlayerId;
    [HideInInspector]
    public bool mIsConfirm = false;
    public void SetData(ulong id, string ip, bool isConfirm, string ipcolor) //1代表领取
    {
        ipColor = ipcolor;
        mPlayerId = id;
        var playerInfo = RoomInfo.Instance.GetPlayerInfoById(id);
        if (playerInfo != null)
        {
            nameLab.text = playerInfo.name;
        }
        ipLab.text = string.Format("<color={0}>{1}</color>", ipcolor, ip.ToString());

        if (mPlayerId == PlayerInfo.Instance.mPlayerPid)
        {
            headTex.texture = DataManager.Instance.GetTextureByName(ETextureName.EHeadTex1);
        }
        else
        {
            string urlHead = RoomInfo.Instance.GetPlayerInfoById(id).head_portrait;
            StartCoroutine(GlobalData.GetHeadTextureByIdx(headTex, urlHead));

        }
        mIsConfirm = isConfirm;
        RefreshConfirm(mIsConfirm);
    }

    public void RefreshConfirm(bool isConfirm)
    {
        mIsConfirm = isConfirm;
        confirmIcon.enabled = isConfirm;
    }
}
