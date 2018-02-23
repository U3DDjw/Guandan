using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class ResourceLoader : IResourceLoader {

    public override T LoadAsset<T>(string fileName)
    {
        string path = fileName;
        return Resources.Load<T>(path);
    }

    public override T LoadAssetAsync<T>(string fileName)
    {
        string path = fileName;
        ResourceRequest quest = Resources.LoadAsync<T>(path);
        if(quest.isDone)
        {
            return (T)quest.asset;
        }
        return null;
    }

    public override string LoadText(string fileName)
    {
        throw new NotImplementedException();
    }

}
