using UnityEngine;

namespace Inventory.Attachments
{
    public class Magazine : WeaponAttachment
    {
        #region FIELDS SERIALIZED

        [Header("Settings")]
        
        [Tooltip("Total Ammunition.")]
        [SerializeField]
        private int ammunitionTotal = 10;

        #endregion

        #region GETTERS

        /// <summary>
        /// Ammunition Total.
        /// </summary>
        public int GetAmmunitionTotal() => ammunitionTotal;

        #endregion
    }
}