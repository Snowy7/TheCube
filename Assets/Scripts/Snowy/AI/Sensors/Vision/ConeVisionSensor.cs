using Snowy.Utils;
using UnityEngine;

namespace Snowy.AI.Sensors
{
    public class ConeVisionSensor : BaseVisionSensor
    {
        readonly float detectionAngle;
        readonly float detectionRadius;
        readonly float detectionInnerRadius;
        
        public ConeVisionSensor(float detectionAngle, float detectionRadius, float detectionInnerRadius)
        {
            this.detectionAngle = detectionAngle;
            this.detectionRadius = detectionRadius;
            this.detectionInnerRadius = detectionInnerRadius;
        }
        
        public override bool Execute(Transform target, Transform detector, float innerCircle, CountdownTimer timer)
        {
            if (timer.IsRunning) return false;
            
            bool losBlocked = Physics.Linecast(detector.position, target.position, out var hit);
                
# if UNITY_EDITOR
            // gizmos for line of sight check
            Debug.DrawLine(detector.position + Vector3.up * .5f, target.position + Vector3.up * .5f, !losBlocked ? Color.green : Color.red);
# endif
                
            if (losBlocked) return false;
            
            timer.Start();
            return true;
        }
        
        public override Transform Execute(LayerMask target, Transform detector, CountdownTimer timer)
        {
            if (timer.IsRunning) return null;
            
            var results = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(detector.position, detectionRadius, results, target);
            
            foreach (var collider in results)
            {
                if (collider == null) continue;
                var directionToPlayer = collider.transform.position - detector.position;
                var angleToPlayer = Vector3.Angle(directionToPlayer, detector.forward);
                

                // If the player is not within the detection angle or outer radius (aka the cone in front of the AI),
                // or isn't within the inner radius, return false.
                if ((!(angleToPlayer < detectionAngle / 2f) || !(directionToPlayer.magnitude < detectionRadius))
                    && !(directionToPlayer.magnitude < detectionInnerRadius))
                    continue;
                
                bool losBlocked = Physics.Linecast(detector.position, collider.transform.position, out var hit);
                
                # if UNITY_EDITOR
                // gizmos for line of sight check
                Debug.DrawLine(detector.position + Vector3.up * .5f, collider.transform.position + Vector3.up * .5f, !losBlocked ? Color.green : Color.red);
                # endif
                
                if (losBlocked) continue;
                
                timer.Start();
                return collider.transform;
            }
            
            return null;
        }
        
        public override Transform Execute(string target, Transform detector, CountdownTimer timer)
        {
            if (timer.IsRunning) return null;
            
            var results = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(detector.position, detectionRadius, results);
            Debug.Log($"Size: {size}");
            foreach (var collider in results)
            {
                if (collider == null) continue;
                if (!collider.CompareTag(target)) continue;
                var directionToPlayer = collider.transform.position - detector.position;
                var angleToPlayer = Vector3.Angle(directionToPlayer, detector.forward);
                

                // If the player is not within the detection angle or outer radius (aka the cone in front of the AI),
                // or isn't within the inner radius, return false.
                if ((!(angleToPlayer < detectionAngle / 2f) || !(directionToPlayer.magnitude < detectionRadius))
                    && !(directionToPlayer.magnitude < detectionInnerRadius))
                    continue;
                
                bool losBlocked = Physics.Linecast(detector.position, collider.transform.position, out var hit);
                
                # if UNITY_EDITOR
                // gizmos for line of sight check
                Debug.DrawLine(detector.position + Vector3.up * .5f, collider.transform.position + Vector3.up * .5f, !losBlocked ? Color.green : Color.red);
                # endif
                
                if (losBlocked) continue;
                
                timer.Start();
                return collider.transform;
            }
            
            return null;
        }
        
        public override void DrawGizmos(Transform transform)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0, detectionAngle / 2f, 0) * transform.forward * detectionRadius);
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -detectionAngle / 2f, 0) * transform.forward * detectionRadius);
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Gizmos.DrawWireSphere(transform.position, detectionInnerRadius);
        }
    }
}