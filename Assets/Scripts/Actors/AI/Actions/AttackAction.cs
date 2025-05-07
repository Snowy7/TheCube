using Actors;
using Actors.AI;
using System;
using Actors.Player;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Self] Attack [Target] With [AttackType]", category: "AI", id: "d44d908c6849afdb0cd5aeec7421d599")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<Player> Target;
    [SerializeReference] public BlackboardVariable<BaseAttack> AttackType;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null)
        {
            Debug.LogError("[AttackAction] Self is null");
            return Status.Failure;
        }
        
        if (Target.Value == null)
        {
            Debug.LogError("[AttackAction] Target is null");
            return Status.Failure;
        }
        
        if (AttackType.Value == null)
        {
            Debug.LogError("[AttackAction] AttackType is null");
            return Status.Failure;
        }
        
        AttackType.Value.Attack(Target.Value);
        Self.Value.LookAtTarget(Target.Value);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

