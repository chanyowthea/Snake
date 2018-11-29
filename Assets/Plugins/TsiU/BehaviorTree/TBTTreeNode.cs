using System;
using System.Collections.Generic;

namespace TsiU
{
    /// <summary>
    /// 行为树节点
    /// </summary>
    public class TBTTreeNode
    {
        //-------------------------------------------------------------------
        private const int defaultChildCount = -1; //TJQ： unlimited count
        //-------------------------------------------------------------------
        /// <summary>
        /// 只记录子节点
        /// </summary>
        private List<TBTTreeNode> _children;
        private int _maxChildCount;
        //private TBTTreeNode _parent;
        //-------------------------------------------------------------------
        public TBTTreeNode(int maxChildCount = -1)
        {
            _children = new List<TBTTreeNode>();
            // 设置了Capacity之后，可以直接取下标
            if (maxChildCount >= 0) {
                _children.Capacity = maxChildCount;
            }
            _maxChildCount = maxChildCount;
        }
        public TBTTreeNode()
            : this(defaultChildCount)
        {}
        ~TBTTreeNode()
        {
            _children = null;
            //_parent = null;
        }
        //-------------------------------------------------------------------
        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="node">父节点</param>
        /// <returns>当前树节点</returns>
        public TBTTreeNode AddChild(TBTTreeNode node)
        {
            if (_maxChildCount >= 0 && _children.Count >= _maxChildCount) {
                TLogger.WARNING("**BT** exceeding child count");
                return this;
            }
            _children.Add(node);
            //node._parent = this;
            return this;
        }
        public int GetChildCount()
        {
            return _children.Count;
        }
        public bool IsIndexValid(int index)
        {
            return index >= 0 && index < _children.Count;
        }
        public T GetChild<T>(int index) where T : TBTTreeNode 
        {
            if (index < 0 || index >= _children.Count) {
                return null;
            }
            return (T)_children[index];
        }
    }
}
