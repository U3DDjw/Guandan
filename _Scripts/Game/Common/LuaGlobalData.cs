using MsgContainer;
using System.Collections.Generic;
using UnityEngine;

public interface ILuaGlobalData
{
    string mVersion { get; set; }
    string mGameName { get; set; }
    string mHostId { get; set; }                                     // 运营商ID
    string mHostFkId { get; set; }
    string mStreamingAssetsXml { get; set; }
    string mAppId { get; set; } //App Id

    uint mCardLevelId { get; set; }                              //专门为参谋牌的id值

    float mGameBureauOverWaitTime { get; set; }//小结算 ，小结算页面自动点击 等候时间
    float mLoadGameBreauOverWaitTime { get; set; }                  //最后出完牌等候小结算页面
    float mSameIpTotalTime { get; set; }//SameIp总时间
    float mStartGameWaitTime { get; set; }
    float mReadyGameWaitTime { get; set; }
    float mAutoPlayGameWaitTime { get; set; }


    int mPlayerOwnCardCount { get; set; }                       //54 * 2 / 4	//两幅牌，每个人初始化的牌牌数
    int mCardTypeCount { get; set; }//卡牌类型（4中花色）
    uint mCardNumber { get; set; }
    uint mRefuseCardNumId { get; set; }                         //不出 ，28专门为不出的idx
    float mRequestSpacingTime { get; set; }//出牌无回调，自动刷新

    float mDismissRoomCDTime { get; set; }//解散房间cd时间
    float mDismissRoomAutoResultTime { get; set; }//自动选择解散时间


    float mDatingNoticContentCellSizeX { get; set; }//大厅公告单个宽度

    uint mEachCardsCount { get; set; }//4*18
    float mSingleCardWidth { get; set; }                                //单张牌的宽度
    float mCardContainerWidth { get; set; }//横向最大宽
    float mSingelCardRateY { get; set; }//调节牌的高度间隔
    float mLastSingelCardRateY { get; set; }//调节剩余牌的高度间隔
    float mLastSingelCardWidth { get; set; }//调节剩余牌的高度间隔(与Small牌保持一致 / 4)
    float mPutOutCardRate { get; set; }                                 //打出的牌的与原尺寸大小
    float mCardTypeTipRate { get; set; }//牌型提示中牌是原尺寸的大小
    float mCardTouchMoveY { get; set; }//点击牌震动幅度
    float mCardTouchTime { get; set; }//点击震动时间
    float mDropDragHeight { get; set; }//拖拽状态中牌的高度值
    float mDragDrogTouchRate { get; set; }//拖动牌的灵敏度

    //《远程地址》
    string WeChatShareUrl { get; set; }
    string mTaskActivityHtmlUrl { get; set; }

    //《本地地址》
    string mLoadItemTitlePath { get; set; }
    string mLocalTexturePath { get; set; }                                                  //本地纹理的资源路劲
    string mLocalEmoticonPrefabPath { get; set; }//本地表情动画的预支体路径
    string mLocalPropPrefabPath { get; set; }//本地道具动画的预支体路径
    string mLocalViewAnimationPath { get; set; }//界面的动画的路径
    //动画名称
    string mAnimSortingLayer { get; set; }//动画的层级
    int mAnimOrderIdLayer { get; set; }// 层级排序 id
    //音频
    string AudioNameWarning { get; set; }
    string AudioNameStartGame { get; set; }
    string AudioNameOtherLeaveRoom { get; set; }
    string AudioNameClickBtn { get; set; }
    string AudioNameDaojishi { get; set; }
    string AudioNameJoinRoomPlayer { get; set; }
    string AudioNameSortCard { get; set; }
    string AudioNameGameOverWin { get; set; }
    string AudioNameGameOverLose { get; set; }
    string AudioNameGameOverPingju { get; set; }
    string AudioNameTotalGameOverWin { get; set; }
    string AudioNameTotalGameOverLose { get; set; }


    string mAudioBuyao { get; set; }
    string mAudioSingleTitle { get; set; }
    string mAudioDoublleTitle { get; set; }

    Vector3 mSelectCardToTargetPos { get; set; }
    Color mSelectCardColor { get; set; }
    //方法
    string getCardName(uint id);
    string getHeadCleanCpr(uint rank);
    string getAudioBasePathByStyle(EAudioStyle style);
    string[] getRenderTexturePaths();
    ArgsDatingNotic[] getDatingIosNoticArgs();
    ArgsDatingNotic[] getDatingNoticArgs();
    List<List<int>> goldFiledData();
    //常用语
    string[] getUsefulExpressions();
}

