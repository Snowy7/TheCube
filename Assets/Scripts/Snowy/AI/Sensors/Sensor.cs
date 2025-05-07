using Snowy.Utils;
using UnityEngine;

namespace Snowy.AI.Sensors
{
    public class Sensor : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] DetectionType detectionType;
        [SerializeField] LayerMask targetLayer;
        [ShowIf("detectionType", DetectionType.Cone), SerializeField] float detectionAngle = 45.0f;
        [ShowIf("detectionType", DetectionType.Cone), SerializeField] float detectionRadius = 5.0f;
        [ShowIf("detectionType", DetectionType.Cone), SerializeField] float detectionInnerRadius = 1.0f;
        [ShowIf("detectionType", DetectionType.Sphere), SerializeField] float detectionRadiusSphere = 5.0f;
        
        [SerializeField] float detectionCooldown = 1.0f;
        
        private BaseVisionSensor sensor;
        
        private CountdownTimer detectionTimer;
        
        Transform target;
        private bool m_lastDetection;
        
        public BaseVisionSensor GetSensor()
        {
            switch (detectionType)
            {
                case DetectionType.Cone:
                    return new ConeVisionSensor(detectionAngle, detectionRadius, detectionInnerRadius);
                case DetectionType.Sphere:
                    return new SphereVisionSensor(detectionRadiusSphere);
                default:
                    return null;
            }
        }
        
        # if UNITY_EDITOR
        
        private void OnValidate()
        {
            sensor = GetSensor();
            detectionTimer = new CountdownTimer(detectionCooldown);
        }
        
        void OnDrawGizmos()
        {
            if (sensor == null) return;
            sensor.DrawGizmos(transform);
        }
        
        # endif

        private void Start()
        {
            sensor = GetSensor();
            detectionTimer = new CountdownTimer(detectionCooldown);
        }

        private void Update()
        {
            detectionTimer.Tick(Time.deltaTime);
        }

        public Transform DetectClosest()
        {
            if (detectionTimer.IsRunning)
                return target;
            
            var newTarget = sensor.Execute(targetLayer, transform, detectionTimer);
            return target = newTarget;
        }

        public Transform DetectClosestWithTag(string targetTag)
        {
            // if cooldown is not over, return
            if (detectionTimer.IsRunning)
                return target;
            
            var newTarget = sensor.Execute(targetTag, transform, detectionTimer);
            return target = newTarget;
        }
        
        public bool CanDetectTarget(Transform toCheck)
        {
            if (detectionTimer.IsRunning)
            {
                return m_lastDetection;
            }
            
            m_lastDetection = sensor.Execute(toCheck, transform, detectionInnerRadius, detectionTimer);
            return m_lastDetection;
        }
    }
}