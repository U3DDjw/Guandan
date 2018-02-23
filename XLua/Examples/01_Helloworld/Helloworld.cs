

using UnityEngine;
using XLua;
public class Helloworld : MonoBehaviour {
	// Use this for initialization
	void Start () {
        LuaEnv luaenv = new LuaEnv();
        luaenv.DoString("function s(id) CS.UnityEngine.Debug.Log(id) end CS.UnityEngine.Debug.Log(s(2))");
        luaenv.Dispose();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
