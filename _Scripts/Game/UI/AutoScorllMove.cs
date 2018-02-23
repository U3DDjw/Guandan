using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using MsgContainer;
/// <summary>
/// 1.支持水平移动，暂不支持垂直移动
/// 2.同时这个脚本需要和AutoMoveScrollRect挂载同一个对象上面
/// </summary>
public class AutoScorllMove : MonoBehaviour
{
    [SerializeField]
    ScrollRect scrollRect;
    [SerializeField]
    GameObject content;//对象content
    [SerializeField]
    float autoMoveIntervalTime = 5;//自动移动间隔时间
    [SerializeField]
    float smoothSpeed = 1;//两个content子物体移动的平滑时间
    [SerializeField]
    bool isAutoMove = true;//提供选择是否自动移动
    [HideInInspector]
    public bool isContinueAutoMove = true;//当玩家拖拽的时候停止自动移动
    float contentWidth;
    float hasWaitTime = 0;
    float contentCellSizeX;//单个的content子物体的宽
    MoveDirection targetDir = MoveDirection.Left;
    List<GameObject> listImg = new List<GameObject>();//card列表
    List<float> listPosX = new List<float>();//储存card置中时候的位置
    private void Start()
    {
        contentCellSizeX = GlobalData.mDatingNoticContentCellSizeX;
        for (int i = 0; i < content.transform.childCount; i++)
        {
            GameObject gm = content.transform.GetChild(i).gameObject;
            listImg.Add(gm);
            if (i == 0)
                listPosX.Add(gm.transform.localPosition.x);
            else
                listPosX.Add(listPosX[0] + i * contentCellSizeX);
        }
    }
    void Update()
    {
        if (!isAutoMove) return;
        if (!isContinueAutoMove) return;

        hasWaitTime += Time.deltaTime;
        if (hasWaitTime > autoMoveIntervalTime)
        {
            hasWaitTime = 0;
            StartCoroutine(ContentAutoMove(targetDir));
        }
    }
    /// <summary>
    /// 自动移动
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public IEnumerator ContentAutoMove(MoveDirection dir)
    {
        if (contentWidth == 0)
            contentWidth = content.GetComponent<RectTransform>().rect.width;
        Vector3 contentPos = content.transform.localPosition;
        float startPercent = 0;
        if (dir == MoveDirection.Left)
        {
            float x = Mathf.Clamp(contentPos.x - contentCellSizeX, -(contentWidth - contentCellSizeX), 0);
            while (startPercent < 1)
            {
                yield return null;
                startPercent = Mathf.Clamp01(startPercent);
                content.transform.localPosition = Vector3.Lerp(contentPos, new Vector3(x, contentPos.y, 0), startPercent += Time.deltaTime * smoothSpeed);

            }
            if (x <= -contentWidth + contentCellSizeX * 3 / 2)
            {
                targetDir = MoveDirection.Right;
            }
        }
        else if (dir == MoveDirection.Right)
        {
            float x = Mathf.Clamp(contentPos.x + contentCellSizeX, -(contentWidth - contentCellSizeX), 0);
            while (startPercent < 1)
            {
                yield return null;
                startPercent = Mathf.Clamp01(startPercent);
                content.transform.localPosition = Vector3.Lerp(contentPos, new Vector3(x, contentPos.y, 0), startPercent += Time.deltaTime * smoothSpeed);
            }
            if (x >= -contentCellSizeX / 2)
            {
                targetDir = MoveDirection.Left;
            }
        }
    }
    public void ChildToViewportCenter(MoveDirection dir)
    {
        int index = 0;//移动到的位置定位
        float curContentPos_X = content.transform.localPosition.x;
        for (int i = 0; i < listPosX.Count - 1; i++)//最后一个不需要判断
        {
            float distance = listPosX[i] + curContentPos_X;
            Debug.Log("distance" + distance);
            if (Mathf.Abs(distance) < contentCellSizeX) //小于差值
            {
                index = i;
                Debug.Log("i:" + i);
                break;
            }
        }
        if (MoveDirection.Right == dir)//向左移动在判断的时候接近的值本身就是在左边的
        {
            index++;
        }
        //最大值最小值处理【距离而言】
        if (Mathf.Abs(curContentPos_X) > Mathf.Abs(contentCellSizeX * (listPosX.Count - 1)))//最大值
        {
            return;
        }
        else if (curContentPos_X > 0)//最小值
        {
            return;
        }
        //-----------------
        //Debug.Log("index" + index);
        index = Mathf.Clamp(index, 0, listPosX.Count - 1);
        float startPercent = 1 - Mathf.Abs((listPosX[index] + curContentPos_X) / contentCellSizeX);//根据距离计算移动过去需要的时间
        //Debug.Log("startPercent" + startPercent);
        //开始移动
        StartCoroutine(StartToCenterMove(startPercent, -index * contentCellSizeX));
    }
    IEnumerator StartToCenterMove(float startPercent, float endX)
    {
        Vector3 contentPos = content.transform.localPosition;
        float percent = startPercent;//时间叠加
        while (percent < 1)
        {
            yield return null;//等待0.1秒
            percent += Time.deltaTime;
            percent = Mathf.Clamp(percent, 0, 1.0f);
            //Debug.Log("percent:" + percent);
            float x = Mathf.Lerp(contentPos.x, endX, percent);
            content.transform.localPosition = new Vector3(x, contentPos.y, 0);

        }
        //Debug.Log(content.transform.localPosition);
        isContinueAutoMove = true;
    }
}
