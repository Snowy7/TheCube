using UnityEngine;
using System;
using System.Collections.Generic;

namespace New.Player
{
    public class InventorySystem : MonoBehaviour
    {
        [Header("Inventory Settings")]
        [SerializeField] private int maxItems = 20;
        [SerializeField] private float maxWeight = 50f;
        
        private List<Item> items = new List<Item>();
        private float currentWeight = 0f;
        
        // Events
        public event Action<Item> OnItemAdded;
        public event Action<Item> OnItemRemoved;
        public event Action<Item> OnItemUsed;
        public event Action OnInventoryChanged;
        
        public bool AddItem(Item item)
        {
            if (items.Count >= maxItems || currentWeight + item.Weight > maxWeight)
            {
                Debug.Log("Inventory full or too heavy");
                return false;
            }
            
            // Check if item is stackable and if we already have some
            if (item.IsStackable)
            {
                Item existingItem = items.Find(i => i.ID == item.ID);
                if (existingItem != null)
                {
                    existingItem.StackCount += item.StackCount;
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
            
            // Add new item
            items.Add(item);
            currentWeight += item.Weight;
            
            // Notify listeners
            OnItemAdded?.Invoke(item);
            OnInventoryChanged?.Invoke();
            
            return true;
        }
        
        public bool RemoveItem(Item item)
        {
            if (!items.Contains(item))
            {
                return false;
            }
            
            if (item.IsStackable && item.StackCount > 1)
            {
                item.StackCount--;
            }
            else
            {
                items.Remove(item);
                currentWeight -= item.Weight;
            }
            
            // Notify listeners
            OnItemRemoved?.Invoke(item);
            OnInventoryChanged?.Invoke();
            
            return true;
        }
        
        public bool RemoveItemByID(string itemID, int count = 1)
        {
            Item item = items.Find(i => i.ID == itemID);
            if (item == null)
            {
                return false;
            }
            
            if (item.IsStackable && item.StackCount > count)
            {
                item.StackCount -= count;
            }
            else
            {
                items.Remove(item);
                currentWeight -= item.Weight;
            }
            
            // Notify listeners
            OnItemRemoved?.Invoke(item);
            OnInventoryChanged?.Invoke();
            
            return true;
        }
        
        public bool UseItem(Item item)
        {
            if (!items.Contains(item))
            {
                return false;
            }
            
            bool itemUsed = item.Use();
            
            if (itemUsed)
            {
                OnItemUsed?.Invoke(item);
                
                // Check if item should be consumed
                if (item.ConsumeOnUse)
                {
                    RemoveItem(item);
                }
            }
            
            return itemUsed;
        }
        
        public int GetItemCount(string itemID)
        {
            int count = 0;
            foreach (var item in items)
            {
                if (item.ID == itemID)
                {
                    count += item.StackCount;
                }
            }
            return count;
        }
        
        public Item GetItem(string itemID)
        {
            return items.Find(i => i.ID == itemID);
        }
        
        public List<Item> GetAllItems()
        {
            return new List<Item>(items);
        }
        
        public List<T> GetItemsOfType<T>() where T : Item
        {
            List<T> result = new List<T>();
            foreach (var item in items)
            {
                if (item is T typedItem)
                {
                    result.Add(typedItem);
                }
            }
            return result;
        }
        
        // Properties
        public int ItemCount => items.Count;
        public float CurrentWeight => currentWeight;
        public float MaxWeight => maxWeight;
        public int MaxItems => maxItems;
        public float WeightPercentage => currentWeight / maxWeight;
    }
}
