using Game;
using Game.Level;
using UnityEngine;

namespace Ineraction
{
    
    public class DoorKey : Interactable
    {
        [Title("Settings")]
        [SerializeField] private Door door;
        [SerializeField] private bool autoInteractText;
        
        protected override void Awake()
        {
            base.Awake();
            if (door == null)
                Debug.LogError("Door reference is not set in " + gameObject.name);
            
            if (autoInteractText) interactionText = door.IsOpen ? "To Close" : "To Open";
        }

        public override void Interact(Interactor actor = null)
        {
            if (door.IsOpen)
                LevelManager.Instance.SendCloseDoor(door);
            else
                LevelManager.Instance.SendOpenDoor(door);
            
            if (autoInteractText) interactionText = door.IsOpen ? "To Close" : "To Open";
        }
    }
}