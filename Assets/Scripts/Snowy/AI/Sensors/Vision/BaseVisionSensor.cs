using JetBrains.Annotations;
using Snowy.Utils;
using UnityEngine;

namespace Snowy.AI.Sensors
{
    public enum DetectionType
    {
        Cone,
        Sphere
    }
    
    public abstract class BaseVisionSensor : ISensor
    {
        public abstract bool Execute(Transform target, Transform detector, float innerCircle, CountdownTimer timer);
        [CanBeNull] public abstract Transform Execute(LayerMask target, Transform detector, CountdownTimer timer);
        [CanBeNull] public abstract Transform Execute(string target, Transform detector, CountdownTimer timer);

        public abstract void DrawGizmos(Transform transform);
    }
}