using Inventory;
using Networking;
using UnityEngine;

namespace Ineraction
{
    enum PickupMode
    {
        One,
        Multiple,
        Infinite
    }
    
    enum OnPickup
    {
        Destroy,
        Disable
    }
    
    public class PickupInteractable : Interactable
    {
        [SerializeField] PickupMode pickupMode = PickupMode.Infinite;
        [SerializeField, ShowIf(nameof(pickupMode), PickupMode.Multiple)] int maxPickups;
        [SerializeField, HideIf(nameof(pickupMode), PickupMode.Infinite)] OnPickup onPickup;
        [SerializeField, HideIf(nameof(pickupMode), PickupMode.Infinite)] GameObject[] visualObjects;
        
        [SerializeField] private WeaponItem item;
        
        public override void Interact(Interactor actor = null)
        {
            var player = ClientsManager.Instance.LocalClient.Player;
            if (player == null)
            {
                Debug.LogError("Player is null");
                return;
            }

            if (player.IsDead)
            {
                Debug.LogError("Player is dead");
                return;
            }

            if (player.Character == null)
            {
                Debug.LogError("Player character is null");
                return;
            }

            if (player.Character.GetInventory() == null)
            {
                Debug.LogError("Player inventory is null");
                return;
            }
            
            // Add item to inventory
            player.Character.GetInventory().AddWeapon(item.itemID);

            switch (pickupMode)
            {
                case PickupMode.One:
                    OnPickedup();
                    break;
                case PickupMode.Multiple:
                    maxPickups--;
                    if (maxPickups <= 0) OnPickedup();
                    break;
                case PickupMode.Infinite:
                    break;
            }
        }
        
        private void OnPickedup()
        {
            switch (onPickup)
            {
                case OnPickup.Destroy:
                    foreach (var visualObject in visualObjects)
                    {
                        Destroy(visualObject);
                    }
                    break;
                case OnPickup.Disable:
                    foreach (var visualObject in visualObjects)
                    {
                        visualObject.SetActive(false);
                    }
                    break;
            }
        }
    }
}