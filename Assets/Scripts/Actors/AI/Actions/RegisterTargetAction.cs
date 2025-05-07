using Actors.AI;
using Actors.Player;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RegisterTarget", story: "[Self] register [Target]", category: "Action", id: "3c9d44d44fbbb928ce70d9d5b320e877")]
public partial class RegisterTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<Player> Target;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            Debug.LogError("[RegisterTargetAction] Self is null");
            return Status.Failure;
        }
        
        if (Target.Value == null)
        {
            Debug.LogError("[RegisterTargetAction] Target is null");
            return Status.Failure;
        }
        
        Self.Value.RegisterTarget(Target.Value);
        
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

