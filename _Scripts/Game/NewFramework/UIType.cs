using MsgContainer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HaoyunFramework
{
    public class UIType
    {

        public string Path { get; private set; }
        public string Name { get; private set; }
        public UIType(string path)
        {
            Path = path;
            Name = path.Substring(path.LastIndexOf('/') + 1);
        }

        public override string ToString()
        {
            return string.Format("path:{0} name :{1}", Path, Name);
        }

        static string titlePath = "View/";
        public static readonly UIType LoadingView = new UIType("_LocalResource/" + "LoadingView"); //LoadingView 直接附加到LoginScene场景，不参与热更
        //public static readonly UIType MainMenu = new UIType("View/MainMenuView");
        //public static readonly UIType newTest = new UIType("View/NewTest");
        //主界面
        public static readonly UIType SettingView = new UIType(titlePath + "SettingView");
        public static readonly UIType RoomcardView = new UIType(titlePath + "RoomcardView");
        public static readonly UIType JoinRoomView = new UIType(titlePath + "JoinRoomView");
        public static readonly UIType CreateRoomView = new UIType(titlePath + "CreateRoomView");
        public static readonly UIType ShopView = new UIType(titlePath + "ShopView");
        public static readonly UIType ShopItem = new UIType(titlePath + "ShopItem");
        public static readonly UIType EditPlayerInfoView = new UIType(titlePath + "EditPlayerInfoView");
        public static readonly UIType EmailView = new UIType(titlePath + "EmailView");
        public static readonly UIType EmailItem = new UIType(titlePath + "EmailItem");
        public static readonly UIType EmailTween = new UIType(titlePath + "EmailTween");
        public static readonly UIType RecordView = new UIType(titlePath + "RecordView");
        public static readonly UIType RecordItem = new UIType(titlePath + "RecordItem");
        public static readonly UIType RecordItemDetail = new UIType(titlePath + "RecordItemDetail");
        public static readonly UIType RecordBureauView = new UIType(titlePath + "RecordBureauView");
        public static readonly UIType RecordLookOtherBackView = new UIType(titlePath + "RecordLookOtherBackView");
        public static readonly UIType RuleView = new UIType(titlePath + "RuleView");
        public static readonly UIType NoticeView = new UIType(titlePath + "NoticeView");
        public static readonly UIType BuyCoinView = new UIType(titlePath + "BuyCoinView");
        public static readonly UIType EmoticonView = new UIType(titlePath + "EmoticonView");
        public static readonly UIType BindPhoneView = new UIType(titlePath + "BindPhoneView");
        public static readonly UIType InviteView = new UIType(titlePath + "InviteView");
        public static readonly UIType GoldFiledView = new UIType(titlePath + "GoldFiledView");

        public static readonly UIType TaskView = new UIType(titlePath + "TaskView");
        public static readonly UIType HomeViewDatingNotic = new UIType(titlePath + "HomeViewDatingNotic");
        public static readonly UIType HasOpenRoomView = new UIType(titlePath + "HasOpenRoomView");
        public static readonly UIType CreateRoomDetailItem = new UIType(titlePath + "CreateRoomDetailItem");
        public static readonly UIType PlayBackView = new UIType(titlePath + "PlayBackView");
        //战斗界面
        public static readonly UIType SameIpView = new UIType(titlePath + "SameIpView");
        public static readonly UIType SameIpItem = new UIType(titlePath + "SameIpItem");
        public static readonly UIType VoteDismissView = new UIType(titlePath + "VoteDismissView");
        public static readonly UIType VoteDismissItem = new UIType(titlePath + "VoteDismissItem");
        public static readonly UIType TotalBureauOverView = new UIType(titlePath + "TotalBureauOverView");
        public static readonly UIType TotalBureauOverItem = new UIType(titlePath + "TotalBureauOverItem");
        public static readonly UIType BureauOverView = new UIType(titlePath + "BureauOverView");
        public static readonly UIType BureauOverItem = new UIType(titlePath + "BureauOverItem");
        public static readonly UIType VoiceView = new UIType(titlePath + "VoiceView");
        public static readonly UIType ChatMessageTip = new UIType(titlePath + "ChatMessageTip");
        public static readonly UIType TipsView = new UIType(titlePath + "TipsView");
        public static readonly UIType PropView = new UIType(titlePath + "PropView");
        public static readonly UIType PropItem = new UIType(titlePath + "PropItem");
        public static readonly UIType CardTypeTips = new UIType(titlePath + "CardTypeTips");
        //其他界面
        public static readonly UIType LoginView = new UIType(titlePath + "LoginView");
        public static readonly UIType CommonView = new UIType(titlePath + "CommonView");
        public static readonly UIType loadNextSceneView = new UIType(titlePath + "LoadNextSceneView");
        public static readonly UIType HomeView = new UIType(titlePath + "HomeView");
        public static readonly UIType WaitGameView = new UIType(titlePath + "WaitGameView");
        public static readonly UIType GameInfoView = new UIType(titlePath + "GameInfoView");
        public static readonly UIType PlayingGameView = new UIType(titlePath + "PlayingGameView");
        public static readonly UIType CheckCardIdView = new UIType(titlePath + "CheckCardIdView");

        //==========================金币场 战斗View =============================================
        public static readonly UIType GoldPlayingGameView = new UIType(titlePath + "GoldPlayingGameView");

        public static readonly UIType GoldPlayingStatusView = new UIType(titlePath + "GoldPlayingStatusView");
        public static readonly UIType GoldPlayingSelfView = new UIType(titlePath + "GoldPlayingSelfView");
        public static readonly UIType GoldPlayingInfoView = new UIType(titlePath + "GoldPlayingInfoView");
        public static readonly UIType GoldPlayingPlayerView = new UIType(titlePath + "GoldPlayingPlayerView");
        public static readonly UIType GoldNotEnoughView = new UIType(titlePath + "GoldNotEnoughView");
        //======================================================================================


        //预制体
        public static readonly UIType SwitchBtnFunctionView = new UIType(titlePath + "SwitchBtn");
        public static readonly UIType SingleCard = new UIType(titlePath + "SingleCard");
        public static readonly UIType GuideArrowPrefab = new UIType(titlePath + "GuideArrowPrefab");
        public static readonly UIType MingPai = new UIType(GlobalData.mLocalViewAnimationPath + "AnimMingpai");
        public static readonly UIType DragDropArrow = new UIType(titlePath + "DragDropArrow");
        public static readonly UIType homeViewTaskBtn = new UIType(titlePath + "HomeViewTaskBtn");
        public static readonly UIType UniWebView = new UIType("_LocalResource/Tools/" + "UniWebViewObject");
    }

}