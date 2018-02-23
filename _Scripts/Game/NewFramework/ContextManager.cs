using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HaoyunFramework
{
    public class ContextManager : SingleTon<ContextManager>
    {
        private List<BaseContext> _contextList = new List<BaseContext>();

        bool IsContaines(string name,out BaseContext context)
        {
            context = null;
            for(int i=0;i<_contextList.Count;i++)
            {
                if(_contextList[i].ViewType.Name == name)
                {
                    context = _contextList[i];
                    return true;
                }
            }
            return false;
        }

        public bool IsContains(string name)
        {
            for (int i = 0; i < _contextList.Count; i++)
            {
                if (_contextList[i].ViewType.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 适用于窗体对象的加载
        /// </summary>
        /// <param name="nextContext"></param>
        public void Push(BaseContext nextContext)
        {
            if (IsContains(nextContext.ViewType.Name)) {
                Debug.Log("当前context 已存在:" + nextContext.ToString());
                return;
            }

            _contextList.Add(nextContext);
            var view = UIManagers.Instance.GetSingleUI(nextContext.ViewType).GetComponent<BasesView>();
            view.OnEnter(nextContext);
        }

        /// <summary>
        /// 窗体对象的退出
        /// </summary>
        public void Pop(BaseContext context)
        {
            if(IsContains(context.ViewType.Name))
            {
                _contextList.Remove(context);
                UIManagers.Instance.DestroySingleUI(context.ViewType);
            }
        }

        public void Pop(string name)
        {
            BaseContext context = null;
            if(IsContaines(name,out context))
            {
                Pop(context);
            }
        }
     
        public void PopAll()
        {
            for(int i=_contextList.Count-1;i>=0;i--)
            {
                Pop(_contextList[i]);
            }
       
        }
    }
}
