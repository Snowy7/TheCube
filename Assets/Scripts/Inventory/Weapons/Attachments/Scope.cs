﻿using UnityEngine;

namespace Inventory.Attachments
{
    public class Scope : WeaponAttachment
    {
        #region FIELDS SERIALIZED
    
        [Header("Multipliers")]
        
        [Tooltip("Amount to multiply the mouse sensitivity by while aiming through this scope.")]
        [SerializeField]
        private float multiplierMouseSensitivity = 0.8f;
        
        [Tooltip("Value multiplied by the weapon's spread while aiming through this scope.")]
        [SerializeField]
        private float multiplierSpread = 0.1f;
        
        [Header("Sway")]

        [Tooltip("The value to multiply the weapon sway by while aiming through this scope.")]
        [SerializeField]
        private float swayMultiplier = 1.0f;
        
        [Header("Aiming Offset")]
        
        [Tooltip("Weapon bone location offset while aiming.")]
        [SerializeField]
        private Vector3 offsetAimingLocation;
        
        [Tooltip("Weapon bone rotation offset while aiming.")]
        [SerializeField]
        private Vector3 offsetAimingRotation;
        
        [Header("Field Of View")]

        [Tooltip("Field Of View Multiplier Aim.")]
        [SerializeField]
        private float fieldOfViewMultiplierAim = 0.9f;
        
        [Tooltip("Field Of View Multiplier Aim Weapon.")]
        [SerializeField]
        private float fieldOfViewMultiplierAimWeapon = 0.7f;

        [Header("Materials")]

        [Tooltip("The index of the scope material that gets hidden when we don't aim.")]
        [SerializeField]
        private int materialIndex = 3;

        [Tooltip("Material to block the scope while not aiming through it.")]
        [SerializeField]
        private Material materialHidden;
        
        #endregion

        #region FIELDS

        /// <summary>
        /// Mesh Renderer.
        /// </summary>
        private MeshRenderer meshRenderer;
        /// <summary>
        /// Default scope material. We store it so we can re-apply it at any time, since it is
        /// usually changed at runtime.
        /// </summary>
        private Material materialDefault;

        #endregion

        #region UNITY

        /// <summary>
        /// Awake.
        /// </summary>
        private void Awake()
        {
            //Cache Scope Renderer.
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            
            //Make sure that the index can exist.
            if (!HasMaterialIndex())
                return;
            
            //Cache default material.
            materialDefault = meshRenderer.materials[materialIndex];
        }
        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            //Start at the default state.
            OnAimStop();
        }

        #endregion

        #region GETTERS

        /// <summary>
        /// GetMultiplierMouseSensitivity.
        /// </summary>
        public float GetMultiplierMouseSensitivity() => multiplierMouseSensitivity;
        /// <summary>
        /// GetMultiplierSpread.
        /// </summary>
        public float GetMultiplierSpread() => multiplierSpread;

        /// <summary>
        /// GetOffsetAimingLocation.
        /// </summary>
        public Vector3 GetOffsetAimingLocation() => offsetAimingLocation;
        /// <summary>
        /// GetOffsetAimingRotation.
        /// </summary>
        public Vector3 GetOffsetAimingRotation() => offsetAimingRotation;

        /// <summary>
        /// GetFieldOfViewMultiplierAim.
        /// </summary>
        public float GetFieldOfViewMultiplierAim() => fieldOfViewMultiplierAim;
        /// <summary>
        /// GetFieldOfViewMultiplierAimWeapon.
        /// </summary>
        public float GetFieldOfViewMultiplierAimWeapon() => fieldOfViewMultiplierAimWeapon;
        
        /// <summary>
        /// GetSwayMultiplier.
        /// </summary>
        public float GetSwayMultiplier() => swayMultiplier;

        /// <summary>
        /// Returns true if the Scope's Mesh Renderer could have this Material index.
        /// </summary>
        private bool HasMaterialIndex()
        {
            //Null check.
            if (meshRenderer == null)
                return false;
            
            //Make sure that the index can exist.
            return materialIndex < meshRenderer.materials.Length && materialIndex >= 0;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// OnAim.
        /// </summary>
        public void OnAim()
        {
            //Make sure that the index can exist.
            if (!HasMaterialIndex())
                return;

            //Get Materials.
            Material[] materials = meshRenderer.materials;
            //Restore to default material.
            materials[materialIndex] = materialDefault;
            //Update Materials.
            meshRenderer.materials = materials;
        }
        
        /// <summary>
        /// OnAimStop.
        /// </summary>
        public void OnAimStop()
        {
            //Make sure that the index can exist.
            if (!HasMaterialIndex())
                return;
            
            //Get Materials.
            Material[] materials = meshRenderer.materials;
            //Hide.
            materials[materialIndex] = materialHidden;
            //Update Materials.
            meshRenderer.materials = materials;
        }

        #endregion
    }
}