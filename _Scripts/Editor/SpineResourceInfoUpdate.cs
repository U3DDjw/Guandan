using System.IO;
using UnityEditor;
using UnityEngine;
//Spine动画改名字用

/// <summary>
/// 1.可以同时多种选择文件夹
/// 2.点击按钮修改名字
/// 3.修改后的名字的前缀的 文件夹名_类型名
/// </summary>
public class SpineResourceInfoUpdate
{
    [MenuItem("Tools/Spine出现问题/Spine动画文件资源名称修改", false, 21)]
    static public void UpdateFileName()
    {
        int indexName = 0;//类型存在多个索引
        Object[] objects = Selection.GetFiltered(typeof(object), SelectionMode.DeepAssets);
        foreach (Object o in objects)
        {
            string directoryName = GetDirectoryName(o);// 获取文件夹的名字
            //获取当前的文件名
            string assetFilePath = AssetDatabase.GetAssetPath(o);
            Debug.Log("文件名：" + assetFilePath);

            //获取当前文件的类型名

            string suffixName = Path.GetExtension(assetFilePath);//后缀名
            if (suffixName == ".png" || suffixName.Length <= 0)//纹理图片直接跳过
            {
                continue;
            }
            string typeName = GetFileTypeName(suffixName, ref indexName);
            string withoutExName = Path.GetDirectoryName(assetFilePath);

            int lastIndex = Application.dataPath.LastIndexOf('/');
            string rootPath = Application.dataPath.Remove(lastIndex + 1);

            string nowPath = rootPath + assetFilePath;
            string restPath = rootPath + withoutExName + '/' + directoryName + typeName + suffixName;
            File.Move(nowPath + ".meta", restPath + ".meta");//先移动meta文件，否则移动文件，unity会自动删除meta文件 就没有了
            File.Move(nowPath, restPath);
        }
        AssetDatabase.Refresh();
        Debug.Log(">>>>>>名字修改完毕");
    }
    [MenuItem("Tools/Spine出现问题/spine动画纹理设置修改统一(ios特供)", false, 22)]
    static void SpineTextureSetting()
    {

        int indexName = 0;//类型存在多个索引
        Object[] objects = Selection.GetFiltered(typeof(object), SelectionMode.DeepAssets);
        foreach (Object o in objects)
        {
            string directoryName = GetDirectoryName(o);// 获取文件夹的名字
            //获取当前的文件名 Asset/....
            string assetFilePath = AssetDatabase.GetAssetPath(o);
            Debug.Log("文件名：" + assetFilePath);

            //获取当前文件的类型名

            string suffixName = Path.GetExtension(assetFilePath);//后缀名
            if (suffixName == ".png" || suffixName.Length <= 0)//纹理图片直接跳过
            {
                //string fullPath = Path.GetFullPath(assetFilePath);
                //WWW www = new WWW(fullPath);
                SetTexureInspector(assetFilePath);
            }
        }
        AssetDatabase.Refresh();
    }
    static void SetTexureInspector(string assetFilePath)
    {
        TextureImporter ti = TextureImporter.GetAtPath(assetFilePath) as TextureImporter;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.alphaSource = TextureImporterAlphaSource.FromInput;
        ti.alphaIsTransparency = true;
        ti.mipmapEnabled = false;
        ti.npotScale = TextureImporterNPOTScale.None;
        ti.textureType = TextureImporterType.GUI;
        ti.filterMode = FilterMode.Point;
        ti.SaveAndReimport();
    }
    static string GetFileTypeName(string suffixName, ref int suffixIndex)
    {
        Debug.Log("扩展名：" + suffixName);
        string typeName = suffixName;
        switch (typeName)
        {
            case ".json":
                typeName = "_json";
                break;
            case ".txt":
                typeName = "_txt";
                break;
            case ".mat":
                typeName = "_mat";
                break;
            case ".asset":
                typeName = "_asset" + (suffixIndex++);
                break;
        }
        return typeName;
        //string rootP = Application.dataPath;
        //File.Move(rootP + , rootP + );

    }
    ///获取文件夹的名字
    static string GetDirectoryName(Object obj)
    {
        string curFilePath = AssetDatabase.GetAssetPath(obj);
        string direcName = Path.GetDirectoryName(curFilePath);
        string curDirectoryName = new DirectoryInfo(direcName).Name;
        Debug.Log("文件夹名=" + curDirectoryName);
        return curDirectoryName;
    }
}
