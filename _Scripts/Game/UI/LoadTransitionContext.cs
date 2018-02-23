using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HaoyunFramework;
public class LoadTransitionContext : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UIManagers.Instance.RefreshParentCanvas();
        ContextManager.Instance.Push(new LoadNextSceneContext());
	}
}
