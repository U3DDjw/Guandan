using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HaoyunFramework;
public class LoginSceneController : MonoBehaviour
{
    void Start()
    {
        UIManagers.Instance.RefreshParentCanvas();
        var g = Resources.Load<GameObject>(UIType.LoadingView.Path);
        GameObject.Instantiate(g, GameObject.Find("Canvas").transform);
    }
}
