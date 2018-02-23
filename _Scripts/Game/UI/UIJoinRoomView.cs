using UnityEngine;
using DNotificationCenterManager;
using MsgContainer;
using System.Text;
using HaoyunFramework;
using UnityEngine.UI;
public class JoinRoomContext : BaseContext
{
    public JoinRoomContext()
    {
        ViewType = UIType.JoinRoomView;
    }
}
public class UIJoinRoomView : BasesView
{

    //id 为10 和11 分别为 取消和删除
    StringBuilder curNumbers = new StringBuilder(6);

    [SerializeField]
    GameObject numberContainer;
    [SerializeField]
    Transform numberParent;
    [SerializeField]
    GameObject maskBg;
    [SerializeField]
    GameObject closeBtn;
    void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EInputNumber, UpdateNumber);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EJoinRoomFail, UpdateJoinRoomFail);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ERoomJoin_Suc, UpdateJoinRoomSuc);
    }
    void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EInputNumber, UpdateNumber);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EJoinRoomFail, UpdateJoinRoomFail);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ERoomJoin_Suc, UpdateJoinRoomSuc);
    }



    private void Start()
    {
        InitNumber();
        for (int i = 0; i < 6; i++)
        {
            numberContainer.transform.GetChild(i).GetChild(0).GetComponent<Image>().enabled = false;
        }
        EventTriggerListener.Get(closeBtn).onClick = OnCloseViewClick;
        EventTriggerListener.Get(maskBg).onClick = OnCloseViewClick;
    }

    void OnCloseViewClick(GameObject gm)
    {
        ContextManager.Instance.Pop(curContext);
    }


    int rateX = 200;
    int rateY = -100;
    /// <summary>
    /// 初始化数字
    /// </summary>
    void InitNumber()
    {
        //GameObject gm= Resources.Load<GameObject>("RoomNumberItem"); 
        GameObject gm = ResourceManager.Instance.LoadAsset<GameObject>(RoomNumberItem.Path);
        for (int i = 0; i < 12; i++)
        {
            GameObject obj = GameObject.Instantiate(gm);
            obj.transform.SetParent(numberParent);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = GetPos(i);
            obj.transform.GetComponent<RoomNumberItem>().SetRoomNumberData(i);
            if (i == 11 || i == 10)
            {
                SetSprName(i, obj.transform.GetComponent<Image>());
            }
        }

    }

    void SetSprName(int id, Image m)
    {
        string pathSpr = "";
        if (id == 10) //删除
        {
            pathSpr = "room_btn" + 3;
        }
        else if (id == 11) //清空
        {
            pathSpr = "room_btn" + 2;
        }
        m.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, pathSpr);

        m.GetComponent<Button>().spriteState = UIManagers.Instance.SetButtonStateSpr(EAtlasType.EMain, pathSpr + "_click");
    }

    void UpdateJoinRoomSuc(LocalNotification e)
    {
        Debug.Log("房间加入成功");
        ContextManager.Instance.Pop(curContext);
        ContextManager.Instance.Push(new WaitGameContext());
    }

    Vector3 GetPos(int id)
    {

        //  以最左上角点为参照点
        Vector3 pos = new Vector3(-195, 100, 0); //自己比对出来的初始位置
        if (id == 0)
        {
            pos = new Vector3(5, -195, 0);
        }
        else if (id == 10)
        {
            pos = new Vector3(-195, -195, 0);
        }
        else if (id == 11)
        {
            pos = new Vector3(205, -195, 0);
        }
        else
        {
            pos.x += ((id - 1) % 3) * rateX;
            pos.y += ((id - 1) / 3) * rateY;
        }
        return pos;
    }

    void UpdateJoinRoomFail(LocalNotification e)
    {
        ArgsJoinRoomReason msg = e.param as ArgsJoinRoomReason;
        if (msg != null)
        {
            switch (msg.reason)
            {
                case EJoinRoomFailReason.EFKBZ:
                    UIManagers.Instance.EnqueueTip("房卡不足");
                    break;
                case EJoinRoomFailReason.EROOM_NULL:
                    UIManagers.Instance.EnqueueTip("房间号不存在");
                    break;
                case EJoinRoomFailReason.ERSYM:
                    UIManagers.Instance.EnqueueTip("人数已满");
                    break;
            }

        }
        curNumbers.Remove(curNumbers.Length - 1, 1);
        isNewLoad = false;
        numberContainer.transform.GetChild(curNumbers.Length).GetChild(0).GetComponent<Image>().enabled = false;
        Debug.Log(RoomInfo.Instance.mRoomCode);
    }


    void UpdateNumber(LocalNotification e)
    {
        ArgsChangeRoomNumber args = e.param as ArgsChangeRoomNumber;
        int num = args.Id;
        switch (num)
        {
            case 11: //减少一个
                if (curNumbers.Length > 0)
                {
                    curNumbers.Remove(curNumbers.Length - 1, 1);
                    numberContainer.transform.GetChild(curNumbers.Length).GetChild(0).GetComponent<Image>().enabled = false;
                }
                break;
            case 10: //清空
                curNumbers.Remove(0, curNumbers.Length);
                for (int i = 0; i < 6; i++)
                {
                    numberContainer.transform.GetChild(i).GetChild(0).GetComponent<Image>().enabled = false;
                }
                break;
            default:
                if (curNumbers.Length < 6)
                {
                    curNumbers.Append(num.ToString());
                }
                break;
        }

        UpdateNumberLab();
        if (curNumbers.Length == 6)
        {
            SendServerArgs();
            isNewLoad = true;
        }
        else
        {
            isNewLoad = false;
        }
    }

    void UpdateNumberLab()
    {
        for (int i = 0; i < curNumbers.Length; i++)
        {
            numberContainer.transform.GetChild(i).GetChild(0).GetComponent<Image>().enabled = true;
            Sprite inputNUm = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, "ts" + curNumbers[i]);
            numberContainer.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = inputNUm;
        }
    }

    bool isNewLoad = false;
    void SendServerArgs()
    {
        //这边需要加一个，满数字还点，和第一次为6的判断、
        if (!isNewLoad)
        {
            Debug.Log("JoinRoom---SendServer");
            Debug.Log(RoomInfo.Instance.mRoomCode);
            RoomInfo.Instance.SendJoinRoomServer(int.Parse(curNumbers.ToString()));
        }
    }
}
