using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace New.Player
{
    public class WeaponSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform weaponSocket;
        [SerializeField] private Transform muzzlePoint;
        
        [Header("Settings")]
        [SerializeField] private WeaponItem startingWeapon;
        [SerializeField] private float weaponSwitchTime = 0.5f;
        
        private InventorySystem inventorySystem;
        private CameraSystem cameraSystem;
        
        private WeaponItem currentWeapon;
        private List<WeaponItem> availableWeapons = new List<WeaponItem>();
        private int currentWeaponIndex = 0;
        
        private GameObject activeWeaponObject;
        private bool isReloading = false;
        private bool isSwitchingWeapon = false;
        private float lastFireTime;
        
        // Events
        public event Action<WeaponItem> OnWeaponEquipped;
        public event Action OnWeaponFired;
        public event Action OnReloadStarted;
        public event Action OnReloadFinished;
        
        private void Start()
        {
            // Add starting weapon if set
            if (startingWeapon != null)
            {
                availableWeapons.Add(startingWeapon);
                EquipWeapon(startingWeapon);
            }
        }
        
        public void Initialize(InventorySystem inventory, CameraSystem camera)
        {
            inventorySystem = inventory;
            cameraSystem = camera;
            
            // Subscribe to inventory events to track weapon items
            inventorySystem.OnItemAdded += HandleItemAdded;
            inventorySystem.OnItemRemoved += HandleItemRemoved;
        }
        
        private void HandleItemAdded(Item item)
        {
            if (item is WeaponItem weaponItem)
            {
                if (!availableWeapons.Contains(weaponItem))
                {
                    availableWeapons.Add(weaponItem);
                }
            }
        }
        
        private void HandleItemRemoved(Item item)
        {
            if (item is WeaponItem weaponItem)
            {
                availableWeapons.Remove(weaponItem);
                
                // If the removed weapon was equipped, switch to another weapon
                if (currentWeapon == weaponItem)
                {
                    if (availableWeapons.Count > 0)
                    {
                        SwitchWeapon(1); // Switch to next weapon
                    }
                    else
                    {
                        UnequipCurrentWeapon();
                    }
                }
            }
        }
        
        public void SwitchWeapon(int direction)
        {
            if (isSwitchingWeapon || availableWeapons.Count <= 1) return;
            
            currentWeaponIndex = (currentWeaponIndex + direction + availableWeapons.Count) % availableWeapons.Count;
            WeaponItem nextWeapon = availableWeapons[currentWeaponIndex];
            
            StartCoroutine(SwitchWeaponRoutine(nextWeapon));
        }
        
        private IEnumerator SwitchWeaponRoutine(WeaponItem newWeapon)
        {
            isSwitchingWeapon = true;
            
            // Lower current weapon
            if (activeWeaponObject != null)
            {
                // Play animation or effect for lowering weapon
                yield return new WaitForSeconds(weaponSwitchTime / 2);
                Destroy(activeWeaponObject);
            }
            
            // Equip new weapon
            EquipWeapon(newWeapon);
            
            // Play animation or effect for raising weapon
            yield return new WaitForSeconds(weaponSwitchTime / 2);
            
            isSwitchingWeapon = false;
        }
        
        private void EquipWeapon(WeaponItem weaponItem)
        {
            if (weaponItem == null || weaponItem.WeaponData == null) return;
            
            currentWeapon = weaponItem;
            
            // Instantiate weapon model
            if (weaponItem.WeaponData.weaponPrefab != null)
            {
                activeWeaponObject = Instantiate(weaponItem.WeaponData.weaponPrefab, weaponSocket);
                activeWeaponObject.transform.localPosition = Vector3.zero;
                activeWeaponObject.transform.localRotation = Quaternion.identity;
                
                // Find muzzle point in the weapon prefab if available
                Transform weaponMuzzle = activeWeaponObject.transform.Find("MuzzlePoint");
                if (weaponMuzzle != null)
                {
                    muzzlePoint = weaponMuzzle;
                }
            }
            
            OnWeaponEquipped?.Invoke(weaponItem);
        }
        
        private void UnequipCurrentWeapon()
        {
            if (activeWeaponObject != null)
            {
                Destroy(activeWeaponObject);
                activeWeaponObject = null;
            }
            
            currentWeapon = null;
        }
        
        public void Fire()
        {
            if (currentWeapon == null || isReloading || isSwitchingWeapon) return;
            
            WeaponData weaponData = currentWeapon.WeaponData;
            
            // Check fire rate
            if (Time.time - lastFireTime < 1f / weaponData.fireRate) return;
            
            // Check ammo
            int currentAmmo = inventorySystem.GetItemCount(weaponData.ammoType);
            if (currentAmmo <= 0)
            {
                // Click sound or feedback for empty weapon
                Reload(); // Auto-attempt reload
                return;
            }
            
            // Consume ammo
            inventorySystem.RemoveItemByID(weaponData.ammoType, 1);
            
            // Fire the weapon
            lastFireTime = Time.time;
            
            // Apply camera recoil
            if (cameraSystem != null)
            {
                cameraSystem.ApplyRecoil(weaponData.recoilAmount, weaponData.recoilDuration);
            }
            
            // Play sound
            if (weaponData.fireSound != null)
            {
                AudioSource.PlayClipAtPoint(weaponData.fireSound, transform.position);
            }
            
            // Show muzzle flash
            if (weaponData.muzzleFlash != null && muzzlePoint != null)
            {
                GameObject flash = Instantiate(weaponData.muzzleFlash, muzzlePoint.position, muzzlePoint.rotation);
                Destroy(flash, 0.1f);
            }
            
            // Perform raycast for hit detection
            RaycastHit hit;
            if (Physics.Raycast(cameraSystem.transform.position, cameraSystem.transform.forward, out hit, weaponData.range))
            {
                // Apply damage if hit something damageable
                HealthSystem targetHealth = hit.collider.GetComponent<HealthSystem>();
                if (targetHealth != null)
                {
                    // Create damage info
                    DamageInfo damageInfo = new DamageInfo(
                        weaponData.damage,
                        DamageType.Physical,
                        hit.point,
                        -hit.normal,
                        gameObject
                    );
                    
                    targetHealth.TakeDamage(damageInfo);
                }
                
                // Show hit effect
                if (weaponData.hitEffect != null)
                {
                    GameObject hitVfx = Instantiate(weaponData.hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(hitVfx, 2f);
                }
            }
            
            OnWeaponFired?.Invoke();
        }
        
        public void Reload()
        {
            if (currentWeapon == null || isReloading || isSwitchingWeapon) return;
            
            // Check if we have ammo
            string ammoType = currentWeapon.WeaponData.ammoType;
            if (inventorySystem.GetItemCount(ammoType) <= 0) return;
            
            StartCoroutine(ReloadRoutine());
        }
        
        private IEnumerator ReloadRoutine()
        {
            isReloading = true;
            OnReloadStarted?.Invoke();
            
            // Play reload sound
            if (currentWeapon.WeaponData.reloadSound != null)
            {
                AudioSource.PlayClipAtPoint(currentWeapon.WeaponData.reloadSound, transform.position);
            }
            
            // Wait for reload time
            yield return new WaitForSeconds(currentWeapon.WeaponData.reloadTime);
            
            isReloading = false;
            OnReloadFinished?.Invoke();
        }
        
        // Properties
        public WeaponItem CurrentWeapon => currentWeapon;
        public bool IsReloading => isReloading;
        public bool IsSwitchingWeapon => isSwitchingWeapon;
        
        // Get current ammo count for UI display
        public int GetCurrentAmmo()
        {
            if (currentWeapon == null) return 0;
            return inventorySystem.GetItemCount(currentWeapon.WeaponData.ammoType);
        }
    }
}
