using Actors.AI;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Alert Enemy")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Alert Enemy", message: "[Self] spotted something at [Location]", category: "Events", id: "a17278773e9e9443cc256f973b14bc56")]
public partial class AlertEnemy : EventChannelBase
{
    public delegate void AlertEnemyEventHandler(Enemy Self, Vector3 Location);
    public event AlertEnemyEventHandler Event; 

    public void SendEventMessage(Enemy Self, Vector3 Location)
    {
        Event?.Invoke(Self, Location);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<Enemy> SelfBlackboardVariable = messageData[0] as BlackboardVariable<Enemy>;
        var Self = SelfBlackboardVariable != null ? SelfBlackboardVariable.Value : default(Enemy);

        BlackboardVariable<Vector3> LocationBlackboardVariable = messageData[1] as BlackboardVariable<Vector3>;
        var Location = LocationBlackboardVariable != null ? LocationBlackboardVariable.Value : default(Vector3);

        Event?.Invoke(Self, Location);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        AlertEnemyEventHandler del = (Self, Location) =>
        {
            BlackboardVariable<Enemy> var0 = vars[0] as BlackboardVariable<Enemy>;
            if(var0 != null)
                var0.Value = Self;

            BlackboardVariable<Vector3> var1 = vars[1] as BlackboardVariable<Vector3>;
            if(var1 != null)
                var1.Value = Location;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as AlertEnemyEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as AlertEnemyEventHandler;
    }
}

