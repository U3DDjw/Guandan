using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MsgContainer;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using HaoyunFramework;

public class LoadNextSceneContext : BaseContext
{
    public LoadNextSceneContext()
    {
        ViewType = UIType.loadNextSceneView;
    }
}
public class UILoadNextSceneView : BasesView
{
    [SerializeField]
    Slider progressFiller;
    string nextSceneName;

    void RequestUrldata()
    {
        WWWForm form = new WWWForm();
        form.AddField("appId", GlobalData.mAppId);
        form.AddField("hostId", GlobalData.mHostId);
        form.AddField("time", Haoyun.Utils.TimeUtils.ConvertToTime(System.DateTime.Now).ToString());
        //form.AddField("sig", GlobalData.sig);
        //地址更新
        string url = GlobalData.mConstBaseServerUrl + ServerUrlTitle.Url_NoticInfo;
        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
        {
            TextureURLData mUrlData = JsonManager.GetNoticData(www.text);
            PlayerInfo.Instance.mUrlData = mUrlData;

            // 根据上架的版本号来判断是否是上架中，使用来mUrlData中的两个预留字段
            if (mUrlData.yuliu1 == GlobalData.mVersion)
            {
                if (mUrlData.yuliu2 == "0")
                {
                    PlayerInfo.Instance.mUrlData.iosSj = true;
                }
                else
                {
                    PlayerInfo.Instance.mUrlData.iosSj = false;
                }
            }
            else
            {
                PlayerInfo.Instance.mUrlData.iosSj = false;
            }

            for (int i = 0; i < DataManager.Instance.mTextureKeyNameList.Count; i++)
            {
                switch (DataManager.Instance.mTextureKeyNameList[i])
                {
                    case (int)ETextureName.EHeadTex1:
                        url = "";
                        if (PlayerInfo.Instance.mPlayerData.headPortrait.Length == 1)
                        {
                            url = "_LocalResource/texture_head_" + PlayerInfo.Instance.mPlayerData.headPortrait;
                            DataManager.Instance.AddTexture((int)ETextureName.EHeadTex1, Resources.Load<Texture>(url));
                        }
                        else
                        {
                            url = PlayerInfo.Instance.mPlayerData.headPortrait;
                            StartCoroutine(DataManager.Instance.LoadEachTexture(url, delegate (WWW w)
                            {
                                DataManager.Instance.AddTexture((int)ETextureName.EHeadTex1, w.texture);
                            }));
                        }
                        break;

                    case (int)ETextureName.ENotic:
                        StartCoroutine(DataManager.Instance.LoadEachTexture(mUrlData.url, delegate (WWW w)
                        {
                            DataManager.Instance.AddTexture((int)ETextureName.ENotic, w.texture);
                        }));
                        break;
                    case (int)ETextureName.EDatingNotic:
                        LoadDatingTextrueOfUrl(mUrlData);
                        break;
                }

            }
        }));
    }
    void Start()
    {
        nextSceneName = GlobalData.mScene_Main;
        DataManager.Instance.InitData();
        TextureURLData mUrlData = PlayerInfo.Instance.mUrlData;
        if (mUrlData == null)
        {
            RequestUrldata();
        }
        else
        {
            for (int i = 0; i < DataManager.Instance.mTextureKeyNameList.Count; i++)
            {
                switch (DataManager.Instance.mTextureKeyNameList[i])
                {
                    case (int)ETextureName.EHeadTex1:
                        string url = "";
                        if (PlayerInfo.Instance.mPlayerData.headPortrait.Length == 1)
                        {
                            url = "_LocalResource/texture_head_" + PlayerInfo.Instance.mPlayerData.headPortrait;
                            DataManager.Instance.AddTexture((int)ETextureName.EHeadTex1, Resources.Load<Texture>(url));
                        }
                        else
                        {
                            url = PlayerInfo.Instance.mPlayerData.headPortrait;
                            StartCoroutine(DataManager.Instance.LoadEachTexture(url, delegate (WWW www)
                            {
                                DataManager.Instance.AddTexture((int)ETextureName.EHeadTex1, www.texture);
                            }));
                        }
                        break;

                    case (int)ETextureName.ENotic:
                        StartCoroutine(DataManager.Instance.LoadEachTexture(mUrlData.url, delegate (WWW www)
                        {
                            DataManager.Instance.AddTexture((int)ETextureName.ENotic, www.texture);
                        }));
                        break;
                    case (int)ETextureName.EDatingNotic:
                        LoadDatingTextrueOfUrl(mUrlData);
                        break;
                }

            }
        }

        if (LoadBundleController.GetInstance().IsContainesMainBundle())
        {
            progressFiller.value = 0;
            LoadBundleController.GetInstance().LoadMainBundles(
          () =>
          {
              StartCoroutine(LoadNormalScene(nextSceneName, GlobalData.LOADNEXTSCENE_PERCENT_RATE));
              SDKManager.Instance.saveSharePic();
          },
          (idx) =>
          {
              float totalFiller = GlobalData.LOADNEXTSCENE_PERCENT_RATE * idx;
              progressFiller.value = totalFiller / LoadBundleController.GetInstance().TotalMainBundleCount();
          });
        }
        else
        {
            progressFiller.value = GlobalData.LOADNEXTSCENE_PERCENT_RATE;
            SDKManager.Instance.saveSharePic();
            StartCoroutine(LoadNormalScene(nextSceneName, GlobalData.LOADNEXTSCENE_PERCENT_RATE));
        }


    }

    void LoadDatingTextrueOfUrl(TextureURLData mUrlData, int index = 0)
    {
        if (GameManager.Instance.mGameMode == EGameMode.EAppleOnLine && mUrlData.iosSj)
        {
            //大厅公告
            if (mUrlData.iosSjDtUrls != null && mUrlData.iosSjDtUrls.Count != 0)
            {
                if (index < mUrlData.iosSjDtUrls.Count)
                {
                    StartCoroutine(DataManager.Instance.LoadDaTingTexture(mUrlData.iosSjDtUrls[index].url, delegate (Texture texture)
                    {
                        DataManager.Instance.IsCanClicks.Add(mUrlData.iosSjDtUrls[index].canClick);
                        index++;
                        DataManager.Instance.mDaTingNoticTextureList.Add(texture);
                        LoadDatingTextrueOfUrl(mUrlData, index);
                    }));
                }

            }
        }
        else
        {
            //大厅公告
            if (mUrlData.dtUrls != null && mUrlData.dtUrls.Count != 0)
            {
                if (index < mUrlData.dtUrls.Count)
                {
                    StartCoroutine(DataManager.Instance.LoadDaTingTexture(mUrlData.dtUrls[index].url, delegate (Texture texture)
                    {
                        DataManager.Instance.IsCanClicks.Add(mUrlData.dtUrls[index].canClick);
                        index++;
                        DataManager.Instance.mDaTingNoticTextureList.Add(texture);
                        LoadDatingTextrueOfUrl(mUrlData, index);
                    }));
                }

            }
        }
    }

    IEnumerator LoadNormalScene(string sceneName, float startPercent)
    {
        int startProgress = (int)(startPercent * 100);
        int toProgress = startProgress;
        int displayProgress = startProgress;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName); //Application.LoadLevelAsync
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            toProgress = startProgress + (int)(op.progress * (1.0f - startPercent) * 100);
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                SetProgress(displayProgress);
            }
            yield return null;
        }
        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            SetProgress(displayProgress);
            yield return null;
        }
        op.allowSceneActivation = true;

    }

   
    string GetScenePath(string fileName)
    {
        string path = DataUrl.GetFilePersistentUrl(fileName) + ".unity3d";
        // string path= DataUrl.LOCAL_URL_PREFIX + Application.dataPath + "/StreamingAssets/" + "Scene_Main" + ".unity3d";
        //读取Per目录的时候不需要加prefix,但是读取Streaming目录时候需加上prefix
        bool isPersistentDataPath = System.IO.File.Exists(path);
        if (!isPersistentDataPath)
        {
            path = DataUrl.GetFileStreamingUrl(fileName) + ".unity3d";
            return path;
        }

        return DataUrl.LOCAL_URL_PREFIX + path;
    }
    void SetProgress(int progress)
    {
        progressFiller.value = progress * 0.01f;
    }

    private void OnDestroy()
    {
        Debug.Log("结束Transition场景");
        StopAllCoroutines();
    }
}
