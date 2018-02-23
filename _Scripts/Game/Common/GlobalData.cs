using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using DNotificationCenterManager;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;
using HaoyunFramework;
using System.Text;
using XLua;

namespace MsgContainer
{

    public class GlobalData : MonoBehaviour
    {
        //////////////////////////////新增GlobalData//////////////////////////////////////////////
        //public static string mTaskActivityHtmlUrl = "https://dlcs.3399hy.com/#!/wxGPay?gameId=1.";
        //////////////////////////////////////////////////////////////////////////////////////////
        private static LuaTable luaCtrl = null;
        public static LuaTable LuaCtrl
        {
            get { return luaCtrl; }
            set
            {
                luaCtrl = value;
                if (iLuaGlobalData == null)
                    iLuaGlobalData = luaCtrl.Get<ILuaGlobalData>("LuaGlobalData");
            }
        }
        public static LuaFunction luaFun = null;
        static ILuaGlobalData iLuaGlobalData = null;
        //任务链接
        public static string mTaskActivityHtmlUrl
        {
            get
            {
                if (iLuaGlobalData.mTaskActivityHtmlUrl == null)
                {
                    return "https://open.3399hy.com/gametask/#!/playerTask/{0}/{1}";
                }
                return iLuaGlobalData.mTaskActivityHtmlUrl;
            }
        }
        ////////////////////////////////-游戏参数////////////////////////////////////////////////
        public static readonly string mGamePackageName = "com.haoyun.nj.guandan";

        public static string mResVersion = "1.3";

        public static string mVersion
        {
            get
            {
                return iLuaGlobalData.mVersion;
            }
        }


        public static string mGameName // 分享出去的游戏名称
        {
            get
            {
                return iLuaGlobalData.mGameName;
            }
        }
        public static string WeChatShareUrl// 分享出去的下载链接
        {
            get
            {
                return iLuaGlobalData.WeChatShareUrl;
            }
        }
        public static string mHostId // 运营商ID
        {
            get
            {
                return mServerResChanelName;
            }
        }
        public static string mHostFkId // 友盟中，房卡消耗记录（商品id）
        {
            get
            {
                return iLuaGlobalData.mHostFkId;
            }
        }

#if UNITY_IPHONE
     
        public static  string umengAppkey="598085d1f5ade41f8f001c6a";  // 友盟的appkey//IOS
#elif UNITY_ANDROID
        public static string umengAppkey = "5980c755734be443ec000c3a";// 友盟的appkey//ANDROID
#else
        public static readonly string umengAppkey = "598085d1f5ade41f8f001c6a";
#endif
        public static uint yayaAppid = 1002074;
        public static readonly string WXAPPID = "wxe7f103cba8edf02a";
        public static readonly string SECRET = "e76c473af83f2e77aa5d9d389f6744ae";
        // for online fix ===================

        public static string applePayOrderId = "";

        public static uint mCardLevelId //专门为参谋牌的id值
        {
            get
            {
                return iLuaGlobalData.mCardLevelId;
            }
        }
        public static string mOnLineUrl
        {
            get
            {
                return "http://hygg.3399hy.com:7770/";
            }
        }
        public static string mConstBaseServerUrl
        {
            get
            {
                switch (GameManager.Instance.mGameUrlType)
                {
                    case EGameServerUrl.ESCZ:
                        return "http://192.168.0.110:7970/";
                    case EGameServerUrl.EFK:
                        return "http://192.168.1.157:7970/";
                    case EGameServerUrl.EFormalTest:
                        return "http://111.231.103.64:7970/";
                    case EGameServerUrl.EOnLine_TEST:
                        return "http://111.231.103.64:7971/";
                    case EGameServerUrl.EOnLine:
                        return mOnLineUrl;
                    default:
                        return "";
                }

            }
        }

        #region DownLoad Res Data

        static string mServerResChanelName
        {
            get
            {
                switch (GameManager.Instance.mGameChanel)
                {
                    case EGameChanel.EDLJ:
                        return "DLJ";
                    case EGameChanel.EHY:
                        return "HY";
                    case EGameChanel.Ehytf:
                        return "hytf";
                    case EGameChanel.Ehyycgd:
                        return "hyycgd";
                }
                return "";
            }
        }
        static string VersionResTitle(string version)
        {
            return "HY_NJ_GD_" + mResVersion + "/";
        }





        static string mServerResIpTitle
        {
            get
            {
                if (GameManager.Instance.mIsFormalResServer)
                {

                    return "http://122.152.211.245:12480/game_resource/" + VersionResTitle(mResVersion);
                }
                else
                {
                    return "http://111.231.103.64:12480/game_resource/HY_NJ_GD/";
                }
            }
        }
        //资源服务器地址(Bundle资源类型ios/Android区分)
        public static readonly string mServerResIp =
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
            mServerResIpTitle + mServerResChanelName + "/android/";
#elif UNITY_IPHONE
          mServerResIpTitle+ mServerResChanelName+"/ios/";
#endif

        #endregion

        #region  ======================Audio Setting=====================
        public static string AudioNameWarning
        {
            get
            {
                return luaCtrl.Get<string>("AudioNameWarning");
            }
        }
        public static string AudioNameStartGame
        {
            get
            {
                return luaCtrl.Get<string>("AudioNameStartGame");
            }
        }
        public static string AudioNameOtherLeaveRoom
        {
            get
            {
                return luaCtrl.Get<string>("AudioNameOtherLeaveRoom");
            }
        }
        public static string AudioNameClickBtn
        {
            get
            {
                return luaCtrl.Get<string>("AudioNameClickBtn");
            }
        }

        public static string AudioNameDaojishi
        {
            get
            {
                return luaCtrl.Get<string>("AudioNameDaojishi");
            }
        }
        public static string AudioNameJoinRoomPlayer
        {
            get
            {
                return luaCtrl.Get<string>("AudioNameJoinRoomPlayer");
            }
        }
        public static string AudioNameSortCard
        {
            get
            {
                return luaCtrl.Get<string>("AudioNameSortCard");
            }
        }

        public static string AudioNameGameOverWin
        {
            get
            {
                return luaCtrl.Get<string>("AudioNameGameOverWin");
            }
        }
        public static string AudioNameGameOverLose
        {
            get
            {
                return luaCtrl.Get<string>("AudioNameGameOverLose");
            }
        }
        public static string AudioNameGameOverPingju
        {
            get
            {
                return luaCtrl.Get<string>("AudioNameGameOverPingju");
            }
        }
        public static string AudioNameTotalGameOverWin
        {
            get
            {
                return luaCtrl.Get<string>("AudioNameTotalGameOverWin");
            }
        }
        public static string AudioNameTotalGameOverLose
        {
            get
            {
                return luaCtrl.Get<string>("AudioNameTotalGameOverLose");
            }
        }
        #endregion

        #region ======================Time Setting=====================
        #region 第一部分
        public static float mGameBureauOverWaitTime//小结算 ，小结算页面自动点击 等候时间
        {
            get
            {
                return iLuaGlobalData.mGameBureauOverWaitTime;
            }
        }
        public static float mLoadGameBreauOverWaitTime//最后出完牌等候小结算页面
        {
            get
            {
                return iLuaGlobalData.mLoadGameBreauOverWaitTime;
            }
        }
        public static float mSameIpTotalTime//SameIp总时间
        {
            get
            {
                return iLuaGlobalData.mSameIpTotalTime;
            }
        }
        public static float mStartGameWaitTime
        {
            get
            {
                return iLuaGlobalData.mGameBureauOverWaitTime;
            }
        }
        public static float mReadyGameWaitTime
        {
            get
            {
                return iLuaGlobalData.mGameBureauOverWaitTime;
            }
        }

        public static float mAutoPlayGameWaitTime
        {
            get
            {
                return iLuaGlobalData.mGameBureauOverWaitTime;
            }
        }

        public static string mStreamingAssetsXml
        {
            get
            {
                return iLuaGlobalData.mStreamingAssetsXml;
            }
        }
        public static int mPlayerOwnCardCount //54 * 2 / 4; 两幅牌，每个人初始化的牌牌数
        {
            get
            {
                return iLuaGlobalData.mPlayerOwnCardCount;
            }
        }
        public static int mCardTypeCount //卡牌类型（4中花色）
        {
            get
            {
                return iLuaGlobalData.mCardTypeCount;
            }
        }
        public static uint mCardNumber
        {
            get
            {
                return iLuaGlobalData.mCardNumber;
            }
        }
        public static uint mRefuseCardNumId//不出 ，28专门为不出的idx
        {
            get
            {
                return iLuaGlobalData.mRefuseCardNumId;
            }
        }
        public static float mRequestSpacingTime//出牌无回调，自动刷新
        {
            get
            {
                return iLuaGlobalData.mRequestSpacingTime;
            }
        }

        public static float mDismissRoomCDTime//解散房间cd时间
        {
            get
            {
                return iLuaGlobalData.mDismissRoomCDTime;
            }
        }
        public static float mDismissRoomAutoResultTime//自动选择解散时间
        {
            get
            {
                return iLuaGlobalData.mDismissRoomAutoResultTime;
            }
        }

        public static float mDatingNoticContentCellSizeX //大厅公告单个宽度
        {
            get
            {
                return iLuaGlobalData.mDatingNoticContentCellSizeX;
            }
        }
        public static readonly string mAppId = "HY_NJ_GD";


        public static readonly string mUIRootName = "Canvas";
        public static readonly string mGameControllerName = "GameController";

        public static readonly string mScene_Login = "Scene_Login";

        public static readonly string mScene_Main = "Scene_Main";
        public static readonly string mScene_Playing = "Scene_Playing";

        public static readonly string mScene_Transition = "Scene_Transition";

        public static string mLoadItemTitlePath
        {
            get
            {
                if (luaCtrl != null)
                {
                    return iLuaGlobalData.mLoadItemTitlePath;
                }
                return "View/";
            }
        }
        #endregion
        #region ==============  战斗场景 卡牌大小比例设置
        public static uint mEachCardsCount = 72; //4*18

        public static float mSingleCardWidth//单张牌的宽度
        {
            get
            {
                return iLuaGlobalData.mSingleCardWidth;
            }
        }
        public static float mCardContainerWidth //横向最大宽
        {
            get
            {
                return iLuaGlobalData.mCardContainerWidth;
            }
        }
        public static float mSingelCardRateY//调节牌的高度间隔
        {
            get
            {
                return iLuaGlobalData.mSingelCardRateY;
            }
        }
        public static float mLastSingelCardRateY //调节剩余牌的高度间隔
        {
            get
            {
                return iLuaGlobalData.mLastSingelCardRateY;
            }
        }
        public static float mLastSingelCardWidth //调节剩余牌的高度间隔(与Small牌保持一致 / 4)
        {
            get
            {
                return iLuaGlobalData.mLastSingelCardWidth;
            }
        }
        public static float mPutOutCardRate  //打出的牌的与原尺寸大小
        {
            get
            {
                return iLuaGlobalData.mPutOutCardRate;
            }
        }
        public static float mCardTypeTipRate //牌型提示中牌是原尺寸的大小
        {
            get
            {
                return iLuaGlobalData.mCardTypeTipRate;
            }
        }
        public static float mCardTouchMoveY  //点击牌震动幅度
        {
            get
            {
                return iLuaGlobalData.mCardTouchMoveY;
            }
        }
        public static float mCardTouchTime //点击震动时间
        {
            get
            {
                return iLuaGlobalData.mCardTouchTime;
            }
        }
        public static float mDropDragHeight//拖拽状态中牌的高度值
        {
            get
            {
                return iLuaGlobalData.mDropDragHeight;
            }
        }
        public static float mDragDrogTouchRate //拖动牌的灵敏度
        {
            get
            {
                return iLuaGlobalData.mDragDrogTouchRate;
            }
        }
        //===============
        public static string mLocalTexturePath //本地纹理的资源路劲
        {
            get
            {
                return iLuaGlobalData.mLocalTexturePath;
            }
        }
        public static string mLocalEmoticonPrefabPath//本地表情动画的预支体路径
        {
            get
            {
                return iLuaGlobalData.mLocalEmoticonPrefabPath;
            }
        }
        public static string mLocalPropPrefabPath //本地道具动画的预支体路径
        {
            get
            {
                return iLuaGlobalData.mLocalPropPrefabPath;
            }
        }
        public static string mLocalViewAnimationPath //界面的动画的路径
        {
            get
            {
                if (iLuaGlobalData != null)
                {
                    return iLuaGlobalData.mLocalViewAnimationPath;
                }
                return "Animation/ViewAnimation/";
            }
        }
        //动画层级名
        public static string mAnimSortingLayer//动画的层级
        {
            get
            {
                if (iLuaGlobalData != null)
                {
                    return iLuaGlobalData.mAnimSortingLayer;
                }
                return "Default";
            }

        }
        public static int mAnimOrderIdLayer// 层级排序 id
        {
            get
            {
                if (iLuaGlobalData != null)
                {
                    return iLuaGlobalData.mAnimOrderIdLayer;
                }
                return 1;
            }
        }
        //Audio Set
        public static string mAudioBuyao
        {
            get
            {
                return iLuaGlobalData.mAudioBuyao;
            }
        }
        public static string mAudioSingleTitle
        {
            get
            {
                return iLuaGlobalData.mAudioSingleTitle;
            }
        }
        public static string mAudioDoublleTitle
        {
            get
            {
                return iLuaGlobalData.mAudioDoublleTitle;
            }
        }
        #endregion
        #endregion
        public static Vector3 mSelectCardToTargetPos
        {
            get
            {
                return iLuaGlobalData.mSelectCardToTargetPos;
            }
        }
        public static Color mSelectCardColor
        {
            get
            {
                return iLuaGlobalData.mSelectCardColor;

            }
        }

        #region ===================从lua中加载方法===============
        /// <summary>
        /// 根据id获取到卡片的spriteName
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetCardName(uint id)
        {
            return iLuaGlobalData.getCardName(id);
        }
        public static string GetHeadCleanCpr(uint rank)
        {
            return iLuaGlobalData.getHeadCleanCpr(rank);
        }
        public static string GetAudioBasePathByStyle(EAudioStyle style)
        {
            return iLuaGlobalData.getAudioBasePathByStyle(style);
        }
        public static string[] mRenderTexturePaths()
        {
            return iLuaGlobalData.getRenderTexturePaths();
        }

        public static ArgsDatingNotic[] mDatingNoticArgs()
        {
            if (PlayerInfo.Instance.mUrlData.iosSj && GameManager.Instance.mGameMode == EGameMode.EAppleOnLine)
            {
                ArgsDatingNotic[] datingNotics = iLuaGlobalData.getDatingIosNoticArgs();
                return datingNotics;
            }
            else
            {
                ArgsDatingNotic[] datingNotics = iLuaGlobalData.getDatingNoticArgs();
                return datingNotics;
            }
        }
        public static List<List<int>> GetGoldFiledData
        {
            get
            {
                return iLuaGlobalData.goldFiledData();
            }
        }
        //常用语
        public static string[] mGetUsefulExpressions()
        {
            return iLuaGlobalData.getUsefulExpressions();
        }
        //    public static ArgsDatingNotic[] mDatingNoticArgs = new ArgsDatingNotic[]
        //{
        //    new ArgsDatingNotic() { path="TexturePic/dating_notice",isCanClick=false},
        //    new ArgsDatingNotic() { path = "TexturePic/dating_notice2",isCanClick = true},
        // };
        //public static string[] mRenderTexturePaths = new string[]
        //  {
        //        "TexturePic/RT_UIDown",
        //        "TexturePic/RT_UIUp",
        //        "TexturePic/RT_UIWaitView",
        //  };
        #endregion
        public static IEnumerator GetHeadTextureByIdx(RawImage _texture, string url)
        {
            if (url.Length > 1)
            {
                WWW www = new WWW(url);
                yield return www;
                Debug.Log("GetHeadTextureByIdx:" + www.error);
                if (www.error == null || www.error == "")
                {
                    _texture.texture = www.texture as Texture;
                    if (url == PlayerInfo.Instance.mPlayerData.headPortrait)
                    {
                        DataManager.Instance.AddTexture((int)ETextureName.EHeadTex1, www.texture);
                    }
                }
                else
                {
                    GetHeadTextureByIdx(_texture, url);
                }
                www.Dispose();
            }
            else
            {
                _texture.texture = ResourceManager.Instance.LoadAsset<Texture>("_LocalResource/texture_head_" + url);
            }
        }

        public static Sprite GetGameCleanCpr(int rank)
        {
            return ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, GetHeadCleanCpr((uint)rank));
        }
        public static IEnumerator SendPost(string url, WWWForm form, HandleFinishDownload func)
        {
            print("发送的请求的url：" + url);
            WWW www;
            if (form == null)

            {
                www = new WWW(url);
            }
            else
            {
                www = new WWW(url, form);
            }
            yield return www;
            if (www.error == null || www.error == "")
            {
                if (func != null)
                {
                    func(www);
                }
            }
            else
            {
                UIManagers.Instance.EnqueueTip(www.error);

                SendPost(url, form, func);
                HandleError(www.text);
            }
            www.Dispose();
        }
        public static void HandleError(string realError)
        {
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EHideLogining);
            if (ServerErrorKey.ERROR_ERR == realError)
            {
            }
            else if (ServerErrorKey.ERROR_NO_USER == realError)
            {
                NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EResetLogin);
                return;
            }
            else if (ServerErrorKey.ERROR_FRONZEN_USER == realError)
            {
                UIManagers.Instance.EnqueueTip("账号被冻结");
                return;
            }
            else if (ServerErrorKey.ERROR_PASSWORD == realError)
            {
                UIManagers.Instance.EnqueueTip("密码错误");
                return;
            }
            else if (ServerErrorKey.ERROR_PASSWORD == realError || realError == "" || realError == null)
            {
                //UIManager.Instance.EnqueueTip("服务器维护中");
                return;
            }
            //   UIManager.Instance.EnqueueTip(realError);


        }

#if UNITY_IPHONE
            /* Interface to native implementation */  
            [DllImport ("__Internal")]  
            private static extern void _copyTextToClipboard(string text);  
#endif
        /// <summary>
        /// 复制功能
        /// </summary>
        /// <param name="lab"></param>
        public static void CopyTextFromLab(string text)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            TextEditor te = new TextEditor();
            te.text = text;
            te.OnFocus();
            te.Copy();
            Debug.Log("复制内容：" + text);
#elif UNITY_ANDROID
        Android(text);  
#elif UNITY_IPHONE
            _copyTextToClipboard(text);  
#endif
            if (text != "") {
                UIManagers.Instance.EnqueueTip("复制成功!");
            }
        }

        static AndroidJavaObject currentActivity;
        static AndroidJavaClass UnityPlayer;
        static AndroidJavaObject context;
        static string copyText = null;
        static void Android(string text)
        {
            try
            {
                copyText = text;
                UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(ToClipBoardOnUiThread));
            }
            catch (System.Exception e)
            {
                UIManagers.Instance.EnqueueTip("复制失败！");
            }
        }
        static void ToClipBoardOnUiThread()
        {
            AndroidJavaClass Context = new AndroidJavaClass("android.content.Context");
            AndroidJavaObject CLIPBOARD_SERVICE = Context.GetStatic<AndroidJavaObject>("CLIPBOARD_SERVICE");
            AndroidJavaObject clipboardMgr = currentActivity.Call<AndroidJavaObject>("getSystemService", CLIPBOARD_SERVICE);
            AndroidJavaClass ClipData = new AndroidJavaClass("android.content.ClipData");
            AndroidJavaObject clipData = ClipData.CallStatic<AndroidJavaObject>("newPlainText", "deltaBit", copyText);
            clipboardMgr.Call("setPrimaryClip", clipData);
            // currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(ShowToast));
        }
        static void ShowToast()
        {
            AndroidJavaClass totast = new AndroidJavaClass("android.widget.Toast");
            AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", copyText);
            AndroidJavaObject toast2 = totast.CallStatic<AndroidJavaObject>("makeText", context, javaString, totast.GetStatic<int>("LENGTH_SHORT"));
            toast2.Call("show");
        }


        /// <summary>
        /// 数字变换为A J Q K
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string StringUpgradeIndex(string index)
        {
            string nextTo = "";
            switch (index)
            {
                case "14":
                    nextTo = "A";
                    break;
                case "11":
                    nextTo = "J";
                    break;
                case "12":
                    nextTo = "Q";
                    break;
                case "13":
                    nextTo = "K";
                    break;
                default:
                    if (index.Equals("0"))
                    {
                        index = "2";
                    }
                    nextTo = index.ToString();
                    break;
            }
            return nextTo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="minValue">最小值</param>
        /// <returns></returns>
        public static List<string> EnumToListInfo(Type enumType, int minValue)
        {
            List<string> list = new List<string>();
            foreach (int myCode in Enum.GetValues(enumType))
            {
                string strName = Enum.GetName(enumType, myCode);//获取名称
                string strVaule = myCode.ToString();//获取值
                if (int.Parse(strVaule) != minValue)
                    list.Add(strVaule);//添加到DropDownList控件
            }
            return list;
        }
        public static string FormatServerTime(string serverTime)//20171219161050
        {
            return DateTime.ParseExact(serverTime, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 返回实际高和宽
        /// </summary>
        /// <param name="defaultWandH"></param>
        /// <returns></returns>
        public static Vector2 GetRealWAndH(Vector2 defalutResolution, Vector2 defaultWandH)
        {
            print("ScreenSize" + new Vector2(Screen.width, Screen.height));
            float curW = 0;
            float curH = 0;

            curW = Screen.width / defalutResolution.x * defaultWandH.x;
            curH = Screen.height / defalutResolution.y * defaultWandH.y;
#if UNITY_IPHONE
                curW = curW/2;
                curH = curH/2;
#endif
            return new Vector2(curW, curH);
        }
        //=================================== 热更======================================
        public static readonly string ASSET_BASE_FORMAT = "Assets/Game/Resources/{0}";
        public static readonly string ASSETBUNDLE_AFFIX = ".ab";
        public static readonly string Path_DataBase = "_LocalResource/DataBase/";
        public static readonly string HOT_FIX_FILE_NAME = "Hotfix.lua";
        public static readonly string HOT_GLOBALDATA_NAME = "LuaGlobalData.lua";

        /// <summary>
        /// 加载MainBundle所占进度条比例
        /// </summary>
        public static readonly float LOADNEXTSCENE_PERCENT_RATE = 0.8f;

        //==============================================================================
    }
}
