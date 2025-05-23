﻿

using Snowy;
using UnityEngine;

namespace Actors.Player
{
    /// <summary>
    /// LowerData. Contains all the information needed by a character to lower its weapon with some nice
    /// motion, and nice offsets.
    /// </summary>
    [CreateAssetMenu(fileName = "SO_Lower_Name", menuName = "Infima Games/Low Poly Shooter Pack/Lower Data", order = 0)]
    public class LowerData : ScriptableObject
    {
        #region PROPERTIES
        
        /// <summary>
        /// Interpolation.
        /// </summary>
        public SpringSettings Interpolation => interpolation;

        /// <summary>
        /// LocationOffset.
        /// </summary>
        public Vector3 LocationOffset => locationOffset;
        /// <summary>
        /// RotationOffset.
        /// </summary>
        public Vector3 RotationOffset => rotationOffset;
        
        #endregion
        
        #region FIELDS SERIALIZED

        [Title("Interpolation")]

        [Tooltip("Interpolation settings.")]
        [SerializeField]
        private SpringSettings interpolation = SpringSettings.Default();

        [Title("Offsets")]

        [Tooltip("Location offset applied in the lowered state.")]
        [SerializeField]
        private Vector3 locationOffset;

        [Tooltip("Rotation offset applied in the lowered state.")]
        [SerializeField]
        private Vector3 rotationOffset;

        #endregion
    }
}