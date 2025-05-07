using Actors;
using System;
using Actors.Player;
using Actors.AI;
using Game;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Navigate", story: "[Self] Navigate to [Target]", category: "AI", id: "50ba0c514ac05b63b3ce81c06cf2f43c")]
public partial class NavigateAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Self;
    [SerializeReference] public BlackboardVariable<Player> Target;

    [SerializeReference] public BlackboardVariable<Enemy> enemy;
    [SerializeReference] public BlackboardVariable<AIConfig> config;
    
    protected override Status OnStart()
    {
        if (Self.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }
        
        if (Target.Value.IsDead || Target.Value.Character == null)
        {
            return Status.Failure;
        }
        
        // Set navigation parameters
        Self.Value.speed = config.Value.chaseSpeed;
        Self.Value.destination = Target.Value.transform.position;
        Self.Value.isStopped = false;
        
        // when the surface is updated (door opened, etc), we need to recalculate the path
        LevelManager.Instance.OnSurfaceUpdated += RecalculatePath;
        return Status.Running;
    }
    
    private void RecalculatePath()
    {
        if (Self.Value == null || Target.Value == null)
        {
            return;
        }
        
        Self.Value.destination = Target.Value.transform.position;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }
        
        if (Target.Value.IsDead || Target.Value.Character == null)
        {
            return Status.Failure;
        }
        
        // We need to go to the target; set the destination
        // We need to stop the agent if we are close enough
        Self.Value.destination = Target.Value.transform.position;
        if (Self.Value.remainingDistance <= config.Value.attackRange && !Self.Value.pathPending && enemy.Value.CanSeeTarget(Target.Value).detected)
        {
            // We are close enough to the target
            Self.Value.speed = 0;
            Self.Value.isStopped = true;
            // Reset velocity
            Self.Value.velocity = Vector3.zero;
            return Status.Success;
        }
        
        enemy.Value?.LookAtDirection(Self.Value.velocity);
        return Status.Running;
    }

    protected override void OnEnd()
    {
        try
        {
            // Reset speed
            Self.Value.speed = 0;
            Self.Value.isStopped = true;
        }
        catch (Exception e)
        {
            // ignored
        }
        
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnSurfaceUpdated -= RecalculatePath;
        }
    }
}

