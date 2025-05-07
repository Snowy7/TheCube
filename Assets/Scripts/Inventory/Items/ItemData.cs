using Firebase.Game;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Items/Item Data")]
    public class ItemData : ScriptableObject 
    {
        [Header("Item Data")]
        public int itemID;
        public string itemName;
        public string itemDescription;
        public Sprite itemIcon;
        public ItemType itemType = ItemType.Weapon;
        public int price;
    }
}