using System;
using UnityEngine;

namespace New.Player
{
    [Serializable]
    public class WeaponItem : Item
    {
        [SerializeField] private WeaponData weaponData;
        
        public override bool Use()
        {
            // Equip weapon
            // This would typically be handled by the WeaponSystem
            return true;
        }
        
        public WeaponData WeaponData => weaponData;
    }
}