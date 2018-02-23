using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace litefeel
{

    public class BFMenuTool
    {
        //[MenuItem("Assets/Bitmap Font/Rebuild Bitmap Font", true)]
        //public static bool CheckRebuildFont()
        //{
        //    TextAsset selected = Selection.activeObject as TextAsset;
        //    if (selected == null) return false;
        //    return BFImporter.IsFnt(AssetDatabase.GetAssetPath(selected));
        //}

        //[MenuItem("Assets/Bitmap Font/Rebuild Bitmap Font")]
        //public static void RebuildFont()
        //{
        //    TextAsset selected = Selection.activeObject as TextAsset;
        //    BFImporter.DoImportBitmapFont(AssetDatabase.GetAssetPath(selected));
        //}
        //选择字体的.fnt【将.txt修改保证唯一性】配置文件
        [MenuItem("Assets/Bitmap Font/BuildSingle BitmapFont")]
        public static void BuildFont()
        {
            TextAsset selected = Selection.activeObject as TextAsset;
            string path = AssetDatabase.GetAssetPath(selected);
            bool isConfig = path.EndsWith(".fnt", StringComparison.OrdinalIgnoreCase);
            isConfig = path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);//忽略大小比较
            if (selected != null && isConfig)
            {
                BFImporter.DoImportBitmapFont(path);
            }
            else
            {
                Debug.LogError("未选择.fnt【.txt修改而来】文件，支持配置文件数据xml 和txt");
            }
        }
        //只能寻找到.fnt文件
        [MenuItem("Assets/Bitmap Font/Rebuild All Bitmap Font")]
        public static void RebuildAllFont()
        {
            string dataPath = Application.dataPath;
            int startPos = dataPath.Length - "Assets".Length;
            //*.fnt 保证了后缀的唯一性
            string[] files = Directory.GetFiles(Application.dataPath, "*.fnt", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i].Substring(startPos);
                if (!BFImporter.IsFnt(path)) continue;
                BFImporter.DoImportBitmapFont(path);
            }
        }
    }

}