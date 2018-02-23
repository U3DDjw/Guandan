using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
//using LitJson;
public class JsonManager
{

    //public static DataTest GetDataTest(string str)
    //   {
    //       JsonData json = JsonMapper.ToObject(str);
    //       DataTest dt = new DataTest();
    //       //dt.id = json["id"].ToInt32();
    //       //dt.name = json["name"];

    //   }

    public static DataTest GetJsonData(string str)
    {
        //DataTest dt = new DataTest();
        //dt.id = 1;
        //dt.name = "ss";
        //dt.tabs = new string[] { "ss", "dd" };
        //string json = JsonUtility.ToJson(dt);
        if (str.Length > 0)
        {
            return JsonUtility.FromJson<DataTest>(str);
        }
        return null;
    }

    public static FightServerData GetFightServerData(string str)
    {
        if (str.Length == 0 || str == null) return null;
        JsonData js = JsonMapper.ToObject(str);
        string json = js["fightServer"].ToJson();
        FightServerData fightServer = JsonMapper.ToObject<FightServerData>(json);
        Debug.Log("fightServer:" + fightServer.host);
        return fightServer;
    }
  

    public static bool GetIsSfzFlg(string str)
    {
        if (str.Length == 0 || str == null) return false;
        JsonData js = JsonMapper.ToObject(str);
        bool result = false;
         bool.TryParse(js["sfzFlg"].ToJson(),out result);
        return result;
    }
    public static bool GetIsSfzShow(string str)
    {
        if (str.Length == 0 || str == null) return false;
        JsonData js = JsonMapper.ToObject(str);
        bool result = false;
        bool.TryParse(js["sfzShow"].ToJson(), out result);
        return result;
    }

    public static List<HorseData> GetHourseData(string str)
    {
        List<HorseData> list = new List<HorseData>();
        if (str.Length == 0 || str == null) return null;
        JsonData js = JsonMapper.ToObject(str);
        for (int i = 0; i < js.Count; i++)
        {
            string json = js[i].ToJson();
            HorseData horseData = JsonMapper.ToObject<HorseData>(json);
            list.Add(horseData);
        }
        return list;
    }

    public static List<EmailData> GetEmailData(string str)
    {
        List<EmailData> list = new List<EmailData>();
        if (str.Length == 0 || str == null) return null;
        JsonData js = JsonMapper.ToObject(str);
        for (int i = 0; i < js.Count; i++)
        {
            string json = js[i].ToJson();
            EmailData emailData = JsonMapper.ToObject<EmailData>(json);
            list.Add(emailData);
        }
        return list;
    }
    public static PlayerData GetPlayerData(string str)
    {
        if (str.Length == 0 || str == null) return null;
        JsonData js = JsonMapper.ToObject(str);
        string json = js["player"].ToJson();
        PlayerData player = JsonMapper.ToObject<PlayerData>(json);
        return player;
    }

    public static WeChatLoginSucData GetWeChatLoginSucData(string str)
    {
        if (str.Length == 0 || str == null) return null;
        JsonData js = JsonMapper.ToObject(str);
        string json = js.ToJson();
        WeChatLoginSucData weChatData = JsonMapper.ToObject<WeChatLoginSucData>(json);
        return weChatData;
    }
    public static List<RecordData> GetRecordData(string str)
    {
        List<RecordData> recordList = new List<RecordData>();
        if (str.Length == 0 || str == null) return null;
        JsonData js = JsonMapper.ToObject(str);
        for (int i = 0; i < js.Count; i++)
        {
            string json = js[i].ToJson();
            RecordData record = JsonMapper.ToObject<RecordData>(json);
            recordList.Add(record);
        }
        return recordList;
    }

    public static InviterData GetInviterData(string text)
    {
        if (text.Length == 0 || text == null) return null;
        JsonData js = JsonMapper.ToObject(text);
        string json = js.ToJson();
        InviterData inverData = JsonMapper.ToObject<InviterData>(json);
        return inverData;
    }

    public static SfzData GetSfzSucData(string text)
    {
        if (text.Length == 0 || text == null) return null;
        JsonData js = JsonMapper.ToObject(text);
        string json = js.ToJson();
        var sfzData = JsonMapper.ToObject<SfzData>(json);
        return sfzData;
    }

  

    public static TextureURLData GetNoticData(string text)
    {
        if (text.Length == 0 || text == null) return null;
        JsonData js = JsonMapper.ToObject(text);
        string json = js.ToJson();
        TextureURLData noticData = JsonMapper.ToObject<TextureURLData>(json);
        return noticData;
    }


    public static PlayerMoneryInfo GetPlayerMoneyInfo(string str)
    {
        if (str.Length == 0 || str == null) return null;
        JsonData js = JsonMapper.ToObject(str);
        string json = js.ToJson();
        PlayerMoneryInfo player = JsonMapper.ToObject<PlayerMoneryInfo>(json);
        return player;
    }
    public static ActivityInfo GetActivityInfo(string text)
    {
        if (text.Length == 0 || text == null) return null;
        JsonData js = JsonMapper.ToObject(text);
        string json = js.ToJson();
        ActivityInfo player = JsonMapper.ToObject<ActivityInfo>(json);
        return player;
    }
    //详情的信息
    public static RecordDetailData GetRecordDetailData(string str)
    {
        if (str.Length == 0 || str == null) return null;
        return JsonMapper.ToObject<RecordDetailData>(str);
    }
    /// 战绩界面 可以回放的局数详细信息
    public static List<RecordVideoData> GetRecordVideoData(string str)
    {
        List<RecordVideoData> recordVideoList = new List<RecordVideoData>();
        if (str.Length == 0 || str == null) return null;
        JsonData js = JsonMapper.ToObject(str);
        for (int i = 0; i < js.Count; i++)
        {
            string json = js[i].ToJson();
            RecordVideoData recordVideo = JsonMapper.ToObject<RecordVideoData>(json);
            recordVideoList.Add(recordVideo);
        }
        return recordVideoList;
    }
    public static PlayBackData GetPlayBackData(string text)
    {
        if (text.Length == 0 || text == null) return null;
        PlayBackData backData = JsonMapper.ToObject<PlayBackData>(text);
        string fingtingLogData = backData.fightingLog;
        JsonData fightData = JsonMapper.ToObject(fingtingLogData);
        List<PlayBackFightData> listFight = new List<PlayBackFightData>();
        for (int i = 0; i < fightData.Count; i++)
        {
            string each = fightData[i].ToJson();
            listFight.Add(JsonMapper.ToObject<PlayBackFightData>(each));
        }
        backData.fightingLog = null;
        backData.fightingPutoutlist = listFight;
        return backData;
    }
}
[System.Serializable]
public class RecordDetailData//详情
{
    public string endTime;
    public List<PlayBackFightData> players;//allGoal
    public List<PlayBackFightData> endings;//level": , "member","win": "win_win" 使用的其中参数
    public int gameNum;
    public int roomCode;
    public string roomId;
    public int gameType;
    public List<PlayBackFightData> player;//name .pid
}
[System.Serializable]
public class RecordVideoData//回放的每局详情
{
    public int gameId;
    public List<PlayBackFightData> goals;
    public List<PlayBackFightData> endings;
    public int isOver;//1表示本局已经打完
}
[System.Serializable]
public class PlayBackData
{
    public List<PlayBackFightData> cards;//玩家牌的信息
    public string fightingLog;
    public int gameId;
    public int gameOver;//本局游戏结束 为1  游戏未结束 0
    public List<PlayBackFightData> playerGoals;//总积分 
    public List<PlayBackFightData> goals;//积分 
    public uint lightCard;//亮牌
    public List<ulong> lightPlayerIds; //亮牌玩家的id
    public List<PlayBackFightData> players;
    //掼蛋
    public PlayBackFightData tributeInfo;
    public List<PlayBackFightData> tributeRecordLst;//贡牌信息
    public List<PlayBackFightData> endings;
    public List<PlayBackFightData> lastEnding;//最后一局信息
    //fightingLog转换而来
    public List<PlayBackFightData> fightingPutoutlist;//打牌的信息
}

[System.Serializable]
public class PlayBackFightData
{
    public ulong pid;
    public int goal;
    public int allGoal;
    public int rank;
    public List<uint> cardList;
    public uint cardtype;
    public string headPortrait;
    public string name;
    //掼蛋
    public List<ulong> bjgPid;
    public List<ulong> jgPid;
    public List<uint> jgpz;//进贡的牌组
    public int kgbs;//不抗贡为0
    public uint card;//贡的牌id
    // 结束信息
    public int level;
    public List<ulong> member;
    public int win;//0赢 1输
    public int win_win;//双上 
    //吴用
    public string ipAds;
    public uint roomCode;
    public string token;
}
[System.Serializable]
public class ActivityInfo//活动信息
{
    public int battleNum;
    public List<Buddy> buddys;
    public long createTime;
    public string gameId;
    public ulong pid;
}
[System.Serializable]
public class Buddy
{
    public string name;
    public string unionId;
    public string pid;
    public bool suc;
    public long time;
}
[System.Serializable]
public class PlayerMoneryInfo
{
    public string ipAds;
    public uint money;
    public ulong pid;
    public uint roomCardNum;
    public uint roomCode;
    public string token;

}
[System.Serializable]
public class TextureURLData
{
    public string appId;
    public string hostId;//主Id
    public string title;//标题
    public string text;//文本
    public string url;//纹理图片的链接
    public List<DtUrls> dtUrls;//公告
    public string downloadUrl;//下载地址 二维码图片
    public string sharePicUrl; //分享图片 地址
    public SomeText someText;
    public bool iosSj; // (现在需要通过yuliu1、yuliu2计算得出)是否正在上架App Store，如果是，需要隐藏商城页底部的宣传字样
    public List<DtUrls> iosSjDtUrls;//正在IOS上架的公告
    public string yuliu1;// 当前上架版本号
    public string yuliu2;// 上线是否通过 1:通过	0:审核中
    public string yuliu3;
    public List<string> yuliuList1;
    public List<string> yuliuList2;
    public List<string> yuliuList3;
}
[System.Serializable]
public class SomeText
{
    public string zhaoshang; // 招商
    public string kefu; // 客服
    public string goumai; // 购买
}
[System.Serializable]
public class DtUrls
{
    public string url;// 大厅公告url路径
    public bool canClick;//是否可以点击
}
[System.Serializable]
public class InviterData
{
    public ulong inviterId;//邀请者Id
    public int count;//一级好友邀请数量
    public int count2;//二级好友邀请数量
}
[System.Serializable]
public class DataTest
{
    public int id;
    public string name;
    public string[] tabs;
}

[System.Serializable]
public class FightServerData
{
    public string host;
    public int port;
    public string serverState;
    public string serverVersion;
    public string serverName;
    public string gameName;
    public string appId;
}

[System.Serializable]
public class EmailData  //邮件数据
{

    public string emailId; // 邮件id

    public string sender; // 发件者

    public string title; // 邮件标题

    public string content; // 邮件内容

    public string goldNum; // 奖励数量

    public string rcNum; // 奖励数量

    public string ineffectTime; // 失效时间

    public string enableTime;   // 启用时间

}

[System.Serializable]
public class HorseData
{
    public string horseId;      // 跑马灯id
    public string sender;       // 发起者
    public string enableTime;   // 启用时间
    public string ineffectTime; // 失效时间
    public string sendTime;     // 创建时间
    public int perWaveInteval; // 每波间隔
    public int intervalTime;   // 每个跑马灯间隔秒数
    public int perWaveNumber;  // 每波次数
    public int isEnable;     // 是否启用
    public string content;      // 跑马灯内容
    public string hostId;

}


[System.Serializable]
public class WeChatLoginSucData
{
    public string access_token;
    public int expires_in;
    public string refresh_token;
    public string openid;
    public string scope;
    public string unionid;
}
[System.Serializable]
public class RecordData
{
    public int roomCode;
    public int gameType;
    public int num;
    public int limitCard;
    public string endTime;
    public List<Dictionary<string, string>> info;//玩家的item列表
    public string roomid;
}
[System.Serializable]
public class PlayerData
{
    public string unionId;//联盟id
    public string appId; // appId "HY_NJ_GD"
    public string hostId; // hostId 运营者ID "HY"
    public string channelId; // 发行渠道ID "APPSTORE"
    public string uuid; // 设备唯一标识
    public ulong pid;
    public string loginType; // 登录类型（NORMAL,YK)
    public string token;
    public string openId; // 微信openId
    public string refreshToken; // 微信refreshToken
    public string name;
    public int sex;
    public string headimgurl; // 微信给的地址
    public string headPortrait; // 本地存储的地址
    public int roomCardNum; // 房卡数量
    public int money; // 金币
    public string phone;
    public string password;
    public int level; // 等级
    public string creatTime; // 注册时间
    public string ip;
    public int port;
    public string lastOnlineTime; // 最近登录时间
    public int state; // 0:空闲；1：战斗
    public uint roomCode; // 正在战斗的房间code
    public int accountStatus; // 账号状态 0：正常；1：冻结
    public string unfreezeTime; // 解冻时间 yyyy-MM-dd HH:mm:ss
    public int hideFlag;//0--金币隐藏 1--金币显示
    public string ipAds;//IP具体位置地址
    private int remainCard; // 剩余牌的数量
    public ulong inviterId; // 邀请者ID
    public PlayerDetail playerDetail;
}
[System.Serializable]
public class PlayerDetail
{
    public string realName; // 真实姓名
    public string cardNo; // 身份证号码
    public string addrCode; // 地区编码
    public string birth; // 出生日期
    public int sex; // 性别
    public string addr; // 身份证所在地
    public string province; // 身份证所在省
    public string city; // 身份证所在市
    public string area; // 身份证所在区
}

[System.Serializable]
public class SfzData
{
    public int result;
    public string detail;
}

