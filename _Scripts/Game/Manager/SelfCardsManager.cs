using UnityEngine;
using System.Collections;
using Common;
using System.Collections.Generic;
using MsgContainer;
using Net;
using ZhiWa;
using System.Linq;
using DNotificationCenterManager;
using HaoyunFramework;

public class NextDropMoveType
{
    public bool mIsLeft = false;
    public int mHorizontalIdx = 0;
}

public class SelfCardsManager : SingleTon<SelfCardsManager>
{
    //获取当前牌的id号码，对id号码作出处理
    uint mCommonId
    {
        get
        {
            //  return 5;
            return PlayingGameInfo.Instance.mCommonJokerCard;
        }
    }

    /// <summary>
    /// 获取是否多张 玩法 0 默认单张玩法
    /// </summary>
    public bool IsPlaySingleOrMulity
    {
        get
        {
            if(XPlayerPrefs.Instance == null)
            {
                return true;
            }
            return XPlayerPrefs.Instance.mGamePlaySingleOrMulity == 0;
        }
        set
        {
            XPlayerPrefs.Instance.mGamePlaySingleOrMulity = value ? 0 : 1;
        }
    }
    /// <summary>
    /// 判断是否是通用牌
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsCommId(uint id)
    {
        if (id == mCommonId || id - GlobalData.mEachCardsCount == mCommonId || id == GlobalData.mCardLevelId)
        {
            return true;
        }
        return false;
    }


    List<SingleCard> cardDataList = new List<SingleCard>();
    ///游戏最开始的所有牌，（固定，27张）依托 PlayingGameInfo 中mSelfBaseCardIdList
    public List<SingleCard> mCardDataList
    {
        get
        {
            return cardDataList;
        }
    }


    public void AddCardDataListItem(SingleCard singleCard)
    {
        IEnumerator<SingleCard> iSings = mCardDataList.GetEnumerator();
        while (iSings.MoveNext())
        {
            if (iSings.Current.mId == singleCard.mId)
            {
                Debug.LogError("重复添加singleCard" + singleCard.mId);
                return;
            }
        }

        mCardDataList.Add(singleCard);
    }
    public void RemoveCardDataListItem(uint id)
    {
        for (int i = 0; i < cardDataList.Count; i++)
        {
            if (cardDataList[i].mId == id)
            {
                cardDataList[i].mIsIn = false;
                cardDataList.RemoveAt(i);
                return;
            }
        }
    }



    public string getCardNameFromInt(int cardNum)
    {
        string cardF = ""; // 花色
        string cardZ = ""; // 面值
        int f = cardNum / 18;
        switch (f)
        {
            case 0:
            case 4:
                cardF = "红桃";
                break;
            case 1:
            case 5:
                cardF = "黑桃";
                break;
            case 2:
            case 6:
                cardF = "方片";
                break;
            case 3:
            case 7:
                cardF = "梅花";
                break;
        }
        int z = cardNum % 18;
        switch (z)
        {
            case 11:
                cardZ = "J";
                break;
            case 12:
                cardZ = "Q";
                break;
            case 13:
                cardZ = "K";
                break;
            case 14:
                cardZ = "A";
                break;
            case 16:
                cardF = "";
                cardZ = "小王";
                break;
            case 17:
                cardF = "";
                cardZ = "大王";
                break;
            default:
                cardZ = "" + z;
                break;
        }

        return cardF + cardZ;
    }


    private List<uint> curSelectCardIds = new List<uint>();
    /// <summary>
    /// 当前选中牌的唯一 的idx的集合
    /// </summary>
    public List<uint> mCurSelectCardIds
    {
        get
        {
            if (curSelectCardIds == null)
            {
                curSelectCardIds = new List<uint>();
            }

            return curSelectCardIds;
        }

        set
        {
            curSelectCardIds.Clear();
            curSelectCardIds = value;
        }
    }
    /// <summary>
    /// 出牌数据同步(更新isIn状态)
    /// </summary>
    /// <param name="idx"></param>
    public void UpdatePutOutInCardList(uint id)
    {
        for (int i = 0; i < cardDataList.Count; i++)
        {
            if (cardDataList[i].mId == id)
            {
                cardDataList[i].mIsIn = false;
                return;
            }
        }
    }

    /// <summary>
    /// 获取当前真实存在的手牌数量
    /// </summary>
    /// <returns></returns>
    public int GetRealCardCount()
    {
        int count = 0;
        for (int i = 0; i < mRealCardDic.Count; i++)
        {
            for (int j = 0; j < mRealCardDic[i].Count; j++)
            {
                count++;
            }
        }

        return count;
    }
    public void AddSelectCard(uint id)
    {
        if (curSelectCardIds.Contains(id) || id == 0)
        {
            return;
        }

        curSelectCardIds.Add(id);
    }

    public void RemoveSelectCard(uint id)
    {
        if (curSelectCardIds.Contains(id))
        {
            curSelectCardIds.Remove(id);
        }
    }

    public SingleCard GetSingleCardById(uint id)
    {
        IEnumerator<SingleCard> iCards = mCardDataList.GetEnumerator();
        while (iCards.MoveNext())
        {
            if (iCards.Current.mId == id)
            {
                return iCards.Current;
            }
        }
        return null;
    }

    void PostCardsToNormalStatus()
    {
        ArgsCardStatus msg = new ArgsCardStatus();
        IEnumerator<uint> iters = SelfCardsManager.Instance.GetOwnCardIdList().GetEnumerator();
        while (iters.MoveNext())
        {
            msg.idList.Add(iters.Current);
        }
        msg.status = ECardStatus.ENormal;

        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EChangeCardSelectStatus, msg);
    }
    /// <summary>
    /// 重置所有牌为非震动
    /// </summary>
    public void SendShakeNormalCards()
    {
        var list = SelfCardsManager.Instance.GetOwnCardIdList();
        ArgsShakeCard msg = new ArgsShakeCard();
        msg.mList.AddRange(list);
        msg.isShake = false;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ETouchCardShake, msg);
    }

    public void ClearAllSelectCards()
    {
        //清空View层
        PostCardsToNormalStatus();
        //清空数据
        mCurSelectCardIds.Clear();

    }

    /// <summary>
    /// 获取当前没有拥有的牌
    /// </summary>
    public List<uint> GetOwnCardIdList()
    {
        var list = new List<uint>();
        for (int i = 0; i < cardDataList.Count; i++)
        {
            if (cardDataList[i].mIsIn)
            {
                list.Add(cardDataList[i].mId);
            }
        }
        return list;
    }
    /// <summary>
    /// 获取当前的被选择中的一张牌所飞往的位置
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public Vector3 GetSelectCardPutOutToTargetPos(uint idx)
    {
        Dictionary<uint, Vector3> dic = new Dictionary<uint, Vector3>();
        uint count = (uint)curSelectCardIds.Count;
        IEnumerator<uint> iters = curSelectCardIds.GetEnumerator();
        while (iters.MoveNext())
        {
            dic.Add(iters.Current, GetSelectTargetPosX(iters.Current, count));
        }
        if (dic.ContainsKey(idx))
        {
            return dic[idx];
        }
        return GlobalData.mSelectCardToTargetPos;
    }

    //当前真实拥有的牌，分好组，排好序之后,存储的位置，与（realCardDic保持一致，牌一定是存在的）,位置
    public Vector3 GetRealCardPos(List<List<SingleCard>> dic, uint idx)
    {
        //两幅牌，有两张相同的牌,idx在前的就在下面
        int horizontalCount = dic.Count;
        int realHorizontalIdx = GetHorizontalIndexInRealDic(dic, idx);

        int verticalCount = GetInHorizontalRealDicCards(dic, idx).Count;
        int realVerticalIdx = GetVerticalIndexInRealDic(dic, idx);

        float xPos = GetCurPosX(realHorizontalIdx, horizontalCount, GetCurDistanceCardPosX(horizontalCount));
        float yPos = GetCurPosY(realVerticalIdx, verticalCount, GlobalData.mSingelCardRateY);
        return new Vector3(xPos, yPos, 0);
    }


    /// <summary>
    /// 通过idx，获取到当前的posX
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public float GetPosXByHorizontalIdx(int idx)
    {
        int totalCount = realCardDic.Count;
        float xPos = GetCurPosX(idx, totalCount, GetCurDistanceCardPosX(totalCount));
        return xPos;
    }


    /// <summary>
    /// 获取当前两张卡牌的X的距离
    /// </summary>
    /// <param name="dic"></param>
    /// <returns></returns>
    public float GetCurDistanceCardPosX(int totalCount)
    {
        return Mathf.Clamp(GlobalData.mCardContainerWidth / totalCount, 1, GlobalData.mSingleCardWidth);
    }


    public Vector3 GetLastDicPos(List<List<SingleCard>> dic, uint idx)
    {
        //两幅牌，有两张相同的牌,idx在前的就在下面
        int horizontalCount = dic.Count;
        int realHorizontalIdx = GetHorizontalIndexInRealDic(dic, idx);

        int verticalCount = GetInHorizontalRealDicCards(dic, idx).Count;
        int realVerticalIdx = GetVerticalIndexInRealDic(dic, idx);

        float xPos = GetCurPosX(realHorizontalIdx, horizontalCount, Mathf.Clamp(GlobalData.mCardContainerWidth / (3 * horizontalCount), 1, GlobalData.mLastSingelCardWidth / 2));
        float yPos = GetCurPosY(realVerticalIdx, verticalCount, GlobalData.mLastSingelCardRateY);
        return new Vector3(xPos, yPos, 0);

    }

    /// <summary>
    /// 获取当前的idx的card所在realDic中的所在第几个位置
    /// </summary>
    /// <param name="idx">唯一的idx</param>
    /// <returns></returns>
    int GetHorizontalIndexInRealDic(List<List<SingleCard>> dic, uint idx)
    {
        for (int k = 0; k < dic.Count; k++)
        {
            var value = dic[k];
            for (int i = 0; i < value.Count; i++)
            {
                if (value[i].mId == idx)
                {
                    return k;
                }
            }
        }
        return 0;
    }

    /// <summary>
    /// 返回值为从小到上，(牌所在realDic中，纵向位置),返回0-7
    /// </summary>
    /// <param name="idx"></param>
    int GetVerticalIndexInRealDic(List<List<SingleCard>> dic, uint idx)
    {
        var list = GetInHorizontalRealDicCards(dic, idx);

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].mId == idx)
            {
                return i;
            }
        }
        return 0;
    }

    /// <summary>
    /// 返回idx所在每一horizontal所有值
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    List<SingleCard> GetInHorizontalRealDicCards(List<List<SingleCard>> dic, uint idx)
    {
        for (int k = 0; k < dic.Count; k++)
        {
            var value = dic[k];
            for (int i = 0; i < value.Count; i++)
            {
                if (value[i].mId == idx)
                {
                    return value;
                }
            }
        }
        return null;
    }
    /// <summary>
    /// 获取当前idx的x轴位置
    /// </summary>
    /// <param name="idx">0- count-1 </param>
    /// <param name="total">总的横向数量</param>
    /// <returns></returns>
    float GetCurPosX(int idx, int total, float ratePosX)
    {
        //  float ratePosX = ;

        float middle = total % 2 == 0 ? total / 2 - 0.5f : total / 2;
        return (idx - middle) * ratePosX;
    }

    float GetCurPosY(int idx, int total, float rateY)
    {
        //  float rateY =;  /*Mathf.Clamp(mCardContainerWidth / total, 1, GlobalData.mSingleCardWidth);*/
        return idx * rateY;
    }

    public Vector3 GetSelectTargetPosX(uint value, uint total)
    {

        IEnumerator<uint> iters = curSelectCardIds.GetEnumerator();
        int idx = 0;
        while (iters.MoveNext())
        {
            if (iters.Current == value)
            {
                break;
            }
            idx++;
        }

        float ratePosX = GlobalData.mSingleCardWidth / 2;

        float middle = total % 2 == 0 ? total / 2 - 0.5f : total / 2;
        var tempPos = GlobalData.mSelectCardToTargetPos;
        tempPos.x += (idx - middle) * ratePosX;
        return tempPos;
    }



    /// <summary>
    /// 要不起
    /// </summary>
    public void SendRefusePutOutCard()
    {
        ClearAllSelectCards();  //清空所有被选中的打出的牌
        MsgGlobal mGl = new MsgGlobal();
        mGl.guandan_room = new MsgGuandanRoom();
        mGl.guandan_room.action = new MsgAction();
        MsgAction msg = mGl.guandan_room.action;
        msg.action_id = (ulong)PlayerInfo.Instance.mPlayerPid;
        msg.action_ct = TGuanDanCT.CT_BUCHU;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_SHOW_CARD, mGl);
    }




    /// <summary>
    /// 出牌请求
    /// </summary>
    public void SendPutOutCard()
    {

        MsgGlobal mGl = new MsgGlobal();
        mGl.guandan_room = new MsgGuandanRoom();
        mGl.guandan_room.action = new MsgAction();
        MsgAction msg = mGl.guandan_room.action;
        msg.action_id = (ulong)PlayerInfo.Instance.mPlayerPid;
        for (int i = 0; i < mCurSelectCardIds.Count; i++)
        {
            var id = mCurSelectCardIds[i];
            msg.action_card.Add(id);
        }
        //待补充缝人配，2代替后的牌发到服务器。
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_SHOW_CARD, mGl);
    }

    public void SendPutOutCardWithCT(TGuanDanCT type)
    {
        MsgGlobal mGl = new MsgGlobal();
        ZhiWa.MsgAction msg = new ZhiWa.MsgAction();
        mGl.guandan_room.action= msg;
        msg.action_id = (ulong)PlayerInfo.Instance.mPlayerPid;
        for (int i = 0; i < mCurSelectCardIds.Count; i++)
        {
            var id = mCurSelectCardIds[i];
            msg.action_card.Add(id);
        }

        msg.action_ct = type;

        //待补充缝人配，2代替后的牌发到服务器。
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_SHOW_CARD, mGl);
    }
    /// <summary>
    /// 取余之后的实际值
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public uint ValueById(uint id)
    {
        if (id > GlobalData.mEachCardsCount)
        {
            id -= GlobalData.mEachCardsCount;
        }

        if (id == mCommonId)
        {
            return id;
        }

        if (id % GlobalData.mCardNumber == mCommonId)
        {
            return 15;
        }
        return id % GlobalData.mCardNumber;
    }

    /// <summary>
    /// 把白塔牌当做 15来处理(方便排序),A当做1来处理
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public uint ValueByIdForNum(uint id)
    {
        if (id > GlobalData.mEachCardsCount)
        {
            id -= GlobalData.mEachCardsCount;
        }

        if (id == mCommonId)
        {
            return 0;
        }

        if (id % GlobalData.mCardNumber == 14) //A当做1来处理
        {
            return 1;
        }
        return id % GlobalData.mCardNumber;
    }

    /// 验证当前选中的牌是否合理
    /// </summary>
    public bool mIsOkPutOutCard
    {
        get
        {
            if (mCurSelectCardIds.Count == 0)
            {
                UIManagers.Instance.EnqueueTip("请选择你要出的牌");
                return false;
            }
            return true;
        }
    }


    /// <summary>
    /// 通过List，发送给需要改变状态为select的卡牌
    /// </summary>
    /// <param name="list"></param>
    public void PostSendCardToSelectStatus(List<uint> list, ECardStatus status)
    {
        ArgsCardStatus msg = new ArgsCardStatus();
        IEnumerator<uint> iMsg = list.GetEnumerator();
        while (iMsg.MoveNext())
        {
            msg.idList.Add(iMsg.Current);
        }
        msg.status = status;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EChangeCardSelectStatus, msg);
    }

    /// <summary>
    /// 排成一列（原理：改变当前的realCardDic） (刷新位置)
    /// </summary>
    public void OrderRealDicToList(int horizontalIdx)
    {
        if (curSelectCardIds.Count == 0)
        { return; }
        for (int i = 0; i < curSelectCardIds.Count; i++)
        {
            int deleteId = 0;
            SingleCard deleteData = null;
            for (int j = 0; j < mRealCardDic.Count; j++)
            {
                bool isBreak = false;
                IEnumerator<SingleCard> iters = mRealCardDic[j].GetEnumerator();
                while (iters.MoveNext())
                {
                    if (iters.Current.mId == curSelectCardIds[i])
                    {
                        deleteId = j;
                        deleteData = iters.Current;
                        isBreak = true;
                        break;
                    }
                }
                if (isBreak) { break; }
            }
            if (mRealCardDic.Count == 0) { break; }

            mRealCardDic[deleteId].Remove(deleteData);
            if (mRealCardDic[deleteId].Count == 0)
            {
                mRealCardDic.RemoveAt(deleteId);
            }
        }

        var tempSingleValue = new List<SingleCard>(); //添加的最右边的那一列
                                                      //   SortByValueIdMinToMax(curSelectCardIds);
        for (int i = 0; i < curSelectCardIds.Count; i++)
        {
            var dt = GetSingleCardById(curSelectCardIds[i]);
            if (dt != null)
            {
                tempSingleValue.Add(dt);
            }

        }
        //将tempSingleValue 排序(从小打到)
        if (horizontalIdx == 0)
        {
            mRealCardDic.Insert(0, tempSingleValue);
        }
        else if(horizontalIdx == 100) //100代表最后
        {
            mRealCardDic.Add(tempSingleValue);
        }
        else //用于拖拽
        {
            if (horizontalIdx > mRealCardDic.Count) { horizontalIdx = mRealCardDic.Count; }
            mRealCardDic.Insert(horizontalIdx, tempSingleValue);
        }

        for(int num = mRealCardDic.Count -1;num>=0;num--)
        {
            if (mRealCardDic[num].Count == 0) { mRealCardDic.RemoveAt(num); }
        }
    }


    #region 拖拽牌

    /// <summary>
    /// 拖拽状态
    /// </summary>
    public bool mIsDragDroping = false;
    public bool mIsDowing = false;

    /// <summary>
    /// 按住桌面
    /// </summary>
    public bool mIsPressingBg = false;
    /// <summary>
    /// 拖拽初始Start所在列数
    /// </summary>
    public int mInitDrogDropIdx = 0;


    /// <summary>
    /// 全部都是select
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public bool IsAllSelect(List<uint> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (!curSelectCardIds.Contains(list[i]))
            {
                return false;
            }
        }
        return true;
    }

    public List<uint> GetIdListByHorizontalIdx(int horizontalIdx)
    {
        List<uint> tempList = new List<uint>();
        var list = realCardDic[horizontalIdx];
        for (int i = 0; i < list.Count; i++)
        {
            tempList.Add(list[i].mId);
        }
        return tempList;
    }
    /// <summary>
    /// 全部都不是select
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public bool IsAllNotSelect(List<uint> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (curSelectCardIds.Contains(list[i]))
            {
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// 获取实际不在拖拽列表的idx 为0 的 x坐标
    /// </summary>
    /// <returns></returns>
    List<float> GetViewRealDicPosX()
    {
        List<float> list = new List<float>();
        for (int i = 0; i < realCardDic.Count; i++)
        {
            if (i != mInitDrogDropIdx)
            {
                list.Add(realCardDic[i][0].transform.localPosition.x);
            }
        }
        return list;
    }
    /// <summary>
    /// 通过PositionX ，获取到当前属于哪一列(实际列表中第一个大于的nextIdx则是目标idx)
    /// </summary>
    /// <param name="x"></param>
    public int GetRealHorizontalIdxByPostion(float x)
    {

        List<float> curRealDicPosXList = GetViewRealDicPosX();

        int count = curRealDicPosXList.Count;
        for (int i = 0; i < count; i++)
        {
            if (i == 0)
            {
                if (x < curRealDicPosXList[i])
                {
                    return 0;
                }
                if (curRealDicPosXList.Count > 1 && x > curRealDicPosXList[i] && x < curRealDicPosXList[i + 1])
                {
                    return 1;
                }

                if (i == count - 1) //只有1列的情况
                {
                    if (x >= curRealDicPosXList[i])
                    {
                        return i + 1;
                    }
                }
            }
            else if (i == count - 1)
            {
                if (x >= curRealDicPosXList[i])
                {
                    return i + 1;
                }
                else if (x > curRealDicPosXList[i - 1] && x < curRealDicPosXList[i])
                {
                    return i;
                }
            }
            else
            {
                if (x < curRealDicPosXList[i + 1] && x > curRealDicPosXList[i])
                {
                    return i + 1;
                }
                else if (x > curRealDicPosXList[i - 1] && x <= curRealDicPosXList[i])
                {
                    return i;
                }
            }

        }
        return 0;
    }
    /// <summary>
    /// 反向搜出最近的没有被选中的id值(本列开始搜，只有最近的左边那列，依次类推)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    uint GerRecerseRealIdBtId(int kValue)
    {
        for (int i = kValue; i >= 0; i--)
        {
            for (int j = 0; j < realCardDic[i].Count; j++)
            {
                var v = realCardDic[i][j];
                if (!curSelectCardIds.Contains(v.mId))
                {
                    return v.mId;
                }
            }
        }
        return 0;
    }
    /// <summary>
    /// 找出可作为拖拽初始化位置的id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    uint GetLocalThisId(uint id)
    {
        if (mCurSelectCardIds.Contains(id))
        {
            for (int k = 0; k < realCardDic.Count; k++)
            {
                for (int i = 0; i < realCardDic[k].Count; i++)
                {
                    if (realCardDic[k][i].mId == id)
                    {
                        return GerRecerseRealIdBtId(k);
                    }
                }
            }
        }
        return id;
    }

    public void OrderRealDicToList(uint curId, out int nextIdx)
    {
        if (curSelectCardIds.Count == 0)
        {
            nextIdx = 0;
            return;
        }

        curId = GetLocalThisId(curId);
        List<List<SingleCard>> tempRealList = new List<List<SingleCard>>();
        for (int i = 0; i < realCardDic.Count; i++)
        {
            var list = new List<SingleCard>();
            IEnumerator<SingleCard> iters = realCardDic[i].GetEnumerator();
            while (iters.MoveNext())
            {
                list.Add(iters.Current);
            }
            tempRealList.Add(list);
        }

        SortByValueIdMinToMax(curSelectCardIds);
        for (int i = 0; i < curSelectCardIds.Count; i++)
        {
            int deleteId = 0;
            SingleCard deleteData = null;
            for (int j = 0; j < tempRealList.Count; j++)
            {
                IEnumerator<SingleCard> iters = tempRealList[j].GetEnumerator();
                while (iters.MoveNext())
                {
                    if (iters.Current.mId == curSelectCardIds[i])
                    {
                        deleteId = j;
                        deleteData = iters.Current;
                        break;
                    }
                }
            }
            tempRealList[deleteId].Remove(deleteData);
            if (tempRealList[deleteId].Count == 0)
            {
                tempRealList.RemoveAt(deleteId);
            }
        }

        var tempSingleValue = new List<SingleCard>(); //添加的最右边的那一列
                                                      //   SortByValueIdMinToMax(curSelectCardIds);
        for (int i = 0; i < curSelectCardIds.Count; i++)
        {
            var dt = GetSingleCardById(curSelectCardIds[i]);
            if (dt != null)
            {
                tempSingleValue.Add(dt);
            }

        }
        //将tempSingleValue 排序(从小打到)
        nextIdx = GetListNextIdxById(curId, tempRealList);
        tempRealList.Insert(nextIdx, tempSingleValue);
       // Debug.Log("nextIdxPos:" + nextIdx);
        realCardDic.Clear();

        for (int num = 0; num < tempRealList.Count; num++)
        {
            realCardDic.Add(tempRealList[num]);
        }
    }

    /// <summary>
    /// 将数组从大到小排列，按牌的实际值大小排序，用于显示
    /// </summary>
    void SortByValueIdMinToMax(List<uint> list)
    {
        //通过idx得到id，然后valueById，排序
        IEnumerator<uint> ienums = list.GetEnumerator();
        Dictionary<uint, ArgsCardData> tempDic = new Dictionary<uint, ArgsCardData>(); //idx，值
        while (ienums.MoveNext())
        {
            ArgsCardData args = new ArgsCardData();
            args.mId = ienums.Current;
            args.mNum = ValueById(ienums.Current);
            tempDic.Add(args.mId, args);
        }

        var dicSort = from objDic in tempDic orderby objDic.Value.mNum ascending select objDic;
        list.Clear();
        foreach (KeyValuePair<uint, ArgsCardData> kvp in dicSort)
        {
            list.Add(kvp.Value.mId);
        }
    }

    int GetListNextIdxById(uint id, List<List<SingleCard>> list)
    {
        //依据当前没有选中的牌进行Id计算
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list[i].Count; j++)
            {
                if (list[i][j].mId == id)
                {
                    return i;
                }
            }
        }
        return 0;
    }

    /// <summary>
    /// 调换两个ItemList
    /// </summary>
    /// <param name="idxMin"></param>
    /// <param name="idxMax"></param>
    public void ReverseTwoListInRealDic(int initIdx, int curIdx)
    {
        var tempItemInit = realCardDic[initIdx];
        realCardDic.RemoveAt(initIdx);
        //realCardDic.Add(tempItemInit);
        realCardDic.Insert(curIdx, tempItemInit);

        //记录 理牌顺序
        SaveSortRecord(curSelectCardIds, curIdx);
    }
    void SaveSortRecord(List<uint> list, int curIdx)
    {
        List<uint> tempList = new List<uint>();
        for (int i = 0; i < list.Count; i++)
        {
            tempList.Add(list[i]);
        }
        SelfCardsManager.Instance.mSortRecordList.Add(tempList);

        SelfCardsManager.Instance.mSortRecordTypeList.Add(curIdx); //同花顺则放到第一个位置，否则放到最后一个位置(100代替)
    }

    #endregion
    public void GetRealCardDataIsOut(uint idx)
    {
        for (int i = 0; i < realCardDic.Count; i++)
        {
            IEnumerator<SingleCard> iters = realCardDic[i].GetEnumerator();
            while (iters.MoveNext())
            {
                if (iters.Current.mId == idx)
                {
                    iters.Current.mIsIn = false;
                    return;
                }
            }
        }
    }
    /// <summary>
    /// 刷新CardData 的 isIn参数
    /// </summary>
    public void RefreshRealDic()
    {
        for (int k = 0; k < realCardDic.Count; k++)
        {
            for (int i = realCardDic[k].Count-1;i>=0 ; i--)
            {
                var t = realCardDic[k][i];
                if (t.mIsIn == false)
                {
                    realCardDic[k].Remove(t);
                }
            }
        }

        for (int i = realCardDic.Count - 1; i >= 0; i--)
        {
           if(realCardDic[i].Count == 0)
            {
                realCardDic.RemoveAt(i);
            }
        }


    }

    /// <summary>
    /// 重新排列恢复
    /// </summary>
    public void ResetToInitDic()
    {
        InitRefreshRealCardDic(SelfCardsManager.Instance.mCardDataList, SelfCardsManager.Instance.mRealCardDic);
    }

    #region 同花顺提示
    int tonghuashunSelectIdx = -1;

    private List<MsgCardGroup> tonghuashunTipList = new List<MsgCardGroup>();

    public List<MsgCardGroup> mTonghuashunTipList
    {
        get
        {
            return tonghuashunTipList;
        }
        set
        {
            tonghuashunTipList = new List<MsgCardGroup>();
            tonghuashunTipList = value;
            tonghuashunSelectIdx = Mathf.Clamp(tonghuashunSelectIdx, -1, tonghuashunTipList.Count - 1);
        }
    }


    public MsgCardGroup GetCurTonghuashunTip()
    {
        if (tonghuashunSelectIdx >= mTonghuashunTipList.Count - 1)
        {
            tonghuashunSelectIdx = -1;
        }
        tonghuashunSelectIdx++;
        return mTonghuashunTipList[tonghuashunSelectIdx];
    }

    public bool IsNoneTonghuashunTip
    {
        get
        {
            return mTonghuashunTipList.Count == 0;
        }
    }

    public bool IsNoneRequestTonghuashunTip
    {
        get
        {
            return tonghuashunSelectIdx == -1;
        }
    }

    public void initTonghuashunTip()
    {
        tonghuashunSelectIdx = -1;
    }

    public void PostTonghuashunToCards()
    {
        if (IsNoneTonghuashunTip)
        {
            UIManagers.Instance.EnqueueTip("没有同花顺!!!");
            return;
        }
        ClearAllSelectCards();  //先清空当前存在的

        MsgCardGroup group = GetCurTonghuashunTip();
        var list = GetGroupTipCardIdxList(group);
        PostSendCardToSelectStatus(list, ECardStatus.ESelected);
    }

    public void SendServerTonghuashunTip()
    {
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_TONGHUASHUN);
    }



    #endregion

    #region 提示牌功能
    public int mPressTipCardTimes = -1;
    /// <summary>
    /// 提示牌
    /// </summary>
    private List<MsgCardGroup> tipCardList = new List<MsgCardGroup>();

    /// <summary>
    /// 每次收到自己出牌时候调
    /// </summary>
    /// <param name="list"></param>
    public void UpdateTipCardList(List<MsgCardGroup> list)
    {
        // Debug.Log("更新提示牌数量:" + list.Count);
        tipCardList.Clear();
        mPressTipCardTimes = -1;
        if (list == null || list.Count == 0)
        {
            return;
        }
        IEnumerator<MsgCardGroup> ienums = list.GetEnumerator();
        while (ienums.MoveNext())
        {
            tipCardList.Add(ienums.Current);
        }
    }


    public bool mIsRecevCardTip = false;
    public MsgAction mMsgAction = new MsgAction();
    public void SendServerCardTip()
    {
        SelfCardsManager.Instance.mIsRecevCardTip = false;
        MsgGlobal mGl = new MsgGlobal();
        mGl.guandan_room = new MsgGuandanRoom();
        mGl.guandan_room.action = mMsgAction;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_CARD_TIP, mGl);
    }
    /// <summary>
    /// 有木有提示牌
    /// </summary>
    /// <returns></returns>
    public bool IsNoHaveTipCard()
    {
        if (tipCardList == null || tipCardList.Count == 0)
        {
            return true;
        }
        return false;
    }


    int TipCardCount()
    {
        if (tipCardList == null)
        {
            return 0;
        }
        return tipCardList.Count;
    }


    MsgCardGroup GetCurTipCards()
    {
        if (IsNoHaveTipCard()) { return null; }

        mPressTipCardTimes++;
        return tipCardList[mPressTipCardTimes % TipCardCount()];
    }

    //Send Select Card

    List<uint> GetGroupTipCardIdxList(MsgCardGroup group)
    {
        var list = new List<uint>();
        IEnumerator<uint> ienums = group.card.GetEnumerator();
        while (ienums.MoveNext())
        {
            for (int k = 0; k < realCardDic.Count; k++)
            {
                bool isBreak = false;
                var v = realCardDic[k];
                IEnumerator<SingleCard> ienumsTemp = v.GetEnumerator();
                while (ienumsTemp.MoveNext())
                {
                    if (ienumsTemp.Current.mId == ienums.Current && !list.Contains(ienumsTemp.Current.mId))
                    {
                        list.Add(ienumsTemp.Current.mId);
                        isBreak = true;
                        break;
                    }
                }
                if (isBreak)
                {
                    break;
                }

            }
        }
        return list;
    }


    /// <summary>
    /// 提示牌显示发送
    /// </summary>
    public void SendTipSelectCard()
    {
        MsgCardGroup group = GetCurTipCards();
        if (group == null) { return; }
        var list = GetGroupTipCardIdxList(group);
        PostSendCardToSelectStatus(list, ECardStatus.ESelected);
    }
    #endregion



    public string GetCardAudioFileName(TGuanDanCT type, uint id, EAudioStyle style)
    {
        string file = "";
        //这边待补充，posType的不同，播放不同语音效果
        switch (type)
        {
            case TGuanDanCT.CT_SINGLE:
                file = GlobalData.mAudioSingleTitle + ValueByIdForAudio(id).ToString();
                break;
            case TGuanDanCT.CT_DOUBLE:
                file = GlobalData.mAudioDoublleTitle + ValueByIdForAudio(id).ToString();
                break;
            case TGuanDanCT.CT_FOUR_KING:
            case TGuanDanCT.CT_SI_ZHANG_BOMB:
            case TGuanDanCT.CT_WU_ZHANG_BOMB:
            case TGuanDanCT.CT_LIU_ZHANG_BOMB:
            case TGuanDanCT.CT_QI_ZHANG_BOMB:
            case TGuanDanCT.CT_BA_ZHANG_BOMB:
            case TGuanDanCT.CT_JIU_ZHANG_BOMB:
            case TGuanDanCT.CT_SHI_ZHANG_BOMB:
                file = "audio_zhadan";
                break;
            case TGuanDanCT.CT_GANG_BAN:
                file = "audio_feiji";
                break;
            case TGuanDanCT.CT_LIANG_LIAN_DUI:
                file = "audio_sanliandui";
                break;
            case TGuanDanCT.CT_SHUN_ZI:
                file = "audio_shunzi";
                break;
            case TGuanDanCT.CT_THREE_TIAO:
                file = "audio_sanzhang";
                break;
            case TGuanDanCT.CT_TONG_HUA_SHUN:
                file = "audio_tonghuashun";
                break;
            case TGuanDanCT.CT_HU_LU:
                file = "audio_sandaier";
                break;
            case TGuanDanCT.CT_BUCHU:
                file = GlobalData.mAudioBuyao;
                break;
        }
        return GlobalData.GetAudioBasePathByStyle(style) + file;
    }
    //当前真实拥有的牌，分好组，排好序之后。
    private List<List<SingleCard>> realCardDic = new List<List<SingleCard>>();
    public List<List<SingleCard>> mRealCardDic
    {
        get
        {
            if (realCardDic == null)
            {
                realCardDic = new List<List<SingleCard>>();
            }
            return realCardDic;
        }
    }



    /// <summary>
    /// 将List整理成列
    /// </summary>
    /// <param name="cardList">已经有的singleCardList</param>
    /// <param name="cardDic">重新理完的dic</param>
    public void InitRefreshRealCardDic(List<SingleCard> cardList, List<List<SingleCard>> cardDic)
    {
        cardDic.Clear();
        var tempDic = new Dictionary<uint, List<SingleCard>>();
        var tempList = new List<uint>();
        for (int i = 0; i < cardList.Count; i++)
        {
            if (!cardList[i].mIsIn || cardList[i] == null) //排除已经出过的牌
            {
                continue;
            }
            uint num = ValueById(cardList[i].mId);

            if (tempList.Contains(num))
            {
                tempDic[num].Add(cardList[i]);
            }
            else
            {
                tempDic.Add(num, new List<SingleCard>() { cardList[i] });
                tempList.Add(num);
            }
        }
        //Sort
        Sort(tempList);
        //把Common牌放到王之后，普通牌之前
        SortCommonIdCard(tempList);
        for (int i = 0; i < tempList.Count; i++)
        {
            uint key = tempList[i];
            cardDic.Add(tempDic[key]);
        }


    }


    /// <summary>
    /// 获取当前id所在的列的singlecard集合
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    List<SingleCard> GetRealDicOneList(uint id)
    {
        for (int k = 0; k < mRealCardDic.Count; k++)
        {
            for (int i = 0; i < mRealCardDic[k].Count; i++)
            {
                if (mRealCardDic[k][i].mId == id)
                {
                    return mRealCardDic[k];
                }

            }
        }
        return null;
    }

    /// <summary>
    /// 获取当前所在id的列数的所有id集合，除了每一列最下面.
    /// </summary>
    public List<uint> GetCurRealDicIdList(uint id)
    {
        var singleList = GetRealDicOneList(id);
        if (singleList == null) { return null; }
        List<uint> list = new List<uint>();
        for (int i = 0; i < singleList.Count; i++)
        {
            list.Add(singleList[i].mId);
        }

        list.RemoveAt(0); //排除最下面的card
        return list;
    }

    public int GetCurRealDicInListIdx(uint id)
    {
        for (int k = 0; k < mRealCardDic.Count; k++)
        {
            var v = mRealCardDic[k];
            for (int i = 0; i < v.Count; i++)
            {
                if (v[i].mId == id)
                {
                    return i;
                }
            }
        }
        return 0;
    }
    public int GetCurRealDicHorizontalIdx(uint id)
    {
        for (int k = 0; k < mRealCardDic.Count; k++)
        {
            var v = mRealCardDic[k];
            for (int i = 0; i < v.Count; i++)
            {
                if (v[i].mId == id)
                {
                    return k;
                }
            }
        }
        return 0;
    }
    public void SortCommonIdCard(List<uint> list)
    {
        if (!list.Contains(mCommonId))
        {
            return;
        }
        int idx = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] > 15) //得到王的数量
            {
                idx++;
            }
        }

        list.Remove(mCommonId);
        list.Insert(idx, mCommonId);
    }



    public void Sort(List<uint> list)
    {
        //从大到小排序(优先通用牌放在最前面)
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list.Count - 1 - i; j++)
            {
                if (list[j] < list[j + 1])
                {
                    uint temp = list[j + 1];
                    list[j + 1] = list[j];
                    list[j] = temp;
                }
            }
        }
    }


    /// <summary>
    /// Audio播放不区分通用牌                                                                                                                                                                                                 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public uint ValueByIdForAudio(uint id)
    {
        if (id > GlobalData.mEachCardsCount)
        {
            id -= GlobalData.mEachCardsCount;
        }
        return id % GlobalData.mCardNumber;
    }
    /// <summary>
    /// 清空当前所有信息
    /// </summary>
    public void ClearInfo()
    {
        cardDataList.Clear();
        realCardDic.Clear();
        curSelectCardIds.Clear();
        ClearSortRecordInfo();
    }


    public void SendServerSortCard()
    {
        MsgGlobal mGl = new MsgGlobal();
        mGl.action = new MsgAction();
        MsgAction msg = mGl.action;
        for (int i = 0; i < mCurSelectCardIds.Count; i++)
        {
            msg.action_card.Add(mCurSelectCardIds[i]);
        }
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_SORT_CARD, mGl);
    }


    #region 理牌记录(含拖拽)

    private List<List<uint>> sortRecordList = new List<List<uint>>();
    public List<List<uint>> mSortRecordList
    {
        get
        {
            if (sortRecordList == null)
            {
                sortRecordList = new List<List<uint>>();
            }
            return sortRecordList;
        }
    }

    private List<int> sortRecordTypeList = new List<int>();

    public List<int> mSortRecordTypeList
    {
        get
        {
            if (sortRecordTypeList == null)
            {
                sortRecordTypeList = new List<int>();
            }
            return sortRecordTypeList;
        }
    }

    public void ClearSortRecordInfo()
    {
        mSortRecordList.Clear();
        mSortRecordTypeList.Clear();
    }


    #endregion


    /// <summary>
    /// 刷新图层
    /// </summary>
    /// <param name="cardsList"></param>
    public void ResetSibling(List<List<SingleCard>> cardsList)
    {
        for (int i = cardsList.Count - 1; i >= 0; i--)
        {
            var data = cardsList[i];
            for (int j = 0; j < data.Count; j++)
            {
                data[j].transform.SetAsFirstSibling();
            }
        }
    }


    public void SetSibling(List<uint> list)
    {
        for(int i=list.Count-1;i>=0;i--)
        {
            GetSingleCardById(list[i]).transform.SetAsLastSibling();
        }
    }



}
