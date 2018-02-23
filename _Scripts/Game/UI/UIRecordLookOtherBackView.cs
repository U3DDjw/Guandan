using System.Collections.Generic;
using UnityEngine;
using HaoyunFramework;
using UnityEngine.UI;
using System.Text;
using MsgContainer;

public class UIRecordLookOtherBackView : BasesView
{
    [SerializeField]
    GameObject closeBtn;
    [SerializeField]
    Text numInputText;
    [SerializeField]
    GameObject numContainer;
    [SerializeField]
    GameObject maskBtn;
    List<GameObject> listNumber = new List<GameObject>();
    StringBuilder shareCode = new StringBuilder();
    void Start()
    {
        InItNumber();
        EventTriggerListener.Get(maskBtn).onClick = OnCloseBtn;
        EventTriggerListener.Get(closeBtn).onClick = OnCloseBtn;
        OnNumberClick(null);
    }
    void InItNumber()
    {
        GameObject gm = ResourceManager.Instance.LoadAsset<GameObject>(RoomNumberItem.Path);
        for (int i = 1; i <= 12; i++)
        {
            GameObject obj = GameObject.Instantiate(gm);
            obj.transform.SetParent(numContainer.transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            SetBtnSprName(i, obj);
            Component.Destroy(obj.transform.GetComponent<RoomNumberItem>());
            EventTriggerListener.Get(obj).onClick = OnNumberClick;
            listNumber.Add(obj);
        }
    }
    void OnNumberClick(GameObject number)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        int index = listNumber.IndexOf(number) + 1;
        if (index == 10)
        {
            shareCode.Remove(0, shareCode.Length);
        }
        else if (index == 12)
        {
            if (shareCode.Length > 0)
                shareCode.Remove(shareCode.Length - 1, 1);
        }
        else if (index == 11)
        {
            shareCode.Append(0);
        }
        else if (number != null)
        {
            shareCode.Append(index);
        }
        numInputText.text = shareCode.ToString();
        CheckInputIsSuc(shareCode.ToString());
    }
    void CheckInputIsSuc(string roomId)
    {
        if (roomId.Length >= 10)
        {
            UIManagers.Instance.EnqueueTip("获取他人回放信息数据中");
            WWWForm info = new WWWForm();
            info.AddField("roomId", roomId);
            string url = GlobalData.mConstBaseServerUrl + ServerUrlTitle.Url_GetOtherRecordDetail;
            StartCoroutine(MsgContainer.GlobalData.SendPost(url, info, delegate (WWW www)
             {
                 if (www.text != null)
                 {
                     string text = www.text;
                     RecordDetailData detailData = JsonManager.GetRecordDetailData(text);
                     DetailDataToRecordData(detailData);
                 }
                 else
                 {
                     UIManagers.Instance.EnqueueTip("获取他人回放信息错误");
                 }
             }));
            if (roomId.Length > 10)
            {
                shareCode.Remove(shareCode.Length - 2, 2);
                numInputText.text = shareCode.ToString();
            }
        }
    }
    public void DetailDataToRecordData(RecordDetailData detailData)
    {
        if (detailData == null) return;
        RecordData recordData = new RecordData();
        recordData.endTime = detailData.endTime;
        recordData.gameType = detailData.gameType;
        recordData.num = detailData.gameNum;
        recordData.roomCode = detailData.roomCode;
        recordData.roomid = detailData.roomId;
        if (detailData.gameType > 2)
        {
            if (detailData.gameType != 6)
                recordData.limitCard = detailData.gameType * 2;//3,4,5 >>6,8,10
            else
                recordData.limitCard = 14;//A
        }
        List<Dictionary<string, string>> dicInfo = new List<Dictionary<string, string>>();

        if (detailData.players != null)//转蛋 其他玩家信息
        {
            for (int i = 0; i < detailData.players.Count; i++)
            {
                PlayBackFightData fightData = detailData.players[i];
                Dictionary<string, string> dicStr = new Dictionary<string, string>();
                dicStr.Add("pid", fightData.pid.ToString());
                dicStr.Add("allGoal", fightData.allGoal.ToString());
                for (int j = 0; j < detailData.player.Count; j++)
                {
                    if (detailData.player[j].pid == detailData.players[i].pid)
                    {
                        dicStr.Add("name", detailData.player[j].name);
                        break;
                    }
                }
                dicInfo.Add(dicStr);
            }
        }
        else if (detailData.endings != null) //掼蛋 其他玩家信息
        {
            List<PlayBackFightData> player = detailData.player;
            List<PlayBackFightData> endings = detailData.endings;
            for (int i = 0; i < player.Count; i++)
            {
                PlayBackFightData fightData = player[i];
                Dictionary<string, string> dicStr = new Dictionary<string, string>();
                dicStr.Add("pid", fightData.pid.ToString());
                dicStr.Add("name", fightData.name);
                for (int j = 0; j < endings.Count; j++)
                {
                    bool isBreak = false;
                    List<ulong> member = endings[j].member;
                    for (int m = 0; m < member.Count; m++)
                    {
                        if (member[m] == player[i].pid)
                        {
                            dicStr.Add("level", endings[j].level.ToString());
                            isBreak = true;
                            break;
                        }
                    }
                    if (isBreak)
                    {
                        break;
                    }
                }
                dicInfo.Add(dicStr);
            }
        }
        recordData.info = dicInfo;
        RoomId_RoomInfo(recordData);
    }
    public void RoomId_RoomInfo(RecordData recordData)
    {
        GameObject recordItemDetail = UIManagers.Instance.GetSingleUI(UIType.RecordItemDetail);
        recordItemDetail.transform.SetParent(this.transform.parent);
        recordItemDetail.transform.localPosition = Vector3.zero;
        recordItemDetail.transform.localScale = Vector3.one;
        recordItemDetail.GetComponent<UIRecordItemDetail>().SetData(recordData);
    }
    void SetBtnSprName(int id, GameObject numBtn)
    {
        string pathSpr = "";
        Image numBtnImage = numBtn.GetComponent<Image>();
        Image num = numBtn.transform.GetChild(0).GetComponent<Image>();
        if (id == 10) //删除
        {
            num.enabled = false;
            pathSpr = "room_btn" + 3;
            numBtnImage.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, pathSpr);
            numBtnImage.GetComponent<Button>().spriteState = UIManagers.Instance.SetButtonStateSpr(EAtlasType.EMain, pathSpr + "_click");
        }
        else if (id == 12) //清空
        {
            num.enabled = false;
            pathSpr = "room_btn" + 2;
            numBtnImage.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, pathSpr);
            numBtnImage.GetComponent<Button>().spriteState = UIManagers.Instance.SetButtonStateSpr(EAtlasType.EMain, pathSpr + "_click");
        }
        else if (id == 11)
        {
            num.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, "ts0");
        }
        else
        {
            num.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, "ts" + id.ToString());
        }

    }
    void OnCloseBtn(GameObject close)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        UIManagers.Instance.DestroySingleUI(UIType.RecordLookOtherBackView);
    }
    #region 热更
    public Dictionary<string, string> InitDic()
    {
        return new Dictionary<string, string>();
    }
    public string GetDic(Dictionary<string, string> dic, string s)
    {
        return dic[s];
    }
    #endregion
}
