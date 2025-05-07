using Actors.AI;
using System;
using Actors;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackIKSetup", story: "[Self] Setup AttackIK", category: "Action", id: "b40ad9c502cdbc5b8c8c3712b3d9af5f")]
public partial class AttackIkSetupAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Self.Value.EnableAimIK();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

