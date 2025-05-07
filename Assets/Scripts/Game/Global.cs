using System.Linq;
using Inventory;
using Snowy.Utils;
using UnityEngine;
using Utilities;

namespace Game
{
    public class Global : MonoSingleton<Global>
    {
        public override bool DestroyOnLoad => false;
        
        [SerializeField] private LayerMask fpViewLayerMask;
        [SerializeField] private LayerMask tpViewLayerMask;
        [SerializeField, InLineEditor] private ShotImpacts shotImpacts;
        [SerializeField, ReorderableList] private WeaponItem[] weaponItems;

        public static int GetFpViewLayerMask => (int)Mathf.Log(Instance.fpViewLayerMask.value, 2);
        public static int GetTpViewLayerMask => (int)Mathf.Log(Instance.tpViewLayerMask.value, 2);
        
        
        public static WeaponItem[] WeaponItems => Instance.weaponItems;
        
        public static Weapon GetWeapon(int weaponId)
        {
            return WeaponItems.FirstOrDefault(x => x.itemID == weaponId)?.weaponPrefab;
        }

        protected override void Awake()
        {
            base.Awake();
            
            // Setup weapons
            for (var index = 0; index < weaponItems.Length; index++)
            {
                var weapon = weaponItems[index];
                weapon.weaponPrefab.ID = weapon.itemID;
            }
        }

        public GameObject GetImpact(GameObject target)
        {
            if (shotImpacts == null) return null;
            Impact imp = shotImpacts.impacts.FirstOrDefault(x => target.CompareTag(x.tag));
            return imp != null ? imp.impact : shotImpacts.impacts[0].impact;
        }
    }
}