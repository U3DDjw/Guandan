using DNotificationCenterManager;
using HaoyunFramework;
using MsgContainer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UIRecordItem : MonoBehaviour
{

    [SerializeField]
    Text roomInfoLab;
    [SerializeField]
    Text endTimeLab;

    //四个玩家的信息的展示
    [SerializeField]
    //list[0] 第一个子物体显示id，第二个子物体显示积分
    List<Image> labelList = new List<Image>();
    [SerializeField]
    Image checkBtn;//child[0]的显示控制表示选择与否
    [SerializeField]
    GameObject detailBtn;//详情按钮

    RecordData curRecordData = null;

    GameObject recordView = null;
    private void Start()
    {
        EventTriggerListener.Get(checkBtn.gameObject).onClick = OnCheckClick;
        EventTriggerListener.Get(detailBtn.gameObject).onClick = OnDetailClick;
    }
    public void OnDetailClick(GameObject gm)
    {
        GameObject recordItemDetail = UIManagers.Instance.GetSingleUI(UIType.RecordItemDetail);
        recordItemDetail.transform.SetParent(recordView.transform);
        recordItemDetail.transform.localPosition = Vector3.zero;
        recordItemDetail.transform.localScale = Vector3.one;
        recordItemDetail.GetComponent<UIRecordItemDetail>().SetData(curRecordData);
    }
    public void SetCheckBtnIsShow(bool isShow)
    {
        checkBtn.gameObject.SetActive(isShow);
    }
    public void SetData(RecordData data, GameObject recordView)
    {
        this.recordView = recordView;
        //掼蛋 房间ID 把数:4 把    时间
        //DateTime.Now.ToString() + DateTime.Now.ToString("HH:mm")
        curRecordData = data;
        endTimeLab.text = GlobalData.FormatServerTime(data.endTime);
        SetdataOfGametype(data);
    }
    string GetGametype(int type)
    {
        if (type == 0) { return null; }
        else if (type == 1)
        {
            return "转蛋";
        }
        else
        {
            return "掼蛋";
        }

    }
    //获得游戏的局数 
    //或者打到几
    void SetdataOfGametype(RecordData data)
    {
        List<Dictionary<string, string>> listInfo = data.info;

        int type = data.gameType;
        //一直打二
        if (type == 1 || type == 2)
        {
            string roomId = "<color=#18285b><size=28>房间号:{0} </size></color>";
            string gameNum = "<color=#d74e2f><size=30>\t局数:{1}</size></color>";
            roomInfoLab.text = string.Format(roomId + gameNum, data.roomCode, data.num);

            for (int i = 0; i < listInfo.Count; i++)
            {
                Dictionary<string, string> dic = listInfo[i];

                if (labelList.Count != 0)
                {
                    //显示名称
                    labelList[i].transform.GetChild(0).GetComponent<Text>().text = dic["name"];
                    //显示结果
                    string idAndEndIndex = " <size= 24 >（Id: {0}）</size>\n<size=28> {1}积分 </size>";
                    string result = string.Format(idAndEndIndex, dic["pid"], dic["allGoal"]);
                    labelList[i].transform.GetChild(1).GetComponent<Text>().text = result;
                    if (dic["pid"] == PlayerInfo.Instance.mPlayerPid.ToString())
                    {
                        labelList[i].transform.GetChild(0).GetComponent<Text>().color = new Color(55 / 255.0f, 83 / 255.0f, 41 / 255.0f, 1);
                        labelList[i].transform.GetChild(1).GetComponent<Text>().color = new Color(55 / 255.0f, 83 / 255.0f, 41 / 255.0f, 1);
                    }
                }
            }
        }
        //掼蛋非打二
        else
        {
            string roomId = "<color=#18285b><size=28>房间号:{0} </size></color>";
            string gameNum = "<color=#d74e2f><size=30>\t打到:{1}</size></color>";
            roomInfoLab.text = string.Format(roomId + '\t' + gameNum, data.roomCode, GlobalData.StringUpgradeIndex(data.limitCard.ToString()));
            for (int i = 0; i < listInfo.Count; i++)
            {
                Dictionary<string, string> dic = listInfo[i];

                if (labelList.Count != 0)
                {
                    //显示名称
                    labelList[i].transform.GetChild(0).GetComponent<Text>().text = dic["name"];
                    //显示结果
                    string idAndEndIndex = "<size=24>（Id: {0}）</size>\n<size= 28> 打到{1} </size>";
                    string result = string.Format(idAndEndIndex, dic["pid"], GlobalData.StringUpgradeIndex(dic["level"]));
                    labelList[i].transform.GetChild(1).GetComponent<Text>().text = result;
                    if (dic["pid"] == PlayerInfo.Instance.mPlayerPid.ToString())
                    {
                        labelList[i].transform.GetChild(0).GetComponent<Text>().color = new Color(55 / 255.0f, 83 / 255.0f, 41 / 255.0f, 1);
                        labelList[i].transform.GetChild(1).GetComponent<Text>().color = new Color(55 / 255.0f, 83 / 255.0f, 41 / 255.0f, 1);
                    }
                }
            }
        }
    }
    void OnCheckClick(GameObject g)
    {
        GameObject yes = checkBtn.transform.GetChild(0).gameObject;
        yes.SetActive(!yes.activeSelf);
        if (yes.activeSelf)
        {
            if (!RecordManager.Instance.mCheckBoxList.Contains(curRecordData))
                RecordManager.Instance.mCheckBoxList.Add(curRecordData);
        }
        else
        {
            if (RecordManager.Instance.mCheckBoxList.Contains(curRecordData))
                RecordManager.Instance.mCheckBoxList.Remove(curRecordData);
        }
    }
    /// <summary>
    /// 改变选择的状态，共全选使用
    /// </summary>
    /// <param name="allBool">全选之后的状态</param>
    public void SetCheckYes(bool allBool)
    {
        GameObject yes = checkBtn.transform.GetChild(0).gameObject;
        if (yes.activeSelf != allBool)
            yes.SetActive(allBool);
        if (yes.activeSelf)
        {
            if (!RecordManager.Instance.mCheckBoxList.Contains(curRecordData))
                RecordManager.Instance.mCheckBoxList.Add(curRecordData);
        }
        else
        {
            if (RecordManager.Instance.mCheckBoxList.Contains(curRecordData))
                RecordManager.Instance.mCheckBoxList.Remove(curRecordData);
        }
    }
    public bool GetCheckBtnIsYes()
    {
        return checkBtn.transform.GetChild(0).gameObject.activeSelf;
    }
    private void OnDestroy()
    {
        RecordManager.Instance.mCheckBoxList.Remove(curRecordData);
    }
}
