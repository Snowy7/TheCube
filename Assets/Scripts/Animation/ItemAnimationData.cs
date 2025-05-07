using Actors.Player;
using UnityEngine;

namespace Actors.Player
{
    /// <summary>
    /// ItemAnimationData. Stores all information related to the weapon-specific procedural data.
    /// </summary>
    public class ItemAnimationData : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Item Offsets")]

        [Tooltip("The object that contains all offset data for this item.")]
        [SerializeField, InLineEditor]
        private ItemOffsets itemOffsets;
        
        [Header("Lowered Data")]

        [Tooltip("This object contains all the data needed for us to set the lowered pose of this weapon.")]
        [SerializeField, InLineEditor]
        private LowerData lowerData;

        [Header("Leaning Data")]

        [Tooltip("LeaningData. Contains all the information on what this weapon should do while the character is leaning.")]
        [SerializeField, InLineEditor]
        private LeaningData leaningData;
        
        [Header("Camera Recoil Data")]

        [Tooltip("Weapon Recoil Data Asset. Used to get some camera recoil values, usually for weapons.")]
        [SerializeField, InLineEditor]
        private RecoilData cameraRecoilData;
        
        [Header("Weapon Recoil Data")]

        [Tooltip("Weapon Recoil Data Asset. Used to get some recoil values, usually for weapons.")]
        [SerializeField, InLineEditor]
        private RecoilData weaponRecoilData;

        #endregion
        
        #region GETTERS

        /// <summary>
        /// GetCameraRecoilData.
        /// </summary>
        public RecoilData GetCameraRecoilData() => cameraRecoilData;
        /// <summary>
        /// GetWeaponRecoilData.
        /// </summary>
        public RecoilData GetWeaponRecoilData() => weaponRecoilData;

        /// <summary>
        /// GetRecoilData.
        /// </summary>
        public RecoilData GetRecoilData(MotionType motionType) =>
            motionType == MotionType.Item ? GetWeaponRecoilData() : GetCameraRecoilData();

        /// <summary>
        /// GetLowerData.
        /// </summary>
        public LowerData GetLowerData() => lowerData;
        /// <summary>
        /// GetLeaningData.
        /// </summary>
        public LeaningData GetLeaningData() => leaningData;
        
        /// <summary>
        /// GetItemOffsets.
        /// </summary>
        public ItemOffsets GetItemOffsets() => itemOffsets;

        #endregion
    }   
}