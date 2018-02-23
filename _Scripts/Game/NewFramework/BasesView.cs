using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HaoyunFramework
{
    public abstract class BasesView : MonoBehaviour
    {

        protected BaseContext curContext = null;
        public virtual void OnEnter(BaseContext context)
        {
            curContext = context;
        }
        //销毁自身
        public virtual void OnExit(BaseContext context)
        {
            DestroySelf();
        }

        public virtual void OnPause(BaseContext context)
        {
            this.gameObject.SetActive(false);
        }

        public virtual void OnResume(BaseContext context)
        {
            this.gameObject.SetActive(true);
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }


    }
}
