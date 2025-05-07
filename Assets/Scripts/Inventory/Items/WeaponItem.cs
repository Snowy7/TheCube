using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "New Weapon Item", menuName = "Inventory/Items/Weapon Item")]
    public class WeaponItem : ItemData
    {
        public Weapon weaponPrefab;
    }
}