using System.Collections.Generic;
using UnityEngine;

namespace New.Player
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Echoes/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic Info")]
        public string weaponName;
        public GameObject weaponPrefab;
        public Sprite weaponIcon;
        
        [Header("Stats")]
        public float damage = 10f;
        public float fireRate = 1f;
        public float range = 100f;
        public int magazineSize = 30;
        public float reloadTime = 2f;
        public string ammoType;
        
        [Header("Effects")]
        public GameObject muzzleFlash;
        public GameObject hitEffect;
        public AudioClip fireSound;
        public AudioClip reloadSound;
        
        [Header("Recoil")]
        public Vector2 recoilAmount = new Vector2(1f, 2f);
        public float recoilDuration = 0.1f;
        
        [Header("Modifiers")]
        public List<WeaponModifierData> compatibleModifiers;
    }
}