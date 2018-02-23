using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class SelectOne : MonoBehaviour
{
    public delegate void DelSelectIsChanage(int type);
    public DelSelectIsChanage OnHasChange;
    public ESelectType mSelectTeam = ESelectType.EA;

    [SerializeField]
    List<GameObject> childCount = new List<GameObject>();
    [SerializeField]
    List<Text> GDLabels = new List<Text>();
    [SerializeField]
    List<Text> ZDLabels = new List<Text>();

    void Awake()
    {
        for (int i = 0; i < childCount.Count; i++)
        {
            GameObject objBtn = childCount[i].transform.GetChild(0).gameObject;
            EventTriggerListener.Get(objBtn).onClick = ButtonClick;

        }
    }
    //这里如果有新的成员，需要继续添加变化ESelectType
    void ButtonClick(GameObject go)
    {
        for (int i = 0; i < childCount.Count; i++)
        {
            childCount[i].transform.GetChild(1).gameObject.SetActive(childCount[i].transform.GetChild(0).gameObject == go);
            if (childCount[i] == go.transform.parent.gameObject)
            {
                mSelectTeam = (ESelectType)(i + 1);
            }
        }
        if (OnHasChange != null)//只在选择支付形式的时候用到
        {
            OnHasChange((int)mSelectTeam);
        }
    }
    public void SetDefaultCreateRoominfo(ESelectType index)
    {
        this.mSelectTeam = index;
        ButtonClick(childCount[(int)(index) - 1].transform.GetChild(0).gameObject);
    }

}
