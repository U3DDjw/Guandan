using UnityEngine;
using System.Collections;
using System;
using ZhiWa;
using System.Collections.Generic;
using XLua;
namespace MsgContainer
{
    //===================== 传递的自定义消息===============================================================
    public class ArgsPlayBackRoomInfo : EventArgs
    {
        public int gameIndex;//第几局
        public int gameTotalNum;
        public TGuanDanGameType gameType;
        public int roomCode;
        public string roomCodeMd5;
    }
    public class ArgsTest : EventArgs
    {
        public int testId;
        public string testName;
    }



    public class ArgsDownLoadProgress : EventArgs
    {
        public float total;//下载文件总的kb
        public float progress;//进度
    }

    public class ArgsChangeRoomNumber : EventArgs
    {
        public int Id; // 数字
    }

    public class ArgsJoinRoomReason : EventArgs
    {
        public EJoinRoomFailReason reason;
    }
    public class ArgsMsgRoomInfo : EventArgs
    {
        public TGuanDanCardUseType card_use_type; //房卡使用模式
        public uint game_num;//房间局数
        public ulong creater_id; //房间创建者id
        public uint creater_pay;//房主消耗房卡
        public uint other_pay;//其他玩家消耗房卡
        public TGuanDanGameType game_type;//游戏类型
        public List<MsgGdEnding> endingInfo;//结束的游戏数据
    }

    public class ArgsChangeSeatInfo : EventArgs
    {
        public ulong applyId;
        public ulong toId;
        public int result;
    }

    public class ArgsCanputOutCard : EventArgs
    {
        public uint lastPlayerId;
        public uint playerId;
        public List<uint> cards = new List<uint>();
        public List<uint> cards_modify;//使用缝人配之后的列表，没有缝人配则为空
        public TGuanDanCT putOutType;//出牌类型
        public ulong lastOperationPId;
        public MsgActionType actionType;//当前出牌玩家类型（正常顺序，一圈结束 ，正常出牌）
        public List<MsgCardGroup> msgCardGroup; //多选牌的情况
        public List<MsgSendCardInfo> sendCardInfo;
        public List<uint> DjCards = new List<uint>();
    }
    public class ArgsDatingNotic : EventArgs
    {
        public string path;
        public bool isCanClick;
    }
    public class ArgsGameCleanInfo : EventArgs
    {
        public ulong playerId;
        public MsgGuandanGameRank rank;
    }
    public class ArgsCardData : EventArgs
    {
        public uint mId;
        public uint mNum;//实际的mId取余之后得出的值
    }


    public class ArgsCardStatus : EventArgs
    {
        public List<uint> idList = new List<uint>(); //被选中的的集合

        public ECardStatus status;
        public bool mIsAll = false;
    }

    public class ArgsFirstPutPlayer : EventArgs
    {
        public ulong playerId;
        public bool isStart;
    }

    public class ArgsMsgGameOverMsg : EventArgs
    {
        public List<MsgGameOverInfo> mGameOverList = new List<MsgGameOverInfo>();
        public List<MsgSendCardInfo> mShowLastCard = new List<MsgSendCardInfo>();
        public bool mIsTotalOver = false; //是否即将总结算
    }
    /// <summary>
    /// 某个玩家是否同意解散
    /// </summary>
    public class ArgsDismissRoomApplyResult : EventArgs
    {
        public uint playerId;
        public bool isAgree;
    }

    public class ArgsDissmissRoomApplyer : EventArgs
    {
        public ulong playerId;
        public string voteTime;
    }
    public class ArgsPlayerId :EventArgs
    {
        public ulong playerId;
    }

    public class ArgsSameIpPlayer : EventArgs
    {
        public List<MsgSameIpPlayer> list = new List<MsgSameIpPlayer>();
    }

    public class ArgsOfflinePlayer : EventArgs
    {
        public uint playerId;
    }

    public class ArgsIpConfirmPlayer : EventArgs
    {
        public uint playerId;
        public List<ulong> mPlayerIds = new List<ulong>();
    }

    public class ArgsMsgTotalScore : EventArgs
    {
        public List<MsgTotalScore> List = new List<MsgTotalScore>();
    }

    public class ArgsTribute : EventArgs
    {
        public ulong addId; //被贡者
        public ulong removeId;//贡者
        public uint card;
        public bool isJingong; // 是否是进贡
        public bool isStart; //是否开始游戏（游戏正式开始游戏决定于start_tag,放在回贡之中）

        public uint jgpz_card_native;//断线重连 我进贡的牌面值
        public uint hgpz_card_native;//断线重连 被还贡的牌面值
        public List<ulong> jg_player_id;//进贡的玩家
        public List<ulong> bjg_player_id;//还贡玩家列表
        public List<uint> jgpz_card;//进贡的牌的列表
    }


    public class ArgsReceiveEmail : EventArgs
    {
        public string emailId;
    }
    public class ArgsMsgTest : EventArgs
    {
        public string testStr = "";
    }

    public class ArgsWeChat : EventArgs
    {
        public string refreshToken;
        public string access_token;
        public string openid;
    }

    public class ArgsPlayerPut : EventArgs
    {
        public ulong playerId;
        public ulong lastOperationId;
        public MsgActionType type;
        public bool isYbq = false;
    }

    public class ArgsCard : EventArgs
    {
        public TGuanDanCT type;
    }

    public class ArgsPlayerList:EventArgs
    {
        public List<ulong> playerIdList =new List<ulong>();
    }


    public class ArgsRefreshData : EventArgs
    {
        public List<MsgGameOverInfo> gameOverInfoList;
        public List<MsgLast3Card> last3CardList;
        public List<MsgPlayerInfo> playerInfoList;
        public List<MsgGdEnding> endingList;
        public MsgTributeInfo tributeInfo;
        public MsgAction action;
        public List<uint> oldSelfCardIdList;
        public List<uint> djCards = new List<uint>();


    }
    public class ArgsPropsInfo : EventArgs
    {
        public ulong action_id;//道具使用者
        public MsgProps propsId;//道具ID
        public ulong target_id;//攻击目标id
    }
    public class ArgsEmoticonInfo : EventArgs
    {
        public ulong action_id; // 使用者ID
        public MsgEmoticon emoticonId; // 表情id
        public string message; // 文字
    }
    public class ArgsTalk : EventArgs
    {
        public string voiceName;//语音
        public uint talkPid;//发起者id
        public float time;//语音时长
    }

    public class ArgsShakeCard : EventArgs
    {
        public bool isShake = false;
        public List<uint> mList = new List<uint>();
    }


    public class ArgsStartDragDrop : EventArgs
    {
        public uint mCardId;

    }

    public class ArgsDragDroping : EventArgs
    {
        public float deltaPosX;
    }

    //======================================================================================================
    public class ServerUrlTitle
    {
        public static readonly string Url_Login = "www/login"; //登录
        public static readonly string Url_EditName = "name/change";//修改名字
        public static readonly string Url_EditHead = "headPortrait/change";//修改头像

        public static readonly string Url_GetPlayerInfo = "email/playerInfo";//玩家信息 刷新金币用
        public static readonly string Url_NoReadEmail = "email/noReadEmail"; //未读邮件列表
        public static readonly string Url_ReadedEmail = "email/ReadedEmail";//已读邮件列表
        public static readonly string Url_ReceiveEmail = "email/receive";//领取邮件

        public static readonly string Url_GetHorse = "horse/getHorse";//获取跑马灯

        public static readonly string Url_GetRecord = "record/getRecord";//获取玩家记录 总的
        //回放添加接口
        public static readonly string Url_GetPlayBackInfo = "record/getRecordViedo";//获取玩家回放数据  
        public static readonly string Url_GetRecordAllVideoInfo = "record/getRecordDetailedInfo";//获取某一局游戏真正局数包含每一局游戏结束详情
        public static readonly string Url_GetOtherRecordDetail = "record/getInfoById";//roomid获取房间信息

        public static readonly string Url_SendMessage = "phone/sendMsg";//发送验证码
        public static readonly string Url_BindSubmit = "phone/binding";//绑定提交

        public static readonly string Url_GetInviterInfo = "invite/getInfo";//获取邀请者
        public static readonly string Url_BindInviter = "invite/setInviterId";//绑定邀请者

        public static readonly string Url_NoticInfo = "notice/get";//公告面板

        public static readonly string Url_Iosiap = "iosiap/check";//apply pay ,callback check

        public static readonly string Url_ActivityInfo = "activity/getActivityInfo";
        public static readonly string Url_SfzName = "sfz/set";
    }
    public class ServerMsgKey
    {
        //客户端向服务器发送协议
        public static readonly string CLIENT_BEAT_HEART = "heartBeat";//心跳发送的 actioncode
        public static string CLIENT_LOGIN = "login";// 登录二次请求
        public static string CLIENT_QUIT_GAME = "quitGame";// 登录二次请求

        public static string CLIENT_IP_CONFIRM = "ipConfirm"; //IP重复确认

        public static string CLIENT_CREATE_ROOM  //  创建房间 
        {
            get
            {
                if (GameManager.Instance.mIsUseAI)
                {
                    return "test_createRoom";
                }
                return "createRoom";
            }
        }
        public static string CLIENT_ENTER_ROOM = "enterRoom";   //  进入房间
        public static string CLIENT_OUT_ROOM_BY_CREATER = "outRoomByCreater";   //游戏未开始时，房主解散房间
        public static string CLIENT_OUT_ROOM_BY_PLAYER = "outRoomByPlayer";     //游戏未开始时，玩家退出房间
        public static string CLIENT_READY_START  //准备开始游戏
        {
            get
            {
                if (GameManager.Instance.mIsUseAI)
                {
                    return "test_readyForStart";
                }
					if (RoomInfo.Instance.IsZhuandanGameType) {
						return "readyForStart";
					} else {
						return "gdReadyForStart";
					}
				}           
        }
        public static string CLIENT_CANCEL_READY_START = "cancleReadyForStart";//取消准备开始游戏
        public static string CLIENT_SHOW_CARD  //玩家出牌
        {
            get
            {
                if (GameManager.Instance.mIsUseAI)
                {
                    return "test_showCard";
                }
                if (GoldFiledManager.Instance.mIsGoldFiled)
                {
                    return "GoldShowCardService";
                }
                else
                {
                    if (RoomInfo.Instance.IsZhuandanGameType)
                    {
                        return "showCard";
                    }
                    else
                    {
                        return "gdShowCard";
                    }
                }
            }
        }

        public static string CLIENT_SORT_CARD = "lipai"; //理牌


        public static string CLIENT_DISS_ROOM = "dissRoom"; //申请解散房间（游戏中）
        public static string CLIENT_DISS_ROOM_AGREE = "dissRoomAgree";  //同意解散房间（游戏中）
        public static string CLIENT_DISS_ROOM_DISAGREE = "dissRoomDisagree";    //不同意解散房间（游戏中）

        public static string CLIENT_USE_PROPS = "useProps";//使用道具
        public static string CLIENT_TALK = "talk";  //语音
        public static string CLIENT_USE_EMOTICON = "useEmoticon";//使用表情

        public static string CLIENT_RECONNECT //= "reconnect";    //断线重连
        {
            get
            {
                return GoldFiledManager.Instance.mIsGoldFiled ? "GoldReconnectService" : "reconnect";
            }
        }
        public static string GUANDAN_ROOM_GD_SQDHZW = "GD_SQDHZW"; // 申请调换座位

        public static string CLIENT_TOTAL_GOAL = "totalGoal";//总结算
        public static string CLIENT_TRIBUTE// = "tribute"; //进贡
        {
            get
            {
                return GoldFiledManager.Instance.mIsGoldFiled ? "GoldTributeService" : "tribute";
            }
        }
        public static string CLIENT_BACK_TRIBUTE// = "backTribute";//还贡
        {
            get
            {
                return GoldFiledManager.Instance.mIsGoldFiled ? "GoldBackTributeService" : "backTribute";
            }
        }

        public static string CLIENT_TONGHUASHUN = "tonghuashun";//同花顺

        public static string CLIENT_APPLY_CHANGE_SEAT = "applyChangeSeats";//换座位
        public static string CLIENT_AGREE_CHANGE_SEAT = "agreeChangeSeats";//回答换座位

        public static string CLIENT_REFRESH //= "freshen";//刷新数据
        {
            get
            {
                return GoldFiledManager.Instance.mIsGoldFiled ? "GoldRefreshService" : "freshen";
            }
        }

        public static string CLIENT_CARD_TIP = "cardTip";//Tip消息

        public static string CLIENT_ENTER_GOLD_FILED = "GoldEnterPlaygroundService"; //加入金币场
        public static string CLIENT_READY_GOLD = "GoldReadyService"; //金币场准备

        public static string CLIENT_READY_SHOW_CARD = "GoldShowCardService";//金币场出牌
        public static string CLIENT_GOLD_QUIT_ROOM = "GoldQuitRoom";//退出房间

        public static string CLIENT_GOLD_EXCHANGE_ROOM = "GoldChangeRoom";//金币场 换桌

        //===============================================回调=================================================================

        public static string GUANDAN_ROOM_UPDATE_IPADS = "updateIpads"; // 更新玩家具体地址
        public static string GUANDAN_ROOM_WECHAT_SHARE_FOR_ROOMCARD = "wechatShareForRoomcard"; // 分享就送2张房卡，每日有效

        //服务器发送客户端回调消息
        public static string GUANDAN_ROOM_GD_RGI = "GD_RECGAMEINFO"; // 断线重连房间信息;
        public static string GUANDAN_ROOM_GD_OC = "GD_SHOW_CARD_SUC";       // 用户出牌消息;GD_SHOW_CARD_SUC  GD_OC
        public static string GUANDAG_ROOM_LEAVE = "GD_LE";          // 离开游戏;
        public static string GUANDAN_ROOM_GD_RDY = "GD_RDY";      // 玩家准备;  
        public static string GUANDAN_ROOM_GD_CRDY = "GD_CRDY";     // 玩家取消准备;  
        public static string GUANDAN_ROOM_GD_ZT_ZD = "GD_ZT_ZD"; // 玩家以为战斗状态 无法再创建房间
        public static string GUANDAN_ROOM_DISS_APPLY = "GD_APPDISS"; // 掼蛋私房申请解散
        public static string GUANDAN_ROOM_DISS_REFUSE = "GD_REFDISS"; // 掼蛋私房拒绝解散
        public static string GUANDAN_ROOM_DISS_AGREE = "GD_AGRDISS"; // 掼蛋私房同意解散
        public static string GUANDAN_ROOM_GD_FKBZ = "GD_FKBZ";          // 房卡不足;
        public static string GUANDAN_ROOM_GD_KF_SUC = "GD_KF_SUC";          // 开房成功;
        public static string GUANDAN_ROOM_GD_NO_ROOM = "GD_NO_ROOM";          // 房间不存在;
        public static string GUANDAN_ROOM_GD_JF_FAIL_FKBZ = "GD_JF_FAIL_FKBZ";          // 进入房间失败（房卡不足）;
        public static string GUANDAN_ROOM_GD_JF_FAIL_RSYM = "GD_JF_FAIL_RSYM";          // 进入房间失败（人数已满）;
        public static string GUANDAN_ROOM_GD_JF_SUC = "GD_JF_SUC";          // 进入房间成功
        public static string GUANDAN_ROOM_GD_NEW_PLAYER = "GD_NEW_PLAYER";          // 新玩家进入房间
        public static string GUANDAN_ROOM_GD_PLAYER_OUT = "GD_PLAYER_OUT";          // 游戏未开始时，玩家(非房主)退出房间
        public static string GUANDAN_ROOM_GD_SAME_IP = "GD_SAME_IP";          // IP冲突
        public static string GUANDAN_ROOM_GD_START_GAME = "GD_START_GAME";          // 游戏开始，发牌
        public static string GUANDAN_ROOM_GD_SHOW_CARD_SUC = "GD_SHOW_CARD_SUC";          // 出牌成功
        public static string GUANDAN_ROOM_GD_SHOW_CARD_FAIL = "GD_SHOW_CARD_FAIL";          // 出牌非法
        public static string GUANDAN_ROOM_GD_SHOW_CARD_CHOOSE = "GD_SHOW_CARD_CHOOSE";//牌型提示选择

        public static string GUANDAN_ROOM_GD_USE_PROP = "GD_USE_PROP";          // 使用道具
        public static string GUANDAN_ROOM_GD_TALK = "GD_TALK";          // 语音

        public static string GUANDAN_ROOM_GD_DISS_SUC = "GD_DISS_SUC"; // 房间解散成功
        public static string GUANDAN_ROOM_GD_DISS_FAIL = "GD_DISS_FAIL"; // 房间解散失败

        public static string GUANDAN_ROOM_LOGIN_SUC = "LOGIN_SUC"; // 请求成功
        public static string GUANDAN_ROOM_FAIL = "FAIL"; // 请求失败
        public static string GUANDAN_ROOM_GAME_ERR = "GD_GAME_ERR"; // 游戏错误


        public static string GUANDAN_ROOM_GAME_TOTALSCORE = "GD_GAME_TOTALSCORE"; // 转蛋总结算


        public static string GUANDAN_ROOM_GD_SQDHZW_SUC = "GD_SQDH_SUC"; // 调换申请成功
        public static string GUANDAN_ROOM_GD_SQDHZW_FAIL = "GD_SQDH_FAIL"; // 调换申请失败
        public static string GUANDAN_ROOM_GD_DHZW_SUC = "GD_DHZW_SUC"; // 调换座位成功
        public static string GUANDAN_ROOM_GD_DHZW_JJ = "GD_DHZW_JJ"; // 拒绝调换座位


        public static string GUANDAN_ROOM_GD_CARD_TIP = "GD_CARD_TIP";//提示回调协议
        public static string GUANDAN_ROOM_GD_GAMEOVER_CARD = "GD_GAMEOVER_CARD"; // 一局结束时，玩家剩余手牌
        public static string GUANDAN_ROOM_GD_T_H_S = "GD_T_H_S"; // 获取当前同花顺列表

        public static string GUANDAN_ROOM_GD_JG_QRSUC = "GD_JG_QRSUC"; // 惯蛋进贡确认成功
        public static string GUANDAN_ROOM_GD_JG_BJGERR = "GD_JG_BJGERR"; // 惯蛋进贡-被进贡者不对应
        public static string GUANDAN_ROOM_GD_JG_NOTMAX = "GD_JG_NOTMAX"; // 进贡的不是最大值
        public static string GUANDAN_ROOM_GD_JG_FQERR = "GD_JG_FQERR"; // 进贡发起失败
        public static string GUANDAN_ROOM_GD_JG_FQSUC = "GD_JG_FQSUC"; // 惯蛋进贡发起成功
        public static string GUANDAN_ROOM_GD_HG_XYS = "GD_HG_XYS"; // 回贡的牌小于10
        public static string GUANDAN_ROOM_GD_HG_FQERR = "GD_HG_FQERR"; // 回贡发起失败
        public static string GUANDAN_ROOM_GD_HG_FQSUC = "GD_HG_FQSUC"; // 惯蛋回贡发起成功
        public static string GUANDAN_ROOM_GD_HG_QRSUC = "GD_HG_QRSUC"; // 惯蛋回贡发起成功
        public static string GUANDAN_ROOM_GD_JG_HTJP = "GD_JG_HTJP";//进贡的为红桃级牌
        public static string GUANDAN_ROOM_IP_CONFIRME = "GD_IP_CONFIRME"; //ip异常后确认继续游戏回调

        public static string GUANDAN_ROOM_FRESHEN = "GD_FRESHEN"; // 刷新
        public static string GUANDAN_ROOM_NEED_RECONNECT = "GD_NEED_RECONNET";//强制需要断线重连

        public static string GUANDAN_ROOM_GD_NEED_FRESHEN = "GD_NEED_FRESHEN"; // 需要刷新

        public static string GUANDAN_ROOM_GD_USE_EMOTICON = "GD_USE_EMOTICON";//使用表情
        public static string GUANDAN_ROOM_GD_REPEAT_LOGIN = "GD_REPEAT_LOGIN"; // 账号异地重复登录 (通知掉线并 强制返回登录界面)


        public static string GUANDAN_ROOM_GD_LIPAI = "GD_LIPAI"; // 理牌

        public static string GUANDAN_ROOM_HORSE_INFO = "GD_HORSE"; // 跑马灯


        //金币场
        public static string GUANDAN_GOLD_NOT_ENOUGH = "GOLD_NOT_ENOUGH"; // 金币不够

        public static string GUANDAN_GOLD_ENTER_SUC = "GOLD_ENTER_SUC"; // 进入金币场

        public static string GUANDAN_GOLD_READY_SUC = "GOLD_READY_SUC"; // 金币场准备成功

        public static string GUANDAN_GOLD_OUT_PLAYGROUND_SUC = "GOLD_OUT_PLAYGROUND_SUC"; // 退出金币场

        public static string GUANDAN_ROOM_GOLD_GD_START_GAME = "GOLD_GD_START_GAME";          // 游戏开始，发牌


        public static string GUANDAN_ROOM_GAME_OFFLINE = "GD_GAME_OFFLINE"; // 游戏掉线
        public static string GUANDAN_ROOM_GD_ONLINE = "GD_ONLINE"; // 玩家上线




    }

    public class ServerErrorKey
    {
        public static readonly string ERROR_ERR = "0";//预留
        public static readonly string ERROR_NO_USER = "1";// 账户不存在。客户端token与服务端token不匹配。手机号未注册。 
        public static readonly string ERROR_FRONZEN_USER = "2"; // 账户被冻结
        public static readonly string ERROR_PASSWORD = "3"; // 密码错误
        public static readonly string ERROR_FIGHTSERVER = "4"; // 服务器维护中
    }

}
