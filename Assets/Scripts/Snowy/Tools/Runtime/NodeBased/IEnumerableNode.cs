using System.Collections.Generic;
using Snowy.NodeBased.Service;

namespace Snowy.NodeBased
{
    public interface IEnumerableNode<TNode> : IEnumerable<TransitionInfo<TNode>> where TNode : Node<TNode>
    {
        new NodeEnumerator<TNode> GetEnumerator();
    }
}
