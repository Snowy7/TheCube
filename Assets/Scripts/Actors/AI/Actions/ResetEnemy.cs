using Actors.AI;
using Actors.Player;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ResetEnemy", story: "Reset [Enemy] : [State] and [Target]", category: "AI", id: "9f40b524f194a37db7fd642d5781f75d")]
public partial class ResetEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<AIState> State;
    [SerializeReference] public BlackboardVariable<Player> Target;
    protected override Status OnStart()
    {
        if (Enemy.Value == null)
        {
            Debug.LogError("[ResetEnemyAction] Enemy is null");
            return Status.Failure;
        }
        
        if (Enemy.Value.GetTarget() == null)
        {
            // Reset the enemy
            State.Value = AIState.Patrol;
            Target.Value = null;
        }
        
        // Just return success
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

