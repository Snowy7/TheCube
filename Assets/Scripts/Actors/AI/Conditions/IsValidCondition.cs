using System;
using Unity.Behavior;
using UnityEngine;

public enum ConditionType
{
    IsValid,
    NotValid
}

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Valid", story: "[Object] [State]", category: "Conditions", id: "4935d591fac6ea7cef87e0df3c36ae81")]
public partial class IsValidCondition : Condition
{
    [SerializeReference] public BlackboardVariable<MonoBehaviour> Object;
    [SerializeReference] public BlackboardVariable<ConditionType> State;

    public override bool IsTrue()
    {
        if (Object.Value == null)
        {
            return State.Value == ConditionType.NotValid;
        }
        return State.Value == ConditionType.IsValid;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
