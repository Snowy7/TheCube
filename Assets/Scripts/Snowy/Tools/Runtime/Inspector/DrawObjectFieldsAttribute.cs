using System;
using UnityEngine;

namespace Snowy.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DrawObjectFieldsAttribute : PropertyAttribute
    {
        internal bool NeedIndent { get; }

        public DrawObjectFieldsAttribute(bool needIndent = true)
        {
            NeedIndent = needIndent;
        }
    }
}
