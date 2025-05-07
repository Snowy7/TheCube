using Actors.AI;
using Actors.Player;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/TargetDeath")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "TargetDeath", message: "On [Target] Death", category: "AI/Events", id: "6adfa5b169fa96d1bf74eda527c21499")]
public partial class TargetDeath : EventChannelBase
{
    public delegate void TargetDeathEventHandler(Player Target);
    public event TargetDeathEventHandler Event; 

    public void SendEventMessage(Player Target)
    {
        Event?.Invoke(Target);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<Player> TargetBlackboardVariable = messageData[1] as BlackboardVariable<Player>;
        var Target = TargetBlackboardVariable != null ? TargetBlackboardVariable.Value : default(Player);

        Event?.Invoke(Target);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        TargetDeathEventHandler del = (Target) =>
        {
            BlackboardVariable<Player> var1 = vars[1] as BlackboardVariable<Player>;
            if(var1 != null)
                var1.Value = Target;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as TargetDeathEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as TargetDeathEventHandler;
    }
}

