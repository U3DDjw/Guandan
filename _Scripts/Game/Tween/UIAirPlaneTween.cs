using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class UIAirPlaneTween : MonoBehaviour
{

    [SerializeField]
    RawImage airPlaneTex;
    [SerializeField]
    Image smock;
    float flyTime = 1.5f;//存在3秒
    public void SetData(Vector3 startPos, Vector3 endPos, TweenCallback callback)
    {
        Sequence se = DOTween.Sequence();
        transform.localPosition = startPos;
        Vector3 offset = startPos - endPos;//中间设置四个点起伏波动
        Vector3 newPos = offset / 4.0f;

        Vector3[] pathPos = new Vector3[5];
        for (int i = 0; i < 4; i++)
        {
            pathPos[i] = i % 2 == 0 ? startPos - newPos * i + new Vector3(0, 10, 0) : startPos - newPos * i + new Vector3(0, -10, 0);
        }
        pathPos[4] = endPos;
        se.Append(transform.DOLocalPath(pathPos, flyTime, PathType.CatmullRom));
        se.AppendCallback(
            delegate
            {
                Destroy(this.gameObject);
                if (callback != null)
                    callback();

            }
            );
        se.Play();
    }

    float time = 0;
    private void Update()
    {
        time += Time.deltaTime;
        if (time > flyTime / 5.0f)
        {
            time = 0;
            string sprName = smock.sprite.name.Equals("pk_plane_smock1") ? "pk_plane_smock2" : "pk_plane_smock1";
            smock.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, sprName);
        }

    }

}
