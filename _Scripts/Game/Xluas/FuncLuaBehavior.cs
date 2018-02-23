using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System;

[LuaCallCSharp]
public class FuncLuaBehavior : MonoBehaviour {
    public string luaFile;
    public Injection[] injections;

    internal static LuaEnv luaEnv = XLuaInstance.Instance.mLuaEnv; //all lua behaviour shared one luaenv only!
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 
    LuaTable dataTable; // Lua table，包括C#创建的和Lua中创建的数据

    LuaTable funcTable; // Lua实现的事件响应函数和功能函数

    public LuaTable luaData
    {
        get { return dataTable; }
    }
    private Action<LuaTable> luaStart;
    private Action<LuaTable> luaUpdate;
    private Action<LuaTable> luaOnDestroy;
    private Action<LuaTable,bool> luaOnApplicationPause;
    private Action<LuaTable, bool> luaOnApplicationFocus;

    void Awake()
    {
        //加载Lua文件
        luaEnv.DoString(string.Format("require 'Views.{0}'", luaFile ));
        //定义dataTable
        dataTable = luaEnv.NewTable();
        //插入需要处理的UI组件到dataTable中
        dataTable.Set("gameObject", gameObject);
        foreach (var injection in injections)
        {
            dataTable.Set(injection.name, injection.value);
        }

        //获取lua文件中函数table的引用
        funcTable = luaEnv.Global.Get<LuaTable>(luaFile);
        if (funcTable != null)
        {
            var luaAwake = funcTable.Get<Action<LuaTable>>("Awake");
            if (luaAwake != null)
            {
                luaAwake(dataTable);
            }

            luaStart = funcTable.Get<Action<LuaTable>>("Start");
            luaUpdate = funcTable.Get<Action<LuaTable>>("Update");
            luaOnDestroy = funcTable.Get<Action<LuaTable>>("OnDestroy");
         //   luaOnApplicationPause = funcTable.Get<Action<LuaTable,bool>>("OnApplicationPause");
           // luaOnApplicationFocus = funcTable.Get<Action<LuaTable,bool>>("OnApplicationFocus");
        }
    }

    // Use this for initialization
    void Start () {
        if (luaStart != null)
        {
            luaStart(dataTable);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (luaUpdate != null)
        {
            luaUpdate(dataTable);
        }
        if (Time.time - FuncLuaBehavior.lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            FuncLuaBehavior.lastGCTime = Time.time;
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (luaOnApplicationPause != null)
        {
            luaOnApplicationPause(dataTable,pause);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (luaOnApplicationFocus != null)
        {
            luaOnApplicationFocus(dataTable,focus);
        }
    }

    void OnDestroy()
    {
        dataTable.Dispose();
        if (funcTable != null)
        {
            //luaOnDestroy(dataTable);
            luaStart = null;
            luaUpdate = null;
            luaOnApplicationPause=null;
            luaOnApplicationFocus = null;
            funcTable.Dispose();
        }
    }
}
