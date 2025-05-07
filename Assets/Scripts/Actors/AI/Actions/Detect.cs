using Actors;
using System;
using Actors.Player;
using Actors.AI;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Detect", story: "[Self] Detect [Target]", category: "AI", id: "7edcb6a4a248d7b54e4ee397605993eb")]
public partial class DetectAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<Player> Target;
    
    # region Action Overrides

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null)
        {
            Debug.LogError("[DetectAction] Self is null");
            return Status.Failure;
        }

        DetectionResult detectionResult = Self.Value.Detect();
        if (detectionResult.detected && detectionResult.target != null && detectionResult.target.Character != null && !detectionResult.target.IsDead)
        {
            Target.Value = detectionResult.target;
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }

    # endregion
}