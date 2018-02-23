using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class RecordManager : SingleTon<RecordManager>
{
    /// <summary>
    /// 勾选的item的信息
    /// </summary>
    List<RecordData> checkBoxList = new List<RecordData>();
    public List<RecordData> mCheckBoxList
    {
        get
        {
            return checkBoxList;
        }
    }
    /// <summary>
    /// 记录
    /// </summary>
    private List<RecordData> recordDataList = new List<RecordData>();
    public List<RecordData> mRecordDataList
    {
        get
        {
            return recordDataList;
        }
    }
    public void UpdateRecordData(List<RecordData> recordList)
    {
        recordDataList = recordList;
    }

    /// <summary>
    /// 获取被选择的场数中胜利的场次
    /// </summary>
    private int winSession = 0;
    public int WinSession
    {
        get
        {
            GetSession();
            return winSession;
        }
        set
        {
            winSession = value;

        }
    }
    //public int loseSession = 0;
    //public int LoseSession
    //{
    //    get { return loseSession; }
    //    set { loseSession = value; }
    //}
    public void ToZero()
    {
        goalNum = 0;
        winSession = 0;
        //loseSession = 0;
    }
    void GetSession()
    {
        if (checkBoxList != null)
        {
            winSession = 0;
            for (int i = 0; i < checkBoxList.Count; i++)
            {
                List<Dictionary<string, string>> dic = checkBoxList[i].info;
                for (int j = 0; j < dic.Count; j++)
                {
                    string pid = dic[j]["pid"];
                    string win = dic[j]["win"];
                    if (PlayerInfo.Instance.mPlayerPid == ulong.Parse(pid))
                    {
                        if (win == null || win == "0")
                        {
                            winSession++;
                        }
                        //else
                        //{
                        //    loseSession--
                        //}
                    }
                }
            }
        }
    }
    private int goalNum = 0;
    public int GoalNum
    {
        get
        {
            GetScore();
            return goalNum;
        }
        set
        {
            goalNum = value;
        }
    }
    void GetScore()
    {
        if (checkBoxList != null)
        {
            for (int i = 0; i < checkBoxList.Count; i++)
            {
                List<Dictionary<string, string>> dic = checkBoxList[i].info;
                for (int j = 0; j < dic.Count; j++)
                {
                    if (dic[j].ContainsKey("pid"))
                    {
                        string pid = dic[j]["pid"];//获取玩家的id
                        string goal = dic[j]["allGoal"];
                        if (PlayerInfo.Instance.mPlayerPid == ulong.Parse(pid))
                        {
                            goalNum += int.Parse(goal);
                        }
                    }
                }
            }
        }
    }
}

