using HaoyunFramework;
using MsgContainer;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class CheckCardIdContext : BaseContext
{
    public CheckCardIdContext()
    {
        ViewType = UIType.CheckCardIdView;
    }
}
public class UICheckCardIdView : BasesView {

    [SerializeField]
    Text sendId;
    [SerializeField]
    Text sendName;

    

    [SerializeField]
    GameObject confirmBtn;


    [SerializeField]
    GameObject closeBtn;

    

  

    private void Start()
    {
        AddUIListener();
    }

  
    void AddUIListener()
    {
        EventTriggerListener.Get(closeBtn.gameObject).onClick = OnCloseClick;
        EventTriggerListener.Get(confirmBtn.gameObject).onClick = OnConfirmClick;
    }

    void OnCloseClick(GameObject g)
    {
        ContextManager.Instance.Pop(this.curContext);
    }

    void OnConfirmClick(GameObject g)
    {
        var tempDic = new Dictionary<string, string>();
        tempDic.Add("pid", PlayerInfo.Instance.mPlayerPid.ToString());
        tempDic.Add("appId", GlobalData.mAppId);

        tempDic.Add("sfzName", sendName.text);

        tempDic.Add("sfzNo", sendId.text);
        tempDic.Add("time", (System.DateTime.Now.Ticks).ToString());

        WWWForm form = new WWWForm();
        form.AddField("sig", RSAVerify.SerifizationSignature(form,tempDic));

        StartCoroutine(GlobalData.SendPost(MsgContainer.GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_SfzName, form, (WWW www) => LoadSuc(www)));
    }

    void LoadSuc(WWW www)
    {
        Debug.Log(www.text);
        var data = JsonManager.GetSfzSucData(www.text);
        Debug.Log(data.detail);
        if(data.result == 1)
        {
            CheckIdComplete();
        }
        else
        {
            UIManagers.Instance.EnqueueTip(data.detail);
        }
    }

    void CheckIdComplete()
    {
        UIManagers.Instance.EnqueueTip("身份验证成功");
        ContextManager.Instance.Pop(curContext);
    }
}
