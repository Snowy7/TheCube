using System;
using Actors;
using Actors.Player;
using Game;
using Level;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using Utils;

# if UNITY_EDITOR
using UnityEditor;
# endif

namespace Actors.AI
{
    [Serializable]
    public struct DetectionResult
    {
        public bool detected;
        public Vector3 direction;
        public float distance;
        public Player.Player target;
    }

    public class Enemy : NetworkActor
    {
        [Header("AI")] 
        public int typeID = 0;
        [SerializeField] private RagdollController ragdollController;
        [SerializeField] private Transform raycastOrigin;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private BehaviorGraphAgent graphAgent;
        [SerializeField] private AIAimAK aimIK;
        [SerializeField] private Transform aimTarget;

        [Header("Configs")] [SerializeField] private AIConfig config;
        [SerializeField] private AlertEnemy alertChannel;
        [SerializeField] private TargetDeath targetDeathChannel;

        [Header("Settings")]
        [SerializeField] private LayerMask playerLayer;
        [SerializeField, Range(0, 100)] private float rotationSpeed = 10;

        [Header("Debug")]
        [SerializeField, Disable] DetectionResult m_lastDetectionResult;
        [SerializeField, Disable] DetectionResult m_lastSeenResult;

        [Disable] public int spawnerID;
        
        private float m_lastDetectionTime;
        private float m_lastSeenTime;

        private float speed;
        private Vector3 suspectLocation;
        
        private Player.Player m_target;

        public override void OnDeath()
        {
            // Disable the agent
            if (agent) Destroy(agent);
            if (graphAgent) Destroy(graphAgent);
            if (aimIK) Destroy(aimIK);

            ragdollController.EnableRagdoll();

            if (m_target)
            {
                m_target.OnActorDeath -= OnTargetDeath;
            }
            
            agent = null;
            graphAgent = null;
            aimIK = null;
            m_target = null;
            
            base.OnDeath();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!isServer)
            {
                // Disable agent
                Destroy(agent);
                Destroy(graphAgent);
                Destroy(aimIK);
                
                agent = null;
                graphAgent = null;
                aimIK = null;
            }
            else
            {
                ResetAimIK();
            }
        }

        private void OnEnable()
        {
            if (EnemyManager.Instance)
            {
                EnemyManager.Instance.RegisterEnemy(this);
            }
        }

        private void OnDisable()
        {
            if (EnemyManager.Instance)
            {
                EnemyManager.Instance.UnregisterEnemy(this);
            }
        }

        private void Update()
        {
            if (IsDead || !isServer)
                return;

            // Animation
            // speed from 0 to 1 (0 = agent.speed = 0, 0.5 = agent.speed < 5, 1 = agent.speed >= 5)

            // Smooth transition by adding or subtracting Time.deltaTime
            if (agent.speed <= 0)
                speed = 0f;
            else if (agent.speed >= 5)
                speed = 1f;
            else
                speed = 0.5f;

            animator.SetFloat("Speed", speed);
        }

        public void LookAtDirection(Vector3 direction)
        {
            // Rotate the NPC to face the direction of movement
            Vector3 lookDirection = direction.normalized;
            if (lookDirection == Vector3.zero)
                return;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            // only rotate on Y axis
            lookRotation.x = 0;
            lookRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation,
                Time.deltaTime * config.rotationSpeed);
        }

        public DetectionResult Detect()
        {
            if (Time.time - m_lastDetectionTime < config.detectRate)
            {
                if (m_lastDetectionResult.target && m_lastDetectionResult.target.IsDead)
                {
                    m_lastDetectionResult = new DetectionResult
                    {
                        detected = false,
                        direction = Vector3.zero,
                        distance = Mathf.Infinity,
                        target = null
                    };
                }
                
                return m_lastDetectionResult;
            }

            m_lastDetectionTime = Time.time;

            // Get all alive players
            Player.Player[] players = GameManager.Instance.GetAlivePlayers();

            foreach (Player.Player player in players)
            {
                if (player == null || player.IsDead)
                    continue;
                
                m_lastDetectionResult = CanSeeTarget(player, true);
                if (m_lastDetectionResult.detected)
                {
                    return m_lastDetectionResult;
                }
            }

            return m_lastDetectionResult;
        }

        public DetectionResult CanSeeTarget(Player.Player player, bool bypassTime = false)
        {
            if (!bypassTime)
            {
                if (Time.time - m_lastSeenTime < config.detectRate)
                {
                    return m_lastSeenResult;
                }

                m_lastSeenTime = Time.time;
            }

            m_lastSeenResult = new DetectionResult
            {
                detected = false,
                direction = Vector3.zero,
                distance = Mathf.Infinity,
                target = null
            };
            
            if (!player || player.IsDead)
            {
                return m_lastSeenResult;
            }

            // Calculate the direction to the player
            Vector3 direction = player.GetHeadPosition() - raycastOrigin.position;

            // the angle is always positive
            float angle = Vector3.Angle(direction, transform.forward);

            // Check if the player is within the field of view
            if (angle < config.fieldOfViewAngle / 2 && direction.magnitude < config.maxDetectRadius)
            {
                // if odd amount add 1 to make it even
                int amount = config.raysCount;
                float offsetAngle = config.rayAngle;

                // get the real angle between -180 and 180
                Vector3 cross = Vector3.Cross(transform.forward, direction);
                if (cross.y < 0)
                {
                    angle = -angle;
                }

                int halfAmount = Mathf.FloorToInt(amount / 2f);

                // Debug ray to the player
                Debug.DrawRay(raycastOrigin.position, direction, Color.red, config.detectRate);
                
                // go from -amount/2 to amount/2
                for (int i = -halfAmount; i <= halfAmount; i++)
                {
                    // direction to the player + the offset angle
                    float rayAngle = i * offsetAngle;
                    Vector3 rayDirection = Quaternion.AngleAxis(rayAngle, Vector3.up) * direction.normalized;

                    // shoot a ray to the rayDirection
                    if (Physics.Raycast(raycastOrigin.position, rayDirection, out RaycastHit hit,
                            config.maxDetectRadius,
                            playerLayer))
                    {
                        // Check if the ray hit the player
                        if (hit.collider.CompareTag(config.playerTag))
                        {
                            m_lastSeenResult.detected = true;
                            m_lastSeenResult.direction = direction;
                            m_lastSeenResult.distance = hit.distance;
                            m_lastSeenResult.target = player;
                            return m_lastSeenResult;
                        }

                        // Debug the ray
                        Debug.DrawRay(raycastOrigin.position,
                            rayDirection * Mathf.Min(hit.distance, config.maxDetectRadius), Color.yellow,
                            config.detectRate);
                    }
                }
            }

            return m_lastSeenResult;
        }

        public Player.Player GetTarget()
        {
            return m_target;
        }

        public void LookAtTarget(Player.Player target)
        {
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0;
            aimTarget.position = target.GetHeadPosition();
            var targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        public void EnableAimIK()
        {
            aimIK.weight = 1;
        }

        public void ResetAimIK()
        {
            aimIK.weight = 0;
        }

# if UNITY_EDITOR

        public void OnDrawGizmosSelected()
        {
            if (!config)
                return;

            // Draw the field of view lines in the scene
            Gizmos.color = Color.red;
            // from -fieldOfViewAngle/2 to fieldOfViewAngle/2
            Vector3 leftRay = Quaternion.AngleAxis(-config.fieldOfViewAngle / 2, transform.up) * transform.forward;
            Vector3 rightRay = Quaternion.AngleAxis(config.fieldOfViewAngle / 2, transform.up) * transform.forward;
            Gizmos.DrawRay(transform.position, leftRay * config.maxDetectRadius);
            Gizmos.DrawRay(transform.position, rightRay * config.maxDetectRadius);


            Handles.color = Color.red;
            Handles.DrawWireArc(transform.position, Vector3.up, leftRay, config.fieldOfViewAngle,
                config.maxDetectRadius);
        }

#endif
        public void Alert(Vector3 location)
        {
            suspectLocation = location;
            alertChannel.SendEventMessage(this, location);
        }

        public void GetSuspectLocation(out Vector3 location)
        {
            location = suspectLocation;
            suspectLocation = Vector3.zero;
        }

        public void AssignPriority(int priority)
        {
            if (agent) agent.avoidancePriority = priority;
        }

        public void RegisterTarget(Player.Player targetValue)
        {
            m_target = targetValue;
            targetValue.OnActorDeath += OnTargetDeath;
        }

        public void OnTargetDeath(NetworkActor actor)
        {
            // Unregister the event
            if (m_target) m_target.OnActorDeath -= OnTargetDeath;
            
            // Reset the target
            m_target = null;
            
            // Run the event
            targetDeathChannel.SendEventMessage(m_target);
        }
    }
}