using Actors.AI;
using System;
using Actors;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Reset Attack IK", story: "[Self] Reset Attack IK", category: "Action", id: "a997519822300ffebf3511809f52800b")]
public partial class ResetAttackIkAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Self.Value.ResetAimIK();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

