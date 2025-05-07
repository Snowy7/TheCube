using System;
using Actors.AI;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check Suspect Location", story: "[Self] Check Suspect [Location]", category: "Action", id: "1cd71b9c06b83266c9f4052e142b0b60")]
public partial class CheckSuspectLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Self;
    [SerializeReference] public BlackboardVariable<Enemy> enemy;
    [SerializeReference] public BlackboardVariable<Vector3> Location;
    [SerializeReference] public BlackboardVariable<AIConfig> Config;
    
    private float m_waitingStartedAt;

    protected override Status OnStart()
    {
        Self.Value.speed = Config.Value.patrolSpeed;
        Self.Value.isStopped = false;
        Self.Value.SetDestination(Location.Value);
        m_waitingStartedAt = 0f;
        
        // if the suspecious location is 0,0,0, then we need to find the player's location
        if (Location.Value == Vector3.zero)
        {
            Location.Value = enemy.Value.transform.position;
        }
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value.remainingDistance <= Self.Value.stoppingDistance && !Self.Value.pathPending)
        {
            Self.Value.isStopped = true;
            Self.Value.speed = 0f;
            m_waitingStartedAt += Time.deltaTime;

            if (m_waitingStartedAt >= Config.Value.patrolWaitTime)
            {
                m_waitingStartedAt = 0f;
                return Status.Success;
            }
        }
        
        // Look into the direction of movement on y axis
        enemy.Value?.LookAtDirection(Self.Value.velocity);
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (Self.Value == null || Self.Value.enabled == false || Self.Value.gameObject.activeSelf == false)
        {
            return;
        }
        
        Self.Value.isStopped = true;
        Self.Value.speed = 0f;
    }
}

