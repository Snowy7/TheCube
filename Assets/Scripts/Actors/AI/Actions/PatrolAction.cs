using System;
using System.Collections.Generic;
using Actors.AI;
using Game;
using Level;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol", story: "[Agent] patrols along [waypoints]", category: "AI",
    id: "7fa0742501dd2f8b7fc2057a03d66f7f")]
public partial class PatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;
    [SerializeReference] public BlackboardVariable<Enemy> enemy;
    [SerializeReference] public BlackboardVariable<AIConfig> config;
    
    Transform m_waypoint;

    private float m_waitingStartedAt;

    protected override Status OnStart()
    {
        Agent.Value.speed = config.Value.patrolSpeed;
        Agent.Value.isStopped = false;
        m_waitingStartedAt = 0f;
        
        LevelManager.Instance.OnSurfaceUpdated += UpdateDestination;
        
        return Status.Running;
    }
    
    void UpdateDestination()
    {
        if (enemy.Value == null || enemy.Value.IsDead) return;
        
        // Set next patrol waypoint
        try
        {
            if (m_waypoint == null)
            {
                m_waypoint = EnemyManager.Instance.GetPatrolPoint(enemy.Value, m_waypoint);
            }
        
            Agent.Value.destination = m_waypoint.position;   
        }
        catch (UnassignedReferenceException)
        {
            // Suggest patrol waypoints for NPC, if none
            Debug.LogWarning("No waypoints assigned for " + Agent.Value.transform.name + ", enemy will remain idle");
            Agent.Value.destination = Agent.Value.transform.position;
        }
    }

    protected override Status OnUpdate()
    {
        if (enemy.Value == null) 
            return Status.Failure;
        
        // if there is no waypoints, stand idle
        if (m_waypoint == null)
        {
            // Try to get a waypoint
            m_waypoint = EnemyManager.Instance.GetPatrolPoint(enemy.Value, m_waypoint);
            
            // Idle
            Idle();
        }

        // Set navigation parameters
        Agent.Value.speed = config.Value.patrolSpeed;

        // Reached waypoint, wait for a moment before keep patrolling
        if (Agent.Value.remainingDistance <= Agent.Value.stoppingDistance && !Agent.Value.pathPending)
        {
            m_waitingStartedAt += Time.deltaTime;
            Agent.Value.speed = 0;

            if (m_waitingStartedAt >= config.Value.patrolWaitTime)
            {
                m_waypoint = EnemyManager.Instance.GetPatrolPoint(enemy.Value, m_waypoint);
                UpdateDestination();
                m_waitingStartedAt = 0f;
                Agent.Value.speed = config.Value.patrolSpeed;
            }
        }
        else
        {
            // Look at the direction of movement
            enemy.Value?.LookAtDirection(Agent.Value.velocity);
        }

        return Status.Running;
    }

    private void Idle()
    {
        // Make sure the speed is set to 0
        Agent.Value.speed = 0;
        Agent.Value.destination = Agent.Value.transform.position;
    }

    protected override void OnEnd()
    {
        if (Agent.Value == null) return;
        
        // Reset the waiting timer
        m_waitingStartedAt = 0; 
        
        // Unsubscribe from the event
        if (LevelManager.Instance) LevelManager.Instance.OnSurfaceUpdated -= UpdateDestination;
        
        m_waypoint = null;
    }
}