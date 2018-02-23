using UnityEngine;
using System.Collections;
using System;


#region 枚举
//界面动画类型
public enum EViewAnimType
{
    ENUll = 0,
    //大厅
    EBindBtn = 1,
    EInviteBtn = 2,
    EPerson = 3,//大厅的女生的动画
    //战斗界面
    ETouyou = 6,

}
//大厅公告移动方向
public enum EConentMoveDir
{
    ENull = 0,
    ELeft = 1,
    ERight = 2
}

public enum EGameChanel
{
    ENull =0,
    EHY=1, //
    EDLJ=2,//大蓝鲸
    Ehytf=3,//好运投放
    Ehyycgd=4,//好运盐城掼蛋
}
public enum EGameMode
{
    ENUll = 0,
    EDebug = 1,//测试
    EFormal = 2,//正式
    EDalanjing,//大蓝鲸
    EAppleOnLine,//苹果上线
}

public enum EAtlasType
{
    ENull = 0,
    EMain = 1,
    EPlaying = 2,
    ECard = 3,
}

public enum EGameServerUrl
{
    ENull = 0,
    EFK = 1, //FK
    ESCZ = 2,//测试服
    EFormalTest = 3,
    EOnLine_TEST = 4,//压力测试服

    EOnLine = 5,//正式


}

/// <summary>
/// 自己的SinglCard 状态
/// </summary>
public enum ECardStatus
{
    ENull = 0,
    ENormal = 1,//正常
    ESelected = 2,//被选中
    EOuted = 3,//被打出显示在桌面
    EDestroyed = 4, //已销毁

    EOther = 5, //其他玩家卡牌
    EOrder = 6,//排成一列中
    ETransparent = 7,//透明状态（看桌面）

    ERefreshCard = 8, //刷新出来的牌
    ELightCard = 9,//明牌
}
public enum ECardDesignType
{

    ENull = 0,
    /// <summary>
    /// 红心
    /// </summary>
    EHeart = 1,
    /// <summary>
    /// 黑桃
    /// </summary>
    ESpade = 2,
    /// <summary>
    /// 方块
    /// </summary>
    EDiamond = 3,//方块
    /// <summary>
    /// 梅花
    /// </summary>
    EClub = 4,//梅花

    ESmallJoker = 5,//小王
    EBigJoker = 6,//大王
}
/// <summary>
/// 用于缓存的纹理图片的key,方便调用
/// </summary>
public enum ETextureName
{
    ENull = 0,
    ENotic = 1,//公告纹理
    EDatingNotic = 2,//大厅公告
    EHeadTex1 = 3,//头像纹理
}
/// <summary>
/// 其他玩家相对自己的位置编号
/// </summary>
public enum EPlayerPositionType
{
    ENull = 0,
    ERight = 1,
    ETop = 2,//
    ELeft = 3,//
    ESelf = 4,//自己
}

/// <summary>
/// 战斗中的各个状态
/// </summary>
public enum EPlayingStatus
{
    ENull = 0,
    EReady = 1,
    ETribute = 2,
    EPlaying = 3,
    EBureau = 4,//结算状态
}
public enum ENotificationMsgType
{
    ENull = 0,
    EPutOutCard = 1,//可以出牌消息(一般是服务器收到可以出牌之后的回调之后出牌)
    ENET_MSG_TIMEOUT = 2,//请求超时
    EDownLoadProgress = 3, //下载进度
    EInputNumber = 4,//输入房间号
    ELoginSucc = 5,//UDP请求登录成功
    ELoginFail = 6,//UDP请求登录失败

    EGUANDAN_ROOM_GD_FKBZ = 7, //创建房间房卡不足
    EGUANDAN_ROOM_GD_KF_SUC = 8,//创建房间成功
    EGUANDAG_ROOM_LEAVE = 9,//房间解散消息

    ERoomJoin_FKBZ = 10,//进入房间房卡不足 (弃用)
    ERoomJoin_RSYM = 11,//进入房间，人数已满(弃用)
    ERoomJoin_Suc = 12,//进入房间成功
    ERoomJoin_other_join = 13,//其他玩家进入房间
    ERoom_other_leave = 14,//其他玩家退出
    ESendCanReadyGame = 15,//第四个人加入游戏，发送可以开始游戏倒计时。
    EInitLicense = 16,//正式开始游戏，发牌吧
    ECanPutOutCard = 17,//通知可以发牌消息
    ESelfPutOutCardSuc = 18,//自己出牌成功回调
//    ESelfPutOutCardFail = 19,//自己出牌失败回调
    EFirstPutOutCard = 20,//首先出牌
    EChangeCardSelectStatus = 21,//改变卡牌选中的状态

    EGameClean = 22,//游戏中某个玩家获得胜利（头游，二游）
    EUpdatePlayerInfo = 23,//刷新游戏中角色数据

    EGameBureauOver = 24,//游戏每局结束
    EJoinRoomFail = 25,//加入房间失败

    EDissmisRoomApply = 26,//申请解散房间

    ESingleDissRoomApplyResult = 27,//玩家申请解散房间 发出回应消息
    EDissmisRoomResultFail = 28,//玩家解散结果 失败消息
    EDissmisRoomResultSuc = 29,//玩家解散结果 成功消息

    ESameIpPlayer = 30,//相同IP
    EOfflinePlayer = 31,//玩家离线
    EOnLinePlayer = 32,//玩家重新上线

    ETotalBureauOverGame = 33,//总结算推送消息

    ETributeSuc = 34, //进贡，回贡成功消息

    EIpConfirmNotify = 35,//Ip 冲突确认
    EStartGame = 36,//开始游戏（出牌按钮显示）

    EGetEmailSuccess = 37,//领取邮件成功

    ELoginByWeChatSuc = 38, // 微信登录成功

    EApplyChangeSeat = 39,//申请调换座位

    EAgreeChangeSeat = 40,//回答调换座位

   // ERefreshData = 41,//刷新数据（用于丢失数据等异常，刷新出牌之类）

    EPlayerPlayingStatus = 42,// 轮到某玩家出牌

    EGoldNotEnough = 43,//金币不足
    EUseProp = 44,//使用道具

    ETalk = 45,//语音
    EReconnectRefresh = 46,//断线重连
    EResetLogin = 47,//重新登录（清除缓存，调用微信登录）

    EUseEmoticon = 48,//使用表情

    EUpdateTip = 49,//收到Tip消息

    EUpdateCreateRoomSpr = 50,//刷新加入房间图片（加入 || 返回）

    EHideLogining = 51, // 隐藏旋转按钮

    EZhuandanStartGame = 52,//转蛋开始回调，用于明牌播放完

    EStartLoadingGame = 53,// 开始加载数据之前
    ESortCard = 54, //理牌

    ETouchCardShake = 55,//触动牌震动效果

    ETalkNotify = 56,//语音推送
    ERefreshEmailBall = 57,//Email红点刷新

    EHorseRefresh = 58,//跑马灯UDP刷新

    EResetWaitGameView = 59,//重新刷新等待场景

    ESwitchTweenBackwards = 60,//战斗界面 SwitchBtn

    EStopAllCoroutines = 61,//关闭协程

    EReturnHomeView = 62,//等待房间中的回到大厅

    EStartDragDrop = 63,// 开始拖拽
    EDragDroping = 64,//正在拖拽

    //   ECardMoveX =65,// 牌移动X轴

    EReleaseDragDrop = 66,//拖拽停止

    ERefreshDragDropPosAndDepth = 67,//刷新拖拽中的牌的位置

    EChooseCardType = 68,//牌型选择
    EBackToLoginScene = 69,//返回登录界面

    EGoldReadySuc = 70,//金币场准备成功
    EGoldOutSuc = 71,//金币场退出成功

    EGoldViewBack=72,//金币场回到主界面

	EGoldRefreshStartGame = 73,//重新刷新数据（用于金币场每局开始重刷数据）
	EGoldOverBureau = 74,//金币场结算点击结束（显示换桌，退出，准备等）

    EGoldReadyStatus = 75, //金币场 小结算状态（准备/换桌）
}

public enum EButtonAnimationType
{
    ENull = 0,
    EBind = 1,//绑定有礼
    EInvite = 2,//邀请有礼
    EHomePerson = 3,//大厅人的动画
}


public enum EJoinRoomFailReason
{
    ENull = 0,
    EFKBZ = 1,//房卡不足
    ERSYM = 2,//人数已满
    EROOM_NULL = 3,//房间号不存在
}

public enum EPlayersTeam
{
    ENull = 0,
    ETeamA = 1, //默认，玩家自己始终与Top 为A组
    ETeamB,//左边和右边玩家为B组
}
//邮件的奖励类型
public enum EEmailType
{
    ENUll = 0,
    ERoomCard = 1,//房卡类型
    EGoldCoin = 2,//金币类型
}
public enum EGuandanGameStatus
{
    ENull = 0,
    EGuandanNormal = 1,
    EGuandanKanggong = 2,
    EGuandan2 = 3,//第一局（因为第一局没有抗贡之类,没有游之类的,一直打2，即：打2的状态）
}

public enum EGameStatus
{
    ENone = 0,//None
    EInit = 1,//初始化

    ELoading = 2,//加载中
    ELogin = 3,//登录界面
    EHome = 4,//游戏外围界面
    EPlaying = 5,//正在打牌


    EDisconnect = 6,//离线
    EOther = 6,
}
//绝对 位置的类型
public enum ETransformType
{
    ENull = 0,
    ECardItemContainer = 1,//牌的位置
    EOutCardItemContainer = 2,  //出牌位置
    EHeadFrame = 3,//头像位置
    ELightCard=4,//明牌位置
    ETimerClock=5,//倒计时
    EBackUp=6,//备用
}

public enum EJoinRoomType
{
    ENull =0,
    EEnter =1,//加入
    EExchange =2,//换房

}

public enum EnumOperation
{
    Add,
    Remove,
    Update,
    Clear,
    None
}

public enum ESelectType
{
    ENull = 0,
    EA = 1,
    EB = 2,
    EC = 3,
    ED = 4,
    EE = 5,
    EF = 6//暂时的选择最大到6

}
/// <summary>
/// 按钮事件
/// </summary>
public enum EnumButtonEvent
{
    OnClick,
    OnClick4Tab,
    OnDoubleClick,
    CheckOn,
    CheckOff,
    OnPress,
    TouchDown,
    TouchUp,
    OnDrop,
    OnLongPress,

}

/// <summary>
/// 出牌声音枚举值
/// </summary>
public enum EAudioEffectType : int
{
    ENull = 0,
    E2 = 2,
    E3 = 3,
    E4 = 4,
    E5 = 5,
    E6 = 6,
    E7 = 7,
    E8 = 8,
    E9 = 9,
    E10 = 10,
    EJ = 11,
    EQ = 12,
    EK = 13,
    EA = 14,
    ESmallJoker = 16,
    EBigJoker = 17,
}


public enum EDragDropDirectionType
{
    ENull = 0,
    ELeft = 1,
    EStop = 2,
    ERight = 3,
}
#endregion

#region 委托
public delegate void OnTouchButtonHandler(ButtonScript obj, object args, int param1, int param2);// touch button
public delegate void DataChangedHandler(string key, object newValue, object oldValue, EnumOperation operation);
public delegate void HandleFinishDownload(WWW www);
public delegate void HandleMakeSureEvent();
public delegate T[] DeleGetArrayFormLua<T>();//获取数组方法的委托
public delegate void FinishCallHandler();
public delegate void UnityApplicationAction(bool isPause);

#endregion

//public class GameStateKey
//{
//    // 服务器地址;
//    public const string ServerHost = "192.168.1.151";
//    public const string ServerPort = "17970";
//    //	版本控制
//    //public const string VerSion = "VerSion";
//    //public const string DataVersion = "DataVersion";
//    //public const string ResDownLoadPath = "ResDownLoadPath";
//    //public const string HomePage = "HomePage";
//    //public const string IsUseLocalFile = "IsUseLocalFile";
//}
