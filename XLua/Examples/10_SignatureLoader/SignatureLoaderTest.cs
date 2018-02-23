using UnityEngine;
using System.Collections;
using XLua;
using System.IO;

public class SignatureLoaderTest : MonoBehaviour {
    public static string PUBLIC_KEY = "BgIAAACkAABSU0ExAAQAAAEAAQBdRLA5vNtTL8cwRbZDgJkl0B0Wh0Nooeo99hqSvfN+RAnlMpHnWeRO2dDuzl8NTPkIQzA9RUaW6oXzUf/FixBvTFYDLRp467teK9ZaKwi/8BjNE6t5Or0UP8OG3BHvOzLOsXWXmNkT9Ku4n3OZENXJQhT4fvatsnbtt9hl20DhsQ==";

    // Use this for initialization
    void Start () {
        LuaEnv luaenv = new LuaEnv();
#if UNITY_EDITOR
        luaenv.AddLoader(new SignatureLoader(PUBLIC_KEY, (ref string filepath) =>
        {
            filepath = Application.dataPath + "/XLua/Examples/10_SignatureLoader/" + filepath.Replace('.', '/') + ".lua";
            if (File.Exists(filepath))
            {
                return File.ReadAllBytes(filepath);
            }
            else
            {
                return null;
            }
        }));
#else //为了让手机也能测试
        luaenv.AddLoader(new SignatureLoader(PUBLIC_KEY, (ref string filepath) =>
        {
            filepath = filepath.Replace('.', '/') + ".lua";
            TextAsset file = (TextAsset)Resources.Load(filepath);
            if (file != null)
            {
                return file.bytes;
            }
            else
            {
                return null;
            }
        }));
#endif
        luaenv.DoString(@"
            require 'signatured1'
            require 'signatured2'
        ");
        luaenv.Dispose();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
