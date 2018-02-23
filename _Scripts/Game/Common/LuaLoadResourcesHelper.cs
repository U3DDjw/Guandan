using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadResourcesHelper
{
    private static LoadResourcesHelper instance;
    public static LoadResourcesHelper Instance
    {
        get
        {
            instance = null ?? new LoadResourcesHelper();
            return instance;
        }
    }
    public Sprite LoadSprite(string path)
    {
        return ResourceManager.Instance.LoadAsset<Sprite>(path);
    }
    public Texture LoadTexture(string path)
    {
        return ResourceManager.Instance.LoadAsset<Texture>(path);
    }
    public AudioClip LoadAudioClip(string path)
    {
        return ResourceManager.Instance.LoadAsset<AudioClip>(path);
    }
    public TextAsset LoadTextAsset(string path)
    {
        return ResourceManager.Instance.LoadAsset<TextAsset>(path);
    }
    public Font LoadFont(string path)
    {
        return ResourceManager.Instance.LoadAsset<Font>(path);
    }
}
