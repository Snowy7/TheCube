using System;
using System.Collections.Generic;
using Actors.Player;
using DataManagers.Presets;
using Firebase.Game;
using Interface;
using Networking;
using SnInput;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ineraction
{
    public class PresetSetterInteractable : Interactable
    {
        [SerializeField] private Transform visualSpawnPoint;
        [SerializeField] private Transform cameraFocus;
        [SerializeField] private GameObject controlUI;
        [SerializeField] private bool isPrimary;
        [SerializeField, ReorderableList, Disable] private List<StorageItem> weapons;
        
        private bool m_isActive;
     
        void OnEnable()
        {
            controlUI.SetActive(false);
            PresetManager.Instance.OnPresetChanged += OnPresetChanged;
        }

        private void OnDisable()
        {
            if (PresetManager.Instance) PresetManager.Instance.OnPresetChanged -= OnPresetChanged;
            InputManager.OnLockCursor -= OnLockCursor;
        }

        private void OnDestroy()
        {
            if (PresetManager.Instance) PresetManager.Instance.OnPresetChanged -= OnPresetChanged;
            InputManager.OnLockCursor -= OnLockCursor;
        }

        public override void Interact(Interactor actor = null)
        {
            controlUI.SetActive(true);
            
            // Zoom camera in and disable player movement
            Player player = ClientsManager.Instance.LocalClient.Player;
            
            if (!player) return;
            
            player.Character.LockCameraAt(cameraFocus);
            
            m_isActive = true;
            
            player.OnLockCursor(false);
            // Subscribe to the dismount event
            MenuManager.Instance.CanToggleMenu = false;
            InputManager.Instance.SwitchToUIControls();
            InputManager.OnLockCursor += OnLockCursor;
        }
        
        private void OnLockCursor(InputAction.CallbackContext context)
        {
            if (!m_isActive) return;
            
            if (!ClientsManager.Instance) return;
            
            // Zoom camera out and enable player movement
            Player player = ClientsManager.Instance.LocalClient.Player;
            
            if (!player) return;
            
            player.Character.UnlockCamera();
            
            m_isActive = false;
            
            controlUI.SetActive(false);
            
            player.OnLockCursor(true);
            MenuManager.Instance.CanToggleMenu = true;
            InputManager.Instance.SwitchToGameControls();
            // Unsubscribe from the dismount event
            InputManager.OnLockCursor -= OnLockCursor;
        }
        
        private void OnPresetChanged(Preset preset)
        {
            // Make sure you cannot use the same weapon twice
            weapons = new List<StorageItem>(UserItems.Instance.GetItems(ItemType.Weapon));
            weapons.RemoveAll(w => w.Id == (isPrimary ? preset.SecondaryWeaponId : preset.PrimaryWeaponId));
        }
    }
}