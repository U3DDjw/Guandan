using System.Collections.Generic;
using UnityEngine;
using HaoyunFramework;
using UnityEngine.UI;
using MsgContainer;
using DG.Tweening;
using ZhiWa;

public class UIRecordItemDetail : BasesView
{
    [SerializeField]
    GameObject closeBtn;
    [SerializeField]
    GameObject playerContainer;
    [SerializeField]
    GameObject playbackBtn;
    [SerializeField]
    GameObject playbackShareBtn;
    [SerializeField]
    Text roomInfoText;
    [SerializeField]
    Text endtimeText;
    [SerializeField]
    GameObject maskBg;
    [SerializeField]
    GameObject videoBtnContainer;
    //动画相关
    [SerializeField]
    GameObject tpBg;//底层透明背景
    [SerializeField]
    GameObject detailBg;//内容背景
    [SerializeField]
    GameObject videoViewBg;//视频按钮背景
    bool isHasClickBack = false;
    Sequence detailTween = null;
    List<RecordVideoData> listVideoData = new List<RecordVideoData>();
    List<GameObject> playerInfoList = new List<GameObject>();
    List<GameObject> videoBtnList = new List<GameObject>();
    RecordData curData;
    void Start()
    {
        EventTriggerListener.Get(closeBtn).onClick = OnCloseBtn;
        EventTriggerListener.Get(maskBg).onClick = OnCloseBtn;
        EventTriggerListener.Get(playbackBtn).onClick = OnPlayBackClick;
        EventTriggerListener.Get(playbackShareBtn).onClick = OnPlayBackShareClick;
        videoViewBg.SetActive(false);
    }
    void InitLabelList()
    {
        for (int i = 0; i < playerContainer.transform.childCount; i++)
        {
            playerInfoList.Add(playerContainer.transform.GetChild(i).gameObject);
        }
    }
    private void OnPlayBackShareClick(GameObject go)
    {
        Debug.Log("点击了回放分享按钮");
    }

    private void OnPlayBackClick(GameObject go)
    {
        Debug.Log("点击了回放按钮");
        if (isHasClickBack) return;
        isHasClickBack = true;
        WWWForm form = new WWWForm();
        form.AddField("roomId", curData.roomid);
        string url = GlobalData.mConstBaseServerUrl + ServerUrlTitle.Url_GetRecordAllVideoInfo;
        StartCoroutine(MsgContainer.GlobalData.SendPost(url, form, delegate (WWW www)
        {
            if (www.text != null)
            {
                string text = www.text;
                List<RecordVideoData> videosData = JsonManager.GetRecordVideoData(text);
                RequesSuc(videosData);
            }
        }));

    }
    void RequesSuc(List<RecordVideoData> videosData)
    {
        InitVideoBtn(videosData.Count);
        ShowTween();
    }
    void ShowVideosData(List<RecordVideoData> videosData)
    {

    }
    void ShowTween()
    {
        detailTween = DOTween.Sequence();
        //透明背景
        RectTransform rect = tpBg.transform.GetComponent<RectTransform>();
        Vector2 endValue = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y * 2 - 40);
        detailTween.Append(rect.DOSizeDelta(endValue, 0.25f)).SetEase(Ease.OutBounce);
        //内容背景
        RectTransform detailBgRect = detailBg.transform.GetComponent<RectTransform>();
        float dis = endValue.y - (rect.sizeDelta.y - detailBgRect.sizeDelta.y);//边距
        Vector2 videoEndValue = new Vector2(detailBgRect.sizeDelta.x, dis);
        detailTween.Join(detailBgRect.DOSizeDelta(videoEndValue, 0.3f)).SetEase(Ease.OutBounce);

        //video背景板出现
        videoViewBg.transform.localScale = Vector3.zero;
        detailTween.AppendCallback(
            delegate
            {
                videoViewBg.SetActive(true);
            });

        detailTween.Append(videoViewBg.transform.DOScale(Vector3.one, 0.3f)).SetEase(Ease.Linear);
        for (int i = 0; i < videoBtnList.Count; i++)
        {
            detailTween.Append(videoBtnList[i].transform.DOScale(Vector3.one, 0.1f));
            Image videoImage = videoBtnList[i].GetComponent<Image>();
            detailTween.Join(DOTween.ToAlpha(() => videoImage.color, x => videoImage.color = x, 1, 0.1f));
        }
    }
    void InitVideoBtn(int realNum)//实际局数
    {
        int videoListLength = realNum;//测试数据
        GameObject videoBtn = ResourceManager.Instance.LoadAsset<GameObject>("View/RecordVideoBtn");
        for (int i = 0; i < videoListLength; i++)
        {
            GameObject gm = GameObject.Instantiate(videoBtn);
            gm.transform.SetParent(videoBtnContainer.transform);
            gm.GetComponentInChildren<Text>().text = "第" + (i + 1) + "局";
            gm.transform.localPosition = Vector3.zero;
            gm.transform.localScale = Vector3.zero;
            gm.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            videoBtnList.Add(gm);
            EventTriggerListener.Get(gm).onClick = OnVideoBtnClick;
        }
    }

    void OnVideoBtnClick(GameObject gm)
    {
        int index = videoBtnList.IndexOf(gm) + 1;
        GameObject playBackView = UIManagers.Instance.GetSingleUI(UIType.PlayBackView);
        UIPlayBackView playBack = playBackView.GetComponent<UIPlayBackView>();
        ArgsPlayBackRoomInfo argsRoomInfo = new ArgsPlayBackRoomInfo();
        if ((int)argsRoomInfo.gameType == 1 || (int)argsRoomInfo.gameType == 2)
            argsRoomInfo.gameType = (TGuanDanGameType)curData.gameType;
        else
            argsRoomInfo.gameType = GetGameType(curData.limitCard);
        argsRoomInfo.roomCodeMd5 = curData.roomid;
        argsRoomInfo.roomCode = curData.roomCode;
        argsRoomInfo.gameIndex = index;
        argsRoomInfo.gameTotalNum = curData.num;
        playBack.SetRoomInfo(argsRoomInfo);
    }
    public TGuanDanGameType GetGameType(int limitCard)
    {
        TGuanDanGameType curGameType;
        switch (limitCard)
        {
            case 6:
                curGameType = TGuanDanGameType.TGuanDanGameTypeGuanDan6;
                break;
            case 8:
                curGameType = TGuanDanGameType.TGuanDanGameTypeGuanDan8;
                break;
            case 10:
                curGameType = TGuanDanGameType.TGuanDanGameTypeGuanDan10;
                break;
            case 14:
                curGameType = TGuanDanGameType.TGuanDanGameTypeGuanDanA;
                break;
            default:
                curGameType = TGuanDanGameType.TGuanDanGameTypeGuanDan;
                break;
        }
        return curGameType;
    }
    public void SetData(RecordData data)
    {
        this.curData = data;
        InitLabelList();
        List<Dictionary<string, string>> listInfo = data.info;

        int type = data.gameType;
        endtimeText.text = GlobalData.FormatServerTime(data.endTime);
        //一直打二
        if (type == 1 || type == 2)
        {
            string roomCode = "<color=#18285b><size=28>房间号:{0}  </size></color>";
            string gameNum = "<color=#d74e2f><size=30>局数:{1}   </size></color>";
            roomInfoText.text = string.Format(roomCode + gameNum, data.roomCode, data.num);

            for (int i = 0; i < listInfo.Count; i++)
            {
                Dictionary<string, string> dic = listInfo[i];

                if (playerInfoList.Count != 0)
                {
                    //显示名称
                    playerInfoList[i].transform.GetChild(0).GetComponent<Text>().text = dic["name"];
                    //显示结果
                    string idAndEndIndex = " <size= 24 >（Id: {0}）</size>\n<size=28> {1}积分 </size>";
                    string result = string.Format(idAndEndIndex, dic["pid"], dic["allGoal"]);
                    playerInfoList[i].transform.GetChild(1).GetComponent<Text>().text = result;
                    if (dic["pid"] == PlayerInfo.Instance.mPlayerPid.ToString())
                    {
                        playerInfoList[i].transform.GetChild(0).GetComponent<Text>().color = new Color(55 / 255.0f, 83 / 255.0f, 41 / 255.0f, 1);
                        playerInfoList[i].transform.GetChild(1).GetComponent<Text>().color = new Color(55 / 255.0f, 83 / 255.0f, 41 / 255.0f, 1);
                    }
                }
            }
        }
        //掼蛋非打二
        else
        {
            string roomId = "<color=#18285b><size=28>房间号:{0}  </size></color>";
            string gameNum = "<color=#d74e2f><size=30>打到:{1}  </size></color>";
            roomInfoText.text = string.Format(roomId + '\t' + gameNum, data.roomCode, GlobalData.StringUpgradeIndex(data.limitCard.ToString()));
            for (int i = 0; i < listInfo.Count; i++)
            {
                Dictionary<string, string> dic = listInfo[i];

                if (playerInfoList.Count != 0)
                {
                    //显示名称
                    playerInfoList[i].transform.GetChild(0).GetComponent<Text>().text = dic["name"];
                    //显示结果
                    string idAndEndIndex = "<size=24>（Id: {0}）</size>\n<size= 28> 打到{1} </size>";
                    string result = string.Format(idAndEndIndex, dic["pid"], GlobalData.StringUpgradeIndex(dic["level"]));
                    playerInfoList[i].transform.GetChild(1).GetComponent<Text>().text = result;
                    if (dic["pid"] == PlayerInfo.Instance.mPlayerPid.ToString())
                    {
                        playerInfoList[i].transform.GetChild(0).GetComponent<Text>().color = new Color(55 / 255.0f, 83 / 255.0f, 41 / 255.0f, 1);
                        playerInfoList[i].transform.GetChild(1).GetComponent<Text>().color = new Color(55 / 255.0f, 83 / 255.0f, 41 / 255.0f, 1);
                    }
                }
            }
        }
        roomInfoText.text += string.Format("回放码:{0}", data.roomid);
    }

    private void OnCloseBtn(GameObject go)
    {
        UIManagers.Instance.DestroySingleUI(UIType.RecordItemDetail);
    }

    private void OnDestroy()
    {
        DOTween.Kill(detailTween, false);
    }
}
