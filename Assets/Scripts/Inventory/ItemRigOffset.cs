using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Inventory
{
    public class ItemRigOffset : MonoBehaviour
    {
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private MultiAimConstraint rig;
        [SerializeField] private Vector3 equipOffset;
        [SerializeField] private Vector3 unequipOffset;
        
        private void Start()
        {
            if (!inventory) inventory = GetComponentInParent<PlayerInventory>();
            
            rig.data.offset = unequipOffset;
            
            inventory.OnEquip += Equip;
        }
        
        private void Equip(int index)
        {
            if (index != -1) return;
            
            rig.data.offset = equipOffset;
        }
        
        private void OnDestroy()
        {
            inventory.OnEquip -= Equip;
        }
    }
}