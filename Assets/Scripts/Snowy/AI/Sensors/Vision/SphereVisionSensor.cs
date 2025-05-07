using Snowy.Utils;
using UnityEngine;

namespace Snowy.AI.Sensors
{
    public class SphereVisionSensor : BaseVisionSensor
    {
        readonly float detectionRadius;
        
        public SphereVisionSensor(float detectionRadius)
        {
            this.detectionRadius = detectionRadius;
        }
        
        public override bool Execute(Transform target, Transform detector, float innerCircle, CountdownTimer timer)
        {
            if (timer.IsRunning) return false;
            
            var directionToPlayer = target.position - detector.position;
            
            // If the player is not within the detection radius, return false.
            if (!(directionToPlayer.magnitude < detectionRadius)) return false;
            
            timer.Start();
            return true;
        }
        
        public override Transform Execute(LayerMask target, Transform detector, CountdownTimer timer)
        {
            if (timer.IsRunning) return null;
            
            var colliders = Physics.OverlapSphere(detector.position, detectionRadius, target);
            foreach (var collider in colliders)
            {
                timer.Start();
                return collider.transform;
            }
            
            return null;
        }
        
        public override Transform Execute(string target, Transform detector, CountdownTimer timer)
        {
            if (timer.IsRunning) return null;
            
            var colliders = Physics.OverlapSphere(detector.position, detectionRadius);
            foreach (var collider in colliders)
            {
                if (collider.CompareTag(target))
                {
                    timer.Start();
                    return collider.transform;
                }
            }
            
            return null;
        }
        
        public override void DrawGizmos(Transform transform)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}