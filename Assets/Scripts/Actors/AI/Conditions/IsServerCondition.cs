using System;
using Mirror;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsServer", story: "Is Server", category: "Conditions", id: "dd97f71d3f2437c32a5bac0478a54edd")]
public partial class IsServerCondition : Condition
{

    public override bool IsTrue()
    {
        return NetworkServer.active;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
