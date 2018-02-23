/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;
using System.IO;

[System.Serializable]
public class Injection
{
    public string name;
    public GameObject value;
}

[LuaCallCSharp]
public class LuaBehaviour : MonoBehaviour {
    public string luaScriptName = "";
 //    TextAsset luaScript;
    public Injection[] injections;

    internal static LuaEnv luaEnv = null;
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;
    private UnityApplicationAction luaOnApplicationPause;
    private UnityApplicationAction luaOnApplicationFocus;

    private LuaTable scriptEnv;
    public LuaTable LuaData
    {
        get
        {
            return scriptEnv;
        }
    }

    void Awake()
    {
        luaEnv = XLuaInstance.Instance.mLuaEnv;
        scriptEnv = luaEnv.NewTable();
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);
        foreach (var injection in injections)
        {
            scriptEnv.Set(injection.name, injection.value);
        }

        if (GameManager.Instance.mIsHotUpdate)
        {
            luaScriptName = "LuaScripts/Views/" + luaScriptName;
            TextAsset tex = ResourceManager.Instance.LoadLuaAB(luaScriptName);
            luaEnv.DoString(tex.text, "LuaBehaviour", scriptEnv);
        }
        else
        {
            string fileText = Resources.Load<TextAsset>("LuaScripts/Views/" + luaScriptName).text;
            luaEnv.DoString(fileText, "LuaBehaviour", scriptEnv);
        }

        Action luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out luaUpdate);
        scriptEnv.Get("ondestroy", out luaOnDestroy);
        scriptEnv.Get("onaplicationpause", out luaOnApplicationPause);
        scriptEnv.Get("luaonApplicationfocus", out luaOnApplicationFocus);

        if (luaAwake != null)
        {
            luaAwake();
        }
    }
   
    // Use this for initialization
    void Start ()
    {
        if (luaStart != null)
        {
            luaStart();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
        if (Time.time - LuaBehaviour.lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            LuaBehaviour.lastGCTime = Time.time;
        }
	}

    private void OnApplicationPause(bool pause)
    {
        if(luaOnApplicationPause!=null)
        {
            luaOnApplicationPause(pause);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (luaOnApplicationFocus != null)
        {
            luaOnApplicationFocus(focus);
        }
    }
   
    void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
        injections = null;
    }
}
