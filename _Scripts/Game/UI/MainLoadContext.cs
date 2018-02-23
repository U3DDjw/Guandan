using HaoyunFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLoadContext : MonoBehaviour {

    private void Start()
    {
        UIManagers.Instance.RefreshParentCanvas();
        GameManager.Instance.mIsBackToLoginModule = false;
        ContextManager.Instance.Push(new CommonContext());
        ContextManager.Instance.Push(new HomeContext());
    }
}
