using System;
using UnityEngine;

namespace Actors.AI
{
    public class AIAimAK : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private Transform target;
        [SerializeField] private Transform aimTransform;
        [SerializeField] private Transform bone;
        
        [Title("Settings")]
        [SerializeField, Range(0, 50)] private int iterations = 10;
        [SerializeField, Range(0, 1)] public float weight = 1;
        
        private void LateUpdate()
        {
            for (int i = 0; i < iterations; i++)
            {
                AimAtTarget();
            }
        }
        
        private void AimAtTarget()
        {
            Vector3 aimDirection = aimTransform.forward;
            Vector3 targetDirection = target.position - aimTransform.position;
            Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
            
            Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
            bone.rotation = blendedRotation * bone.rotation;
        }
        
        public void SetTarget(Transform target)
        {
            this.target = target;
        }
        
        public void SetWeight(float weight)
        {
            this.weight = weight;
        }
    }
}