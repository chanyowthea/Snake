using System;
using System.Collections.Generic;

namespace TsiU
{
    /// <summary>
    /// 按顺序执行，每一个都要执行
    /// </summary>
    public class TBTActionSequence : TBTAction
    {
        //-------------------------------------------------------
        protected class TBTActionSequenceContext : TBTActionContext
        {
            internal int currentSelectedIndex;
            public TBTActionSequenceContext()
            {
                currentSelectedIndex = -1;
            }
        }
        //-------------------------------------------------------
        private bool _continueIfErrorOccors;
        //-------------------------------------------------------
        public TBTActionSequence()
            : base(-1)
        {
            _continueIfErrorOccors = false;
        }
        public TBTActionSequence SetContinueIfErrorOccors(bool v)
        {
            _continueIfErrorOccors = v;
            return this;
        }
        //------------------------------------------------------
        protected override bool onEvaluate(/*in*/TBTWorkingData wData)
        {
            TBTActionSequenceContext thisContext = getContext<TBTActionSequenceContext>(wData);
            int checkedNodeIndex = -1;
            if (IsIndexValid(thisContext.currentSelectedIndex)) {
                checkedNodeIndex = thisContext.currentSelectedIndex;
            } else {
                checkedNodeIndex = 0;
            }
            // 检测当前节点
            if (IsIndexValid(checkedNodeIndex)) {
                TBTAction node = GetChild<TBTAction>(checkedNodeIndex);
                if (node.Evaluate(wData)) {
                    thisContext.currentSelectedIndex = checkedNodeIndex;
                    return true;
                }
            }
            return false;
        }
        protected override int onUpdate(TBTWorkingData wData)
        {
            TBTActionSequenceContext thisContext = getContext<TBTActionSequenceContext>(wData);
            int runningStatus = TBTRunningStatus.FINISHED;
            TBTAction node = GetChild<TBTAction>(thisContext.currentSelectedIndex);

            // 更新当前节点
            runningStatus = node.Update(wData);

            // 如果当前节点结束或者失败，那么父节点也结束
            if (_continueIfErrorOccors == false && TBTRunningStatus.IsError(runningStatus)) {
                thisContext.currentSelectedIndex = -1;
                return runningStatus;
            }

            // 如果当前节点结束，那么切换下一个节点
            if (TBTRunningStatus.IsFinished(runningStatus)) {
                thisContext.currentSelectedIndex++;
                // 这个检测不应该放在这里
                if (IsIndexValid(thisContext.currentSelectedIndex)) {
                    runningStatus = TBTRunningStatus.EXECUTING;
                } else {
                    thisContext.currentSelectedIndex = -1;
                }
            }
            return runningStatus;
        }
        protected override void onTransition(TBTWorkingData wData)
        {
            TBTActionSequenceContext thisContext = getContext<TBTActionSequenceContext>(wData);
            TBTAction node = GetChild<TBTAction>(thisContext.currentSelectedIndex);
            if (node != null) {
                node.Transition(wData);
            }
            thisContext.currentSelectedIndex = -1;
        }
    }
}
