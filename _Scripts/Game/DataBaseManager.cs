using UnityEngine;
using System.Collections;
using Common;
using System.Collections.Generic;
public class DataBaseManager : SingleTon<DataBaseManager>
{
    /// <summary>
    /// 虚拟本地玩家(转为版属服)
    /// </summary>
    private Dictionary<string, string> userDic;
    public Dictionary<string,string> mUserDic
    {
        get
        {
            if(userDic==null)
            {
                userDic = new Dictionary<string, string>();
                return userDic;
            }
            return userDic;
        }

        set
        {
            userDic = value;
        }
    }

    public void AddUser(string userName,string passWord)
    {
        if(userDic.ContainsKey(userName))
        {
            Debug.Log("该玩家已经有");
        }
        userDic.Add(userName, passWord);
    }

    public void ModifyPassWord(string userName,string password)
    {
        if(userDic.ContainsKey(userName))
        {
            userDic[userName] = password;
        }
    }

    public bool IsContainsKey(string name)
    {
        if (userDic.ContainsKey(name))
        {
            return true;
        }
        return false;
    }
}
