using UnityEngine;

namespace New.Player
{
    [CreateAssetMenu(fileName = "WeaponModifier", menuName = "Echoes/Weapon Modifier")]
    public class WeaponModifierData : ScriptableObject
    {
        public string modifierName;
        public Sprite modifierIcon;
        public GameObject modifierPrefab;
        
        [Header("Stat Modifications")]
        public float damageMultiplier = 1f;
        public float rangeMultiplier = 1f;
        public float fireRateMultiplier = 1f;
        public float reloadTimeMultiplier = 1f;
        public int magazineSizeBonus = 0;
        
        [Header("Special Effects")]
        public bool addSilencer = false;
        public bool addElementalDamage = false;
        public DamageType elementalDamageType;
        public float elementalDamageAmount = 0f;
    }
}