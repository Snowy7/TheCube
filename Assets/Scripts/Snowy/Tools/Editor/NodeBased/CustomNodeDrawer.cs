﻿using System;
using Snowy.CSharp;
using Snowy.NodeBased.Service;

namespace SnowyEditor.NodeBased
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomNodeDrawerAttribute : Attribute
    {
        internal Type NodeType { get; }

        public CustomNodeDrawerAttribute(Type nodeType)
        {
            if (!nodeType.IsAssignableTo(typeof(RawNode)))
                throw new ArgumentException($"Given type is not assignable to {typeof(RawNode).FullName}");

            NodeType = nodeType;
        }
    }
}
