using System;
using System.Collections.Generic;

namespace TsiU
{
    /// <summary>
    /// 不一定每个都要执行，但要按顺序检测。可以后面的节点先达到条件，然后再前面的达到条件。
    /// </summary>
    public class TBTActionPrioritizedSelector : TBTAction
    {
        protected class TBTActionPrioritizedSelectorContext : TBTActionContext
        {
            internal int currentSelectedIndex;
            internal int lastSelectedIndex;

            public TBTActionPrioritizedSelectorContext()
            {
                currentSelectedIndex = -1;
                lastSelectedIndex = -1;
            }
        }
        public TBTActionPrioritizedSelector()
            : base(-1)
        {
        }
        protected override bool onEvaluate(/*in*/TBTWorkingData wData)
        {
            TBTActionPrioritizedSelectorContext thisContext = getContext<TBTActionPrioritizedSelectorContext>(wData);
            thisContext.currentSelectedIndex = -1;
            int childCount = GetChildCount();
            // 按顺序依次判断所有的子节点，只要有一个子节点满足条件就说明处于这个行为树节点，那么这个父节点也就处于这个状态
            // 并设置当前子节点为父节点的当前index
            for(int i = 0; i < childCount; ++i) {
                TBTAction node = GetChild<TBTAction>(i);
                if (node.Evaluate(wData)) {
                    // 只要一个符合，那么就执行这个
                    thisContext.currentSelectedIndex = i;
                    return true;
                }
            }
            return false;
        }
        protected override int onUpdate(TBTWorkingData wData)
        {
            TBTActionPrioritizedSelectorContext thisContext = getContext<TBTActionPrioritizedSelectorContext>(wData);
            int runningState = TBTRunningStatus.FINISHED;
            // 如果切换节点index了，那么执行转换
            // 那么现在就是上次选择节点和当前节点一致了
            if (thisContext.currentSelectedIndex != thisContext.lastSelectedIndex) {
                if (IsIndexValid(thisContext.lastSelectedIndex)) {
                    TBTAction node = GetChild<TBTAction>(thisContext.lastSelectedIndex);
                    node.Transition(wData);
                }
                thisContext.lastSelectedIndex = thisContext.currentSelectedIndex;
            }
            // 更新当前节点，如果当前节点状态结束，那么设置当前节点为非法
            if (IsIndexValid(thisContext.lastSelectedIndex)) {
                TBTAction node = GetChild<TBTAction>(thisContext.lastSelectedIndex);
                runningState = node.Update(wData);
                if (TBTRunningStatus.IsFinished(runningState)) {
                    // 这里应该用Transition
                    thisContext.lastSelectedIndex = -1;
                }
            }
            return runningState;
        }
        protected override void onTransition(TBTWorkingData wData)
        {
            TBTActionPrioritizedSelectorContext thisContext = getContext<TBTActionPrioritizedSelectorContext>(wData);
            TBTAction node = GetChild<TBTAction>(thisContext.lastSelectedIndex);
            if (node != null) {
                node.Transition(wData);
            }
            thisContext.lastSelectedIndex = -1;
        }
    }
}
