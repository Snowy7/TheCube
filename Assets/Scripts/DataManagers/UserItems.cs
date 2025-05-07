using System;
using System.Collections;
using System.Threading.Tasks;
using Inventory;
using Snowy.Utils;
using UnityEngine;

namespace Firebase.Game
{
    public class UserItems : MonoSingleton<UserItems>
    {
        public bool IsReady { get; set; }
        
        private void Start()
        {
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            yield return new WaitUntil(() => UserController.Instance?.IsReady == true);
        }
        
        public bool HasItem(int itemId, ItemType type = ItemType.Weapon)
        {
            return UserController.Instance.UserData.PrivateStorage.GetItem(itemId, type) != null;
        }

        // Get all items of a specific type
        public StorageItem[] GetItems(ItemType type)
        {
            return UserController.Instance.UserData.PrivateStorage.GetItems(type);
        }
        
        public async Task<bool> BuyItem(ItemData item, int amount = 1)
        {
            // Check if the amount of hours left is enough to buy the item
            if (!EconomyHelper.HasEnoughBalance(item.price))
            {
                Debug.Log("Not enough hours left to buy this item.");
                return false;
            }
            
            // Add the item to the user's inventory
            var newTime = EconomyHelper.SubtractBalance(item.price);
            
            // Save the new time to the database
            await UserController.Instance.UpdateTimeToLive(newTime);
            
            // Add the item to the user's inventory
            await UserController.Instance.AddItemToInventory(new StorageItem()
            {
                Id = item.itemID,
                Type = (int)item.itemType,
                Quantity = amount
            });
            
            return true;
        }
    }
}