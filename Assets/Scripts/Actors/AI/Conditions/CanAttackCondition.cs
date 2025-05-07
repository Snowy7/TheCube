using Actors;
using Actors.AI;
using System;
using Actors.Player;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CanAttack", story: "[Self] Can Attack [Target] using [Config]", category: "Conditions", id: "0bd62ce83c34f7a34a36175c1c0f0e07")]
public partial class CanAttackCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<Player> Target;
    [SerializeReference] public BlackboardVariable<AIConfig> Config;

    public override bool IsTrue()
    {
        DetectionResult detectionResult = Self.Value.CanSeeTarget(Target.Value);
        if (detectionResult.detected && detectionResult.distance <= (Config.Value.attackRange + Config.Value.attackRangeThreshold))
        {
            return true;
        }
        
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
