using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HaoyunFramework
{
    public class UIManagers : SingleTon<UIManagers>
    {
         Dictionary<UIType, GameObject> _UIDic = new Dictionary<UIType, GameObject>();
        private Transform _parentCanvas;
        public UIManagers()
        {
            RefreshParentCanvas();
        }

        public bool IsDicContainsType(string type)
        {
          foreach(var v in _UIDic.Keys)
            {
                if(v.Name == type)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 每次进入HomeView刷新一下
        /// </summary>
        public void RefreshParentCanvas()
        {
            _parentCanvas = GameObject.Find("Canvas").transform;
        }
        public GameObject GetSingleUI(UIType uiType)
        {
            if (!_UIDic.ContainsKey(uiType) || _UIDic[uiType] == null)
            {
                GameObject obj = GameObject.Instantiate(ResourceManager.Instance.LoadAsset<GameObject>(uiType.Path));
                obj.transform.SetParent(_parentCanvas, false);
                obj.name = uiType.Name;
                _UIDic.AddOrReplace(uiType, obj);
                return obj;
            }
            return _UIDic[uiType];
        }

        public void DestroySingleUI(UIType uiType)
        {
            if (!_UIDic.ContainsKey(uiType))
            {
                return;
            }

            if (_UIDic[uiType] == null)
            {
                _UIDic.Remove(uiType);
                return;
            }

            Debug.Log("清空:" + uiType.Name);
            GameObject.Destroy(_UIDic[uiType]);
            _UIDic.Remove(uiType);
        }

        public void ShowConfirmBox(string content, string sure, string cancel, HandleMakeSureEvent sureClick, HandleMakeSureEvent cancelClick)
        {
            //生成tips prefab ，再赋初值
            GetSingleUI(UIType.TipsView).GetComponent<UITipsView>().SetEventHandle
                (
                content,
                sure,
                cancel,
                sureClick,
                cancelClick
                );
        }
        /// <summary>
        /// 改变按钮被点击后的状态
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sprName"></param>
        /// <returns></returns>
        public SpriteState SetButtonStateSpr(EAtlasType type, string sprName)
        {
            SpriteState spriteSwop = new SpriteState();
            spriteSwop.pressedSprite = ResourceManager.Instance.GetSpriteByName((int)type, sprName);
            return spriteSwop;
        }
        #region Tip Queue
        private Queue<string> tipsQueue = new Queue<string>();
        public Queue<string> mTipsQueue
        {
            get
            {
                return tipsQueue;
            }

            set
            {
                if (tipsQueue == null)
                {
                    tipsQueue = new Queue<string>();
                }
                tipsQueue = value;
            }
        }

        /// <summary>
        /// 队列末尾添加元素
        /// </summary>
        /// <param name="value"></param> 
        public void EnqueueTip(string value)
        {
            if (tipsQueue.Count < 10) //加一个重复点击消息的限制
            {
                tipsQueue.Enqueue(value);
            }
        }

        /// <summary>
        /// 读取并删除队列头一个元素
        /// </summary>
        public void DequeueTip()
        {
            tipsQueue.Dequeue();
        }

        /// <summary>
        /// 读取队列头的一个位置
        /// </summary>
        /// <returns></returns>
        public string PeekTip()
        {
            if (tipsQueue.Count > 0)
            {
                return tipsQueue.Peek();
            }
            else
            {
                return null;
            }

        }
        #endregion
    }
}
