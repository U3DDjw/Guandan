using UnityEngine;
using System.Collections;
using MsgContainer;
using DNotificationCenterManager;
using UnityEngine.UI;
public class RoomNumberItem : MonoBehaviour
{

    public static readonly string Path = GlobalData.mLoadItemTitlePath + "RoomNumberItem";
    int Id;

    [SerializeField]
    GameObject btn;

    [SerializeField]
    Image labSpr;
    private void Start()
    {
        EventTriggerListener.Get(btn).onClick = ButtonClick;
    }

    void ButtonClick(GameObject gm)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ArgsChangeRoomNumber msg = new ArgsChangeRoomNumber();
        msg.Id = this.Id;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EInputNumber, msg);
    }

    public void SetRoomNumberData(int id)
    {
        this.Id = id;

        ShowLab();

    }

    void ShowLab()
    {
        string labName = "";
        if (this.Id < 10)
        {
            labName = "ts" + Id.ToString();
        }
        else if (this.Id == 10|| this.Id == 11)
        {
            labSpr.enabled = false;
        }
        labSpr.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, labName);
    }
}
