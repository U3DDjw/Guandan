using UnityEngine;
using System.Collections;
using XLua;
using System.IO;
using MsgContainer;

/// <summary>
/// 先判断更新热补丁文件，然后再执行Fixing
/// </summary>
public class XLuaInstance : MonoDontDesSingleton<XLuaInstance>
{
    private LuaEnv luaEnv;
    public LuaEnv mLuaEnv
    {
        get {
            if (luaEnv == null) {
                luaEnv = new LuaEnv();
            }
            return luaEnv;

        }
        set
        {
            luaEnv = value;
        }
    }
    /// <summary>
    /// 热补丁开始修复中。。。
    /// </summary>
    public void HotStartFixing()
    {
        //配置表始终从luaGloablData中读取
        mLuaEnv = new LuaEnv();
        LuaEnv.CustomLoader method = CustomLoaderMethod;
        mLuaEnv.AddLoader(method);
        ReadGlobalData();

        if (!GameManager.Instance.mIsLuaUpdate) { return; } //这句话注释掉可以全部用lua开发
        Debug.Log("Start HotFix...");
        mLuaEnv.DoString("require 'Main'");
    }

    void ReadGlobalData()
    {
        mLuaEnv.DoString("require 'Common.LuaGlobalData'");
        GlobalData.LuaCtrl = mLuaEnv.Global;
    }

    byte[] CustomLoaderMethod(ref string fileName)
    {
        Debug.Log("Lua:Require:"+fileName);
        fileName = "LuaScripts/" + fileName.Replace(".", "/");
        if (GameManager.Instance.mIsHotUpdate&&GameManager.Instance.mIsLuaUpdate) //加载Per 读ab
        {
           var tex = ResourceManager.Instance.LoadLuaAB(fileName);
            return tex.bytes;
        }
        else
        {
            var t = Resources.Load<TextAsset>(fileName);
            return t.bytes;
        }
    }

    void Update()
    {
        if (mLuaEnv != null)
            mLuaEnv.Tick();
    }

    protected override void OnDestroy()
    {

        if (mLuaEnv != null)
        {
            mLuaEnv = null;
            //luaenv.Dispose();
            base.OnDestroy();
        }
   }

    //void OnDestroy()
    //{
    //    //if (luaenv != null)
    //    //    luaenv.Dispose();
    //}
}
