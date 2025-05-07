using Game.Level;
using Ineraction;
using Mirror;
using Networking;
using Snowy.CSharp;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LevelManager : NetworkSingleton<LevelManager>
    {
        [SerializeField] private Door[] doors;
        public event System.Action OnSurfaceUpdated;
        
# if UNITY_EDITOR
        
        [ContextMenu("Gather All Doors")]
        public void GatherAllDoors()
        {
            doors = FindObjectsByType<Door>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            
            for (var i = 0; i < doors.Length; i++)
            {
                doors[i].doorId = i;
                UnityEditor.EditorUtility.SetDirty(doors[i]);
            }
            
            // save the changes
            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif


        public void SendOpenDoor(Door door)
        {
            if (isServer) RpcTriggerDoor(door.doorId, true);
            else CmdTriggerDoor(door.doorId, true);
        }
        
        public void SendCloseDoor(Door door)
        {
            if (isServer) RpcTriggerDoor(door.doorId, false);
            else CmdTriggerDoor(door.doorId, false);
        }
        
        private void OpenDoor(int doorIndex)
        {
            doors[doorIndex].Open();
            LevelManager.Instance.OnSurfaceUpdated?.Invoke();
        }
        
        private void CloseDoor(int doorIndex)
        {
            doors[doorIndex].Close();
            LevelManager.Instance.OnSurfaceUpdated?.Invoke();
        }
        
        [Command(requiresAuthority = false)]
        public void CmdTriggerDoor(int doorIndex, bool open)
        {
            RpcTriggerDoor(doorIndex, open);
        }
        
        [ClientRpc]
        public void RpcTriggerDoor(int doorIndex, bool open)
        {
            if (open)
                OpenDoor(doorIndex);
            else
                CloseDoor(doorIndex);
        }

    }
}