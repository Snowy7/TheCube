

using UnityEngine;

namespace Actors.Player
{
    /// <summary>
    /// Feel. This object contains basically all of the data relating to how weapons feel according to procedural motions.
    /// </summary>
    [CreateAssetMenu(fileName = "SO_Feel", menuName = "Infima Games/Low Poly Shooter Pack/Feel", order = 0)]
    public class Feel : ScriptableObject
    {
        #region PROPERTIES

        /// <summary>
        /// Standing.
        /// </summary>
        public FeelState Standing => standing;
        /// <summary>
        /// Crouching.
        /// </summary>
        public FeelState Crouching => crouching;
        /// <summary>
        /// Aiming.
        /// </summary>
        public FeelState Aiming => aiming;
        /// <summary>
        /// Running.
        /// </summary>
        public FeelState Running => running;
        
        #endregion
        
        #region FIELDS SERIALIZED

        [Title("Standing State")]
        
        [Tooltip("FeelState used while just standing around.")]
        [SerializeField]
        private FeelState standing;
        
        [Title("Crouching State")]
        
        [Tooltip("FeelState used while crouching.")]
        [SerializeField]
        private FeelState crouching;
        
        [Title("Aiming State")]

        [Tooltip("FeelState used while aiming.")]
        [SerializeField]
        private FeelState aiming;
        
        [Title("Running State")]

        [Tooltip("FeelState used while running.")]
        [SerializeField]
        private FeelState running;
        
        #endregion
        
        #region FUNCTIONS

        /// <summary>
        /// Returns the FeelState that should be used based on the character's current Animator's component values.
        /// </summary>
        public FeelState GetState(Animator characterAnimator)
        {
            //Running.
            if (characterAnimator.GetBool(AHashes.Running))
                return Running;
            //Aiming.
            if (characterAnimator.GetBool(AHashes.Aim))
                return Aiming;
            //Crouching.
            if (characterAnimator.GetBool(AHashes.Crouching))
                return Crouching;
            //Standing.
            return Standing;
        }
        
        #endregion
    }
}