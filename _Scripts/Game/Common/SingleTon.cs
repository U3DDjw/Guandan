using UnityEngine;
using System.Collections;
namespace Common
{
    /// <summary>
    /// 单例管理类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleTon<T> where T:class,new()
    {
        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if(m_Instance==null)
                {
                    m_Instance = new T();
                }
                return m_Instance;
            }
        }
    }
}
