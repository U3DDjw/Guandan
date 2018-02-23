using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HaoyunFramework;

public interface IModule  {

    bool mIsLoadByMiddleSence { get; set; } //模块加载是否需要加载中间场景（显示进度条之类的）
  
    Game mGameController { get; }

   
    void LoadModule(); // 加载模块


    void AddServerCallBack(); // 挂在服务器回调协议
}
