using System;
using System.Collections.Generic;

namespace TsiU
{
    /// <summary>
    /// 所有普通的节点都是Leaf
    /// </summary>
    public abstract class TBTActionLeaf : TBTAction
    {
        private const int ACTION_READY = 0;
        private const int ACTION_RUNNING = 1;
        private const int ACTION_FINISHED = 2;

        class TBTActionLeafContext : TBTActionContext
        {
            /// <summary>
            /// 当前行为节点的状态，这个能不能去掉？
            /// </summary>
            internal int status; 

            /// <summary>
            /// 是否需要执行onExit
            /// </summary>
            internal bool needExit;

            /// <summary>
            /// 其实这个没有必要，直接在外部子类定义变量就行
            /// </summary>
            private object _userData;
            public T getUserData<T>() where T : class, new()
            {
                if (_userData == null) {
                    _userData = new T();
                }
                return (T)_userData;
            }
            public TBTActionLeafContext()
            {
                status = ACTION_READY;
                needExit = false;

                _userData = null;
            }
        }
        public TBTActionLeaf()
            : base(0)
        {
        }
        protected sealed override int onUpdate(TBTWorkingData wData)
        {
            int runningState = TBTRunningStatus.FINISHED;
            TBTActionLeafContext thisContext = getContext<TBTActionLeafContext>(wData);
            // 只执行一次
            if (thisContext.status == ACTION_READY) {
                onEnter(wData);
                thisContext.needExit = true;
                thisContext.status = ACTION_RUNNING;
            }

            // status是ACTION_RUNNING，但是thisContext.status是其他的状态
            if (thisContext.status == ACTION_RUNNING) {
                // TBTRunningStatus是内部的执行状态，status是TBTActionLeaf是的状态
                runningState = onExecute(wData);
                if (TBTRunningStatus.IsFinished(runningState)) {
                    thisContext.status = ACTION_FINISHED;
                }
            }
            if (thisContext.status == ACTION_FINISHED) {
                if (thisContext.needExit) {
                    onExit(wData, runningState);
                }
                // 返回默认状态
                thisContext.status = ACTION_READY;
                thisContext.needExit = false;
            }
            return runningState;
        }
        protected sealed override void onTransition(TBTWorkingData wData)
        {
            TBTActionLeafContext thisContext = getContext<TBTActionLeafContext>(wData);
            if (thisContext.needExit) {
                onExit(wData, TBTRunningStatus.TRANSITION);
            }
            thisContext.status = ACTION_READY;
            thisContext.needExit = false;
        }
        protected T getUserContexData<T>(TBTWorkingData wData) where T : class, new()
        {
            // TBTActionLeafContext中获取userdata
            return getContext<TBTActionLeafContext>(wData).getUserData<T>();
        }
        //--------------------------------------------------------
        // inherented by children-
        protected virtual void onEnter(/*in*/TBTWorkingData wData)
        {
        }
        protected virtual int onExecute(TBTWorkingData wData)
        {
            return TBTRunningStatus.FINISHED;
        }
        protected virtual void onExit(TBTWorkingData wData, int runningStatus)
        {

        }
    }
}
