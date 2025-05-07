using System;
using UnityEngine;

namespace Snowy.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class LayerFieldAttribute : PropertyAttribute { }
}
