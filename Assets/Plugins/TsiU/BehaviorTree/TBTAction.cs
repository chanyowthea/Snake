using System;

namespace TsiU
{
    /// <summary>
    /// 行为节点环境，即数据
    /// </summary>
    public class TBTActionContext
    {
    }

    /// <summary>
    /// 行为节点
    /// </summary>
    public abstract class TBTAction : TBTTreeNode
    {
        static private int sUNIQUEKEY = 0;
        static private int genUniqueKey()
        {
            if (sUNIQUEKEY >= int.MaxValue){
                sUNIQUEKEY = 0;
            } else {
                sUNIQUEKEY = sUNIQUEKEY + 1;
            }
            return sUNIQUEKEY;
        }
        //-------------------------------------------------------------
        protected int _uniqueKey;
        protected TBTPrecondition _precondition;
#if DEBUG
        protected string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
#endif
        //-------------------------------------------------------------
        public TBTAction(int maxChildCount)
            : base(maxChildCount)
        {
            // 将GenUniqueKey放在基类是好的，但这个Unique应该作为一个更通用的类
            _uniqueKey = TBTAction.genUniqueKey();
        }
        ~TBTAction()
        {
            _precondition = null;
        }
        //-------------------------------------------------------------
        /// <summary>
        /// 判断是否仍然处于这个状态
        /// </summary>
        /// <param name="wData">整个行为树的工作数据</param>
        /// <returns>是否处于这个状态</returns>
        public bool Evaluate(/*in*/TBTWorkingData wData)
        {
            return (_precondition == null || _precondition.IsTrue(wData)) && onEvaluate(wData);
        }

        /// <summary>
        /// 执行一些处于当前状态的操作
        /// </summary>
        /// <param name="wData"></param>
        /// <returns>当前行为节点的运行状态</returns>
        public int Update(TBTWorkingData wData)
        {
            return onUpdate(wData);
        }

        /// <summary>
        /// 切换成其他状态
        /// </summary>
        /// <param name="wData"></param>
        public void Transition(TBTWorkingData wData)
        {
            onTransition(wData);
        }

        /// <summary>
        /// 进入这个行为节点的前置条件
        /// </summary>
        /// <param name="precondition">前置条件</param>
        /// <returns></returns>
        public TBTAction SetPrecondition(TBTPrecondition precondition)
        {
            _precondition = precondition;
            return this;
        }
        public override int GetHashCode()
        {
            return _uniqueKey;
        }

        /// <summary>
        /// 创建行为节点的数据
        /// </summary>
        /// <typeparam name="T">行为节点数据的类型</typeparam>
        /// <param name="wData"></param>
        /// <returns>行为节点数据实例</returns>
        protected T getContext<T>(TBTWorkingData wData) where T : TBTActionContext, new()
        {
            // 获取NOD_Attack的hashcode
            int uniqueKey = GetHashCode();
            T thisContext;
            if (wData.context.ContainsKey(uniqueKey) == false) {
                thisContext = new T();
                // 在它的context字典中加入这个NOD_Attack
                wData.context.Add(uniqueKey, thisContext);
            } else {
                // 如果这个字典里面已经有了这个action
                thisContext = (T)wData.context[uniqueKey];
            }
            return thisContext;
        }
        //--------------------------------------------------------
        // inherented by children
        /// <summary>
        /// 判断是否处于本行为节点。内部使用
        /// </summary>
        protected virtual bool onEvaluate(/*in*/TBTWorkingData wData)
        {
            return true;
        }
        /// <summary>
        /// 处于本节点时，每一帧的操作。内部使用
        /// </summary>
        protected virtual int onUpdate(TBTWorkingData wData)
        {
            return TBTRunningStatus.FINISHED;
        }
        /// <summary>
        /// 切换成其他状态。内部使用
        /// </summary>
        protected virtual void onTransition(TBTWorkingData wData)
        {
        }
    }
}
