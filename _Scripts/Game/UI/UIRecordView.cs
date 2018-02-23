using Haoyun.Utils;
using MsgContainer;
using System.Collections.Generic;
using UnityEngine;
using ZhiWa;
using HaoyunFramework;
using UnityEngine.UI;
using DNotificationCenterManager;
public class RecordViewContext : BaseContext
{
    public RecordViewContext()
    {
        ViewType = UIType.RecordView;
    }
}
public class UIRecordView : BasesView
{
    [SerializeField]
    Image closeBtn;
    [SerializeField]
    Image guandanBtn;
    [SerializeField]
    Image zhuandanBtn;

    [SerializeField]
    GameObject guandanContainer;
    [SerializeField]
    GameObject zhuandanContainer;

    [SerializeField]
    Transform itemContainer;
    [SerializeField]
    Image allSelectBtn;
    [SerializeField]
    Image bureaudBtn;
    [SerializeField]
    GameObject lookOtherBackBtn;
    List<GameObject> guanDanListBtn = null;   //View中的位置不能调换，调换了就得重写
    List<GameObject> zhuanDanListBtn = null;
    TGuanDanGameType curGameType = TGuanDanGameType.TGuanDanGameTypeNull;
    List<UIRecordItem> listRecordItem = new List<UIRecordItem>();
    bool Btn_AllTrue_IsClick = false;//是否全选按钮点击了
    int type = 0;
    void Start()
    {
        // EventTriggerListener.Get(maskBg.gameObject).onClick = OnCloseClick;
        EventTriggerListener.Get(closeBtn.gameObject).onClick = OnCloseClick;
        EventTriggerListener.Get(allSelectBtn.gameObject).onClick = OnSelectClick;
        EventTriggerListener.Get(bureaudBtn.gameObject).onClick = OnBureaudClick;
        EventTriggerListener.Get(guandanBtn.gameObject).onClick = OnGameTypeBtnClick;
        EventTriggerListener.Get(zhuandanBtn.gameObject).onClick = OnGameTypeBtnClick;
        EventTriggerListener.Get(lookOtherBackBtn).onClick = OnLookOtherBackBtn;
        InitButton();
        InitButtonType();//读取缓存中保存的类型
    }
    void OnLookOtherBackBtn(GameObject gm)
    {
        GameObject lookOtherView=UIManagers.Instance.GetSingleUI(UIType.RecordLookOtherBackView);
        lookOtherView.transform.SetParent(this.transform);
        lookOtherView.transform.localPosition = Vector3.zero;
        lookOtherView.transform.localScale = Vector3.one;
    }
    void InitButton()
    {
        guanDanListBtn = new List<GameObject>();
        zhuanDanListBtn = new List<GameObject>();
        int guanDanCount = guandanContainer.transform.childCount;
        for (int i = 0; i < guanDanCount; i++)
        {
            GameObject btn = guandanContainer.transform.GetChild(i).gameObject;
            EventTriggerListener.Get(btn).onClick = OnGameTypeBtnClick;
            guanDanListBtn.Add(btn);
        }
        int zhuanDanCount = zhuandanContainer.transform.childCount;
        for (int i = 0; i < zhuanDanCount; i++)
        {
            GameObject btn = zhuandanContainer.transform.GetChild(i).gameObject;
            EventTriggerListener.Get(btn).onClick = OnGameTypeBtnClick;
            zhuanDanListBtn.Add(btn);
        }
    }
    void InitButtonType()
    {
        //curGameType = (TGuanDanGameType)XPlayerPrefs.Instance.mRecordType; //玩家偏好保存
        OnGameTypeBtnClick(guandanBtn.gameObject);
        RequestRecord();
    }
    void RequestRecord()
    {
        WWWForm form = new WWWForm();
        form.AddField("appId", GlobalData.mAppId);//app的id
        form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
        form.AddField("time", TimeUtils.ConvertToTime(System.DateTime.Now).ToString()); //当前时间戳
        if (curGameType == 0)//本地是0表示掼蛋所有
        {
            form.AddField("gameType", 7);//服务器是7返回掼蛋所有的数据
            Debug.Log("GameType:" + 7);
        }
        else
        {
            form.AddField("gameType", (int)curGameType);//请求的数据的类型1-6分别对应房间的类型
            Debug.Log("GameType:" + (int)curGameType);
        }
        form.AddField("sig", "tobeadded");
        string url = GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_GetRecord;
        //返回一个迭代
        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
        {
            Debug.Log("战绩数据请求成功");
            if (www.text != null && www.text.Length > 0)
            {
                ItemContainerClear();//请求到了新的数据 将旧的数据清除掉
                List<RecordData> recordListData = JsonManager.GetRecordData(www.text);
                //修改应该根据类型添加到一个字典中，
                //根据指定的类型去调用需要的数据
                //一直打二，为一个key...
                RecordManager.Instance.UpdateRecordData(recordListData);
                InitItem(recordListData);
            }
            else
            {
                ItemContainerClear();//没有新的数据 也要将旧的数据清除掉
            }
        }));
    }
    void InitItem(List<RecordData> recordList)
    {
        GameObject recordItemPre = ResourceManager.Instance.LoadAsset<GameObject>(UIType.RecordItem.Path);

        for (int i = 0; i < recordList.Count; i++)
        {
            GameObject recordItem = GameObject.Instantiate(recordItemPre);
            recordItem.transform.SetParent(itemContainer);
            recordItem.transform.localScale = Vector3.one;
            recordItem.transform.localPosition = new Vector3(-243, -600 - i * 180, 0);
            recordItem.GetComponent<UIRecordItem>().SetData(recordList[i],gameObject);
            if ((int)curGameType == 0)
            {
                recordItem.GetComponent<UIRecordItem>().SetCheckBtnIsShow(false);
            }
            listRecordItem.Add(recordItem.GetComponent<UIRecordItem>());
        }
    }
    void OnGameTypeBtnClick(GameObject gm)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        UpdateSlectedBtnState(gm);
        RequestRecord();
    }
    #region 点击事件(备用)

    //void OnGuanDanContainerClick(GameObject g)
    //{
    //    AudioManager.Instance.PlayClickBtnAudio();
    //    UpdateSlectedBtnState(g);
    //    RequestRecord();
    //}
    //void OnGuandanBtnClick(GameObject g)
    //{
    //    AudioManager.Instance.PlayClickBtnAudio();
    //    UpdateSlectedBtnState(g);
    //    Debug.Log("CurGameType=" + curGameType.ToString());
    //}

    //void OnZhuanDanContainerClick(GameObject g)
    //{
    //    AudioManager.Instance.PlayClickBtnAudio();
    //    UpdateSlectedBtnState(g);
    //    RequestRecord();
    //}
    //void OnZhuandanBtnClick(GameObject g)
    //{
    //    AudioManager.Instance.PlayClickBtnAudio();
    //    UpdateSlectedBtnState(g);
    //    RequestRecord();
    //}
    #endregion
    void UpdateCaculateBtn(bool isOpen)
    {
        allSelectBtn.gameObject.SetActive(isOpen);
        bureaudBtn.gameObject.SetActive(isOpen);
    }
    void UpdateSlectedBtnState(GameObject g)
    {
        TGuanDanGameType lastType = curGameType;//进来之前的点击的类型
        Btn_AllTrue_IsClick = false;
        UpdateCaculateBtn(true);
        if (g == guandanBtn.gameObject)
        {
            curGameType = TGuanDanGameType.TGuanDanGameTypeNull;
            g.transform.GetChild(0).gameObject.SetActive(true);
            g.transform.GetChild(1).gameObject.SetActive(false);

            zhuandanBtn.transform.GetChild(0).gameObject.SetActive(false);
            zhuandanBtn.transform.GetChild(1).gameObject.SetActive(true);
            UpdateCaculateBtn(false);
        }
        else if (g == zhuandanBtn.gameObject)
        {
            curGameType = TGuanDanGameType.TGuanDanGameTypeZhuanDan;
            g.transform.GetChild(0).gameObject.SetActive(true);
            g.transform.GetChild(1).gameObject.SetActive(false);
            guandanBtn.transform.GetChild(0).gameObject.SetActive(false);
            guandanBtn.transform.GetChild(1).gameObject.SetActive(true);
        }
        //处理是否显示被选择
        for (int i = 0; i < guanDanListBtn.Count; i++)
        {
            guanDanListBtn[i].transform.GetChild(0).gameObject.SetActive(guanDanListBtn[i] == g);
            Text text = guanDanListBtn[i].transform.GetChild(1).GetComponent<Text>();
            Color selectedC = new Color(171 / 255.0f, 87 / 255.0f, 6 / 255.0f, 1);
            Color noSelectedC = new Color(101 / 255.0f, 87 / 255.0f, 169 / 255.0f, 1);
            text.color = guanDanListBtn[i] == g ? selectedC : noSelectedC;

            if (guanDanListBtn[i] == g)
            {
                guandanBtn.transform.GetChild(0).gameObject.SetActive(false);
                guandanBtn.transform.GetChild(1).gameObject.SetActive(true);
                zhuandanBtn.transform.GetChild(0).gameObject.SetActive(false);
                zhuandanBtn.transform.GetChild(1).gameObject.SetActive(true);
                curGameType = (TGuanDanGameType)(i + 2);//0是null,1是转蛋 ，2是掼蛋起步 一直打二
                //break;那么其他的就没办法为false了
            }
        }
        for (int i = 0; i < zhuanDanListBtn.Count; i++)
        {
            zhuanDanListBtn[i].transform.GetChild(0).gameObject.SetActive(zhuanDanListBtn[i] == g);
            Text text = zhuanDanListBtn[i].transform.GetChild(1).GetComponent<Text>();
            Color selectedC = new Color(171 / 255.0f, 87 / 255.0f, 6 / 255.0f, 1);
            Color noSelectedC = new Color(101 / 255.0f, 87 / 255.0f, 169 / 255.0f, 1);
            text.color = zhuanDanListBtn[i] == g ? selectedC : noSelectedC;

            if (zhuanDanListBtn[i] == g)
            {
                guandanBtn.transform.GetChild(0).gameObject.SetActive(false);
                guandanBtn.transform.GetChild(1).gameObject.SetActive(true);
                zhuandanBtn.transform.GetChild(0).gameObject.SetActive(false);
                zhuandanBtn.transform.GetChild(1).gameObject.SetActive(true);
                curGameType = TGuanDanGameType.TGuanDanGameTypeZhuanDan;
            }

        }
        if (lastType != curGameType)
        {
            itemContainer.transform.localPosition = Vector3.zero;
            RequestRecord();
        }
    }
    public void ItemContainerClear()
    {
        RecordManager.Instance.mCheckBoxList.Clear();
        for (int i = 0; i < itemContainer.childCount; i++)
        {
            Destroy(itemContainer.GetChild(i).gameObject);
            //listRecordItem.Remove(itemContainer.GetChild(i).GetComponent<RecordItem>());
        }
        listRecordItem.Clear();
    }

    #region 关闭 全选 统计  按钮
    void OnCloseClick(GameObject g)
    {
        XPlayerPrefs.Instance.mRecordType = (int)curGameType;
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Pop(curContext);
    }
    void OnSelectClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        Btn_AllTrue_IsClick = !Btn_AllTrue_IsClick;
        for (int i = 0; i < listRecordItem.Count; i++)
        {
            listRecordItem[i].SetCheckYes(Btn_AllTrue_IsClick);
        }
    }
    void OnBureaudClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (RecordManager.Instance.mCheckBoxList == null || RecordManager.Instance.mCheckBoxList.Count <= 0)
        {
            Debug.Log("没有可统计的数据！");
            return;
        }


        string name = PlayerInfo.Instance.mPlayerData.name;
        string id = PlayerInfo.Instance.mPlayerPid.ToString();
        string playerinfo = string.Format("{0}  ID:{1}", name, id);

        if (curGameType == TGuanDanGameType.TGuanDanGameTypeGuanDan || curGameType == TGuanDanGameType.TGuanDanGameTypeZhuanDan)
        {
            string roomInfo = "一直打二";
            int count = RecordManager.Instance.mCheckBoxList.Count;//选择的个数
            int goalNum = RecordManager.Instance.GoalNum;
            string content = string.Format("共{0}场 ", count) + "积分输赢总和:" + goalNum;
            //ContextManager.Instance.Push(new RecordBureauContext());
            RecordBureauView recordBureau = UIManagers.Instance.GetSingleUI(UIType.RecordBureauView).GetComponent<RecordBureauView>();
            recordBureau.SetData(roomInfo, playerinfo, content);
        }
        else
        {
            string roomInfo = "掼蛋";
            //content
            int count = RecordManager.Instance.mCheckBoxList.Count;//选择的个数
            int winCount = RecordManager.Instance.WinSession;
            int loseCount = count - winCount;

            string content = string.Format("共{0}场 胜:{1}场 输:{2}场", count, winCount, loseCount);
            //ContextManager.Instance.Push(new RecordBureauContext());
            RecordBureauView recordBureau = UIManagers.Instance.GetSingleUI(UIType.RecordBureauView).GetComponent<RecordBureauView>();
            recordBureau.SetData(roomInfo, playerinfo, content);
        }
    }
    #endregion

}
