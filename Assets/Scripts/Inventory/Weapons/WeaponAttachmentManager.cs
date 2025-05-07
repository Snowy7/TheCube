using Inventory.Attachments;
using Actors.Player;
using UnityEngine;
using Utilities;

namespace Inventory
{
    [RequireComponent(typeof(Weapon))]
    public class WeaponAttachmentManager : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Scope")]

        [Tooltip("Determines if the ironsights should be shown on the weapon model.")]
        [SerializeField]
        private bool scopeDefaultShow = true;
        
        [Tooltip("Default Scope!")]
        [SerializeField]
        private Scope scopeDefaultBehaviour;

        [Tooltip("Selected Scope Index. If you set this to a negative number, ironsights will be selected as the enabled scope.")]
        [SerializeField]
        private int scopeIndex = -1;

        [Tooltip("First scope index when using random scopes.")]
        [SerializeField]
        private int scopeIndexFirst = -1;
        
        [Tooltip("Should we pick a random index when starting the game?")]
        [SerializeField]
        private bool scopeIndexRandom;

        [SerializeField]
        private Transform scopeSocket;
        
        [Tooltip("All possible Scope Attachments that this Weapon can use!")]
        [SerializeField]
        private Scope[] scopeArray;
        
        [Header("Muzzle")]

        [Tooltip("Selected Muzzle Index.")]
        [SerializeField]
        private int muzzleIndex;
        
        [Tooltip("Should we pick a random index when starting the game?")]
        [SerializeField]
        private bool muzzleIndexRandom = true;

        [SerializeField]
        private Transform muzzleSocket;
        
        [Tooltip("All possible Muzzle Attachments that this Weapon can use!")]
        [SerializeField]
        private Muzzle[] muzzleArray;
        
        [Header("Laser")]

        [Tooltip("Selected Laser Index.")]
        [SerializeField]
        private int laserIndex = -1;
        
        [Tooltip("Should we pick a random index when starting the game?")]
        [SerializeField]
        private bool laserIndexRandom = true;
        
        [SerializeField]
        private Transform laserSocket;

        [Tooltip("All possible Laser Attachments that this Weapon can use!")]
        [SerializeField]
        private Laser[] laserArray;
        
        [Header("Grip")]

        [Tooltip("Selected Grip Index.")]
        [SerializeField]
        private int gripIndex = -1;
        
        [Tooltip("Should we pick a random index when starting the game?")]
        [SerializeField]
        private bool gripIndexRandom = true;

        [SerializeField]
        private Transform gripSocket;
        
        [Tooltip("All possible Grip Attachments that this Weapon can use!")]
        [SerializeField]
        private Grip[] gripArray;
        
        [Header("Magazine")]

        [Tooltip("Selected Magazine Index.")]
        [SerializeField]
        private int magazineIndex;
        
        [Tooltip("Should we pick a random index when starting the game?")]
        [SerializeField]
        private bool magazineIndexRandom = true;
        
        [SerializeField]
        private Transform magazineSocket;

        [Tooltip("All possible Magazine Attachments that this Weapon can use!")]
        [SerializeField]
        private Magazine[] magazineArray;

        #endregion
        
        #region FIELDS

        /// <summary>
        /// Equipped Scope.
        /// </summary>
        private Scope scopeBehaviour;
        /// <summary>
        /// Equipped Muzzle.
        /// </summary>
        private Muzzle muzzleBehaviour;
        /// <summary>
        /// Equipped Laser.
        /// </summary>
        private Laser laserBehaviour; 
        /// <summary>
        /// Equipped Grip.
        /// </summary>
        private Grip gripBehaviour;
        /// <summary>
        /// Equipped Magazine.
        /// </summary>
        private Magazine magazineBehaviour;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Awake.
        /// </summary>
        protected void Awake()
        {
            //Randomize. This allows us to spice things up a little!
            if (scopeIndexRandom)
                scopeIndex = Random.Range(scopeIndexFirst, scopeArray.Length);
            //Select Scope!
            scopeBehaviour = scopeArray.SelectAndInstantiate(scopeIndex, scopeSocket);
            //Check if we have no scope. This could happen if we have an incorrect index.
            if (scopeBehaviour == null && scopeDefaultBehaviour != null && scopeIndex < 0)
            {
                //Select Default Scope.
                scopeBehaviour = scopeDefaultBehaviour;
                //Set Active.
                scopeBehaviour.gameObject.SetActive(scopeDefaultShow);
            }
            
            //Randomize. This allows us to spice things up a little!
            if (muzzleIndexRandom)
                muzzleIndex = Random.Range(0, muzzleArray.Length);
            //Select Muzzle!
            muzzleBehaviour = muzzleArray.SelectAndInstantiate(muzzleIndex, muzzleSocket);

            //Randomize. This allows us to spice things up a little!
            if (laserIndexRandom)
                laserIndex = Random.Range(0, laserArray.Length);
            //Select Laser!
            laserBehaviour = laserArray.SelectAndInstantiate(laserIndex, laserSocket);
            
            //Randomize. This allows us to spice things up a little!
            if (gripIndexRandom)
                gripIndex = Random.Range(0, gripArray.Length);
            //Select Grip!
            gripBehaviour = gripArray.SelectAndInstantiate(gripIndex, gripSocket);
            
            //Randomize. This allows us to spice things up a little!
            if (magazineIndexRandom)
                magazineIndex = Random.Range(0, magazineArray.Length);
            //Select Magazine!
            magazineBehaviour = magazineArray.SelectAndInstantiate(magazineIndex, magazineSocket);
        }        

        #endregion

        #region GETTERS

        public Scope  GetScope() => scopeBehaviour;
        public Scope  GetScopeDefault() => scopeDefaultBehaviour;

        public Magazine  GetMagazine() => magazineBehaviour;
        public Muzzle  GetMuzzle() => muzzleBehaviour;

        public Laser  GetLaser() => laserBehaviour;
        public Grip  GetGrip() => gripBehaviour;

        #endregion
    }
}