﻿using System;
using System.Collections;
using System.Collections.Generic;
using Snowy.NodeBased.Service;

namespace Snowy.NodeBased
{
    [Serializable]
    public abstract class Node<TNode> : RawNode, IEnumerableNode<TNode> where TNode : Node<TNode>
    {
        public Graph<TNode> Graph => Owner as Graph<TNode>;
        public bool IsRoot => Owner.RootNode == this;
        internal override NodeType NodeType => NodeType.Regular;

        public NodeEnumerator<TNode> GetEnumerator()
        {
            return new NodeEnumerator<TNode>(this as TNode);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new NodeEnumerator<TNode>(this as TNode);
        }

        IEnumerator<TransitionInfo<TNode>> IEnumerable<TransitionInfo<TNode>>.GetEnumerator()
        {
            return new NodeEnumerator<TNode>(this as TNode);
        }
    }
}
