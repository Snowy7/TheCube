using UnityEngine;

namespace Interface.Elements
{
    public class WeaponUI : Element
    {
        [SerializeField] private WeaponSlotUI primaryWeaponSlot;
        [SerializeField] private WeaponSlotUI secondaryWeaponSlot;

        public override void Tick()
        {
            base.Tick();
            if (character == null) return;
            if (character.GetInventory() == null)
            {
                primaryWeaponSlot.gameObject.SetActive(false);
                secondaryWeaponSlot.gameObject.SetActive(false);
                return;
            }
            
            // disable if no weapons
            if (character.GetInventory().GetWeapons().Count == 0)
            {
                primaryWeaponSlot.gameObject.SetActive(false);
                secondaryWeaponSlot.gameObject.SetActive(false);
                return;
            }
            
            primaryWeaponSlot.gameObject.SetActive(true);
            
            // primary is the equipped weapon
            var primaryWeapon = character.GetInventory().GetEquipped();
            primaryWeaponSlot.Tick(primaryWeapon);
        }
    }
}