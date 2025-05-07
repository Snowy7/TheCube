using Game.Level;
using Snowy.Engine;
using UnityEngine;

namespace Game.WorldSystem
{
    public class AutoDoorOpener : MonoBehaviour
    {
        [SerializeField] private LayerMask doorMask;
        
        [SerializeField, Disable] private Door entranceDoor;

        private void OnTriggerEnter(Collider other)
        {
            if (doorMask.HasLayer(other.gameObject.layer))
            {
                var door = other.GetComponent<Door>();
                
                if (door && !door.IsOpen)
                    LevelManager.Instance.SendOpenDoor(door);
                
                entranceDoor = door;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (doorMask.HasLayer(other.gameObject.layer))
            {
                if (entranceDoor)
                    LevelManager.Instance.SendCloseDoor(entranceDoor);
                
                entranceDoor = null;
            }
        }
    }
}