using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Actors.AI
{
    [CreateAssetMenu(menuName = "Enemy AI/Config")]
    public class AIConfig : ScriptableObject
    {
        [Header("General")]
        [Tooltip("NPC rotation speed.")]
        public float rotationSpeed = 120f;
        
        [Header("Detection")]
        public float innerDetectRadius = 5f;
        public float fieldOfViewAngle = 120f;
        public float maxDetectRadius = 10f;
        public float detectRate = 0.5f;
        [Range(1, 11)] public int raysCount = 5;
        public float rayAngle = 10f;
        [TagSelector] public string playerTag = "Player";
        
        [Header("Movement")]
        [Tooltip("NPC patrolling speed (clear state).")]
        public float patrolSpeed = 2f;
        [Tooltip("NPC search speed (warning state).")]
        public float chaseSpeed = 5f;
        [Tooltip("NPC evade speed (engage state).")]
        public float evadeSpeed = 15f;
        [Tooltip("How long to wait on a waypoint.")]
        public float patrolWaitTime = 2f;
        [Tooltip("The obstacle layer mask.")]
        public LayerMask obstacleMask;
        
        [Header("Attack")]
        [Tooltip("The attack range.")]
        public float attackRange = 7f;
        [Tooltip("The threshold to stop attacking if the target is out of range + the threshold.")]
        public float attackRangeThreshold = 1f;
        
        [Header("Cover")]
        [Tooltip("Low cover height to consider crouch when taking cover.")]
        public float aboveCoverHeight = 1.5f;
        [Tooltip("The cover layer mask.")]
        public LayerMask coverMask;

        # if UNITY_EDITOR
        private void OnValidate()
        {
            // make sure the rays count is even
            if (raysCount % 2 == 0)
            {
                raysCount++;
            }
        }
        # endif
    }
}