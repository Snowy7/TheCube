using System;
using Actors.AI;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LoadAlertData", story: "Load [SuspectPosition]", category: "AI", id: "bfc6ffe87dfa2d4e601ea1ce0d7599cf")]
public partial class LoadAlertDataAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<Vector3> SuspectPosition;

    protected override Status OnStart()
    {
        if (Enemy.Value == null)
        {
            return Status.Failure;
        }
        
        Enemy.Value.GetSuspectLocation(out var suspectLocation);
        SuspectPosition.Value = suspectLocation;
        
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

