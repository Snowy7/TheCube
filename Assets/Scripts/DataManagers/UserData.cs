using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using JetBrains.Annotations;

namespace Firebase.Game
{
    [FirestoreData]
    public class UserData : FirestoreDocument<UserData>
    {
        [FirestoreProperty]
        public string Username { get; set; }
        [FirestoreProperty]
        public int Level { get; set; }
        [FirestoreProperty]
        public int Experience { get; set; }
        [FirestoreProperty]
        public string TimeToLive { get; set; }
        [FirestoreProperty]
        public string LastLogin { get; set; }
        
        [FirestoreProperty]
        public DocumentReference InventoryRef { get; set; }
        
        public PrivateStorage PrivateStorage { get; set; }
        
        
        public UserData() { }
        
        public UserData(DocumentSnapshot document) : base(document)
        {
            UpdateWithSnapshot(document);
        }
        
        public override async Task<bool> Load()
        {
            // load the inventory
            if (InventoryRef != null)
            {
                await FirestoreManager.Instance.GetDocument<PrivateStorage>(InventoryRef, OnLoadInventory);
            }
            
            return true;
        }

        public sealed override void UpdateWithSnapshot(DocumentSnapshot document)
        {
            Username = document.GetValue<string>("Username");
            Level = document.GetValue<int>("Level");
            Experience = document.GetValue<int>("Experience");
            TimeToLive = document.GetValue<string>("TimeToLive");
            LastLogin = document.GetValue<string>("LastLogin");
            InventoryRef = document.GetValue<DocumentReference>("InventoryRef");

            base.UpdateWithSnapshot(document);
        }

        public void OnLoadInventory(PrivateStorage privateStorage)
        {
            PrivateStorage = privateStorage;
        }
        
        public override string ToString()
        {
            return $"Username: {Username}, Level: {Level}, Experience: {Experience}, TimeToLive: {TimeToLive}, LastLogin: {LastLogin}";
        }
    }
    
    // Player Inventory
    [FirestoreData]
    public class PrivateStorage : FirestoreDocument<PrivateStorage>
    {
        [FirestoreProperty]
        public StorageItem[] Items { get; set; }
        
        public PrivateStorage() { }
        
        public PrivateStorage(DocumentSnapshot document) : base(document)
        {
            Items = document.GetValue<StorageItem[]>("Items");
        }
        
        public override string ToString()
        {
            return $"Items: {Items.Length}";
        }
        
        public void AddItem(StorageItem item)
        {
            var list = Items.ToList();
            list.Add(item);
            Items = list.ToArray();
        }

        public void RemoveItem(StorageItem item)
        {
            var list = Items.ToList();
            list.Remove(item);
            Items = list.ToArray();
        }
        
        public void AddRange(StorageItem[] items)
        {
            var list = Items.ToList();
            list.AddRange(items);
            Items = list.ToArray();
        }
        
        public StorageItem GetItem(int id)
        {
            return Items.FirstOrDefault(i => i.Id == id);
        }
        
        [CanBeNull]
        public StorageItem GetItem(int id, ItemType type)
        {
            return Items.FirstOrDefault(i => i.Id == id && i.Type == (int)type);
        }
        
        public StorageItem UpdateItem(StorageItem item)
        {
            var index = Items.ToList().FindIndex(i => i.Id == item.Id);
            Items[index] = item;
            return Items[index];
        }
        
        public bool ContainsItem(int id, int type = (int)ItemType.Weapon)
        {
            return Items.Any(i => i.Id == id && i.Type == type);
        }

        public StorageItem[] GetItems(ItemType type)
        {
            return Items.Where(i => i.Type == (int)type).ToArray();
        }
    }
    
    public enum ItemType
    {
        Weapon = 0,
        Armor = 1,
        Skin = 2,
    }

    [FirestoreData, Serializable]
    public class StorageItem : IEquatable<StorageItem>
    {
        [FirestoreProperty]
        public int Type { get; set; }
        [FirestoreProperty]
        public int Id { get; set; }
        [FirestoreProperty]
        public string Metadata { get; set; }
        [FirestoreProperty]
        public int Quantity { get; set; }

        /// <summary>
        /// Get the custom data from the metadata.
        /// </summary>
        /// <returns></returns>
        public int[] CustomData()
        {
            return Metadata.Split(':').Select(int.Parse).ToArray();
        }
        
        /// <summary>
        /// Add Custom Data to the metadata.
        /// </summary>
        /// <returns></returns>
        public void AddCustomData(int[] data)
        {
            Metadata = string.Join(":", data);
        }
        
        /// <summary>
        /// Set the custom data to the metadata.
        /// </summary>
        /// <returns></returns>
        public void SetCustomData(int[] data)
        {
            Metadata = string.Join(":", data);
        }
        
        public ItemType GetItemType()
        {
            return (ItemType)Type;
        }
        
        public override string ToString()
        {
            return $"Type: {GetItemType()}, Id: {Id}, Metadata: {Metadata}, Quantity: {Quantity}";
        }

        public bool Equals(StorageItem other)
        {
            return Type == other.Type && Id == other.Id && Metadata == other.Metadata && Quantity == other.Quantity;
        }

        public override bool Equals(object obj)
        {
            return obj is StorageItem other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Id, Metadata, Quantity);
        }
    }

    // Item in the inventory
}