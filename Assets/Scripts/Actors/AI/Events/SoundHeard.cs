using Actors.AI;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/SoundHeard")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "SoundHeard", message: "[Self] heard sound at [Position]", category: "Events", id: "ad44d400a19857e53ee11d163924011e")]
public partial class SoundHeard : EventChannelBase
{
    public delegate void SoundHeardEventHandler(Enemy Self, Vector3 Position);
    public event SoundHeardEventHandler Event;

    public void SendEventMessage(Enemy Self, Vector3 Position)
    {
        Event?.Invoke(Self, Position);
    }
    
    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<Enemy> SelfBlackboardVariable = messageData[0] as BlackboardVariable<Enemy>;
        BlackboardVariable<Vector3> PositionBlackboardVariable = messageData[1] as BlackboardVariable<Vector3>;
        var Self = SelfBlackboardVariable != null ? SelfBlackboardVariable.Value : default(Enemy);
        var Position = PositionBlackboardVariable != null ? PositionBlackboardVariable.Value : default(Vector3);

        Event?.Invoke(Self, Position);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        SoundHeardEventHandler del = (Self, Position) =>
        {
            BlackboardVariable<Enemy> var0 = vars[0] as BlackboardVariable<Enemy>;
            BlackboardVariable<Vector3> var1 = vars[1] as BlackboardVariable<Vector3>;
            if(var0 != null)
                var0.Value = Self;
            if(var1 != null)
                var1.Value = Position;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as SoundHeardEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as SoundHeardEventHandler;
    }
}

