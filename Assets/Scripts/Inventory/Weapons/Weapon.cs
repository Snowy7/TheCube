using System;
using Inventory.Attachments;
using UnityEngine;
using Actors.Player;
using Actors.AI;
using Audio;
using Inventory;
using Level;
using Snowy.Inspector;
using Utilities;
using AudioType = Audio.AudioType;
using Random = UnityEngine.Random;

namespace Inventory
{
    public class Weapon : InHandItem
    {
        #region FIELDS SERIALIZED
        
        [Header("Baked TP transform")]
        [SerializeField, Unity.Collections.ReadOnly] private Vector3 bakedTpPosition;
        [SerializeField, Unity.Collections.ReadOnly] private Vector3 bakedTpRotation;
        
        [Header("Settings")]
        [Tooltip("Weapon Name. Currently not used for anything, but in the future, we will use this for pickups!")]
        [SerializeField] 
        private string weaponName;

        [Tooltip("How much the character's movement speed is multiplied by when wielding this weapon.")]
        [SerializeField]
        private float multiplierMovementSpeed = 1.0f;
        
        [SerializeField] private float damage = 10.0f;
        [SerializeField] private float force = 10.0f;
        
        [Header("Firing")]

        [Tooltip("Is this weapon automatic? If yes, then holding down the firing button will continuously fire.")]
        [SerializeField] 
        private bool automatic;
        
        [Tooltip("Is this weapon bolt-action? If yes, then a bolt-action animation will play after every shot.")]
        [SerializeField]
        private bool boltAction;

        [Tooltip("Amount of shots fired at once. Helpful for things like shotguns, where there are multiple projectiles fired at once.")]
        [SerializeField]
        private int shotCount = 1;
        
        [Tooltip("How fast the projectiles are.")]
        [SerializeField]
        private float projectileImpulse = 400.0f;

        [Tooltip("Amount of shots this weapon can shoot in a minute. It determines how fast the weapon shoots.")]
        [SerializeField] 
        private int roundsPerMinutes = 200;

        [Header("Spread")]
        [Tooltip("How far the weapon can fire from the center of the screen.")]
        [SerializeField] private float spread = 0.25f;
        [Tooltip("Movement spread multiplier.")]
        [SerializeField] private float walkSpreadMultiplier = 1.5f;
        [Tooltip("Running spread multiplier.")]
        [SerializeField] private float runSpreadMultiplier = 2.0f;
        [Tooltip("In air spread multiplier.")]
        [SerializeField] private float inAirSpreadMultiplier = 2.5f;
        [Tooltip("Crouching spread multiplier.")]
        [SerializeField] private float crouchSpreadMultiplier = 0.5f;
        [Tooltip("Spread increase per shot.")]
        [SerializeField] private float spreadIncrease = 0.1f;
        [Tooltip("Crosshair size multiplier.")]
        [SerializeField] private float crosshairSizeMultiplier = 50.0f;
        
        [Header("Reloading")]
        
        [Tooltip("Determines if this weapon reloads in cycles, meaning that it inserts one bullet at a time, or not.")]
        [SerializeField]
        private bool cycledReload;
        
        [Tooltip("Determines if the player can reload this weapon when it is full of ammunition.")]
        [SerializeField]
        private bool canReloadWhenFull = true;

        [Tooltip("Should this weapon be reloaded automatically after firing its last shot?")]
        [SerializeField]
        private bool automaticReloadOnEmpty;

        [Tooltip("Time after the last shot at which a reload will automatically start.")]
        [SerializeField]
        private float automaticReloadOnEmptyDelay = 0.25f;

        [Header("Animation")]

        [Tooltip("Transform that represents the weapon's ejection port, meaning the part of the weapon that casings shoot from.")]
        [SerializeField]
        private Transform socketEjection;

        [Tooltip("Settings this to false will stop the weapon from being reloaded while the character is aiming it.")]
        [SerializeField]
        private bool canReloadAimed = true;

        [Header("Resources")]

        [Tooltip("Casing Prefab.")]
        [SerializeField]
        private GameObject prefabCasing;
        
        [Tooltip("Projectile Prefab. This is the prefab spawned when the weapon shoots.")]
        [SerializeField]
        private Bullet prefabProjectile;
        
        [Tooltip("(First Person) The AnimatorController a player character needs to use while wielding this weapon.")]
        [SerializeField] 
        public RuntimeAnimatorController controller;
        
        [Tooltip("(Full Body TP)The AnimatorController a player character needs to use while wielding this weapon.")]
        [SerializeField]
        public RuntimeAnimatorController controllerTp;

        [Tooltip("Weapon Body Texture.")]
        [SerializeField]
        private Sprite spriteBody;
        
        [Header("Audio Clips Holster")]

        [Tooltip("Holster Audio Clip.")]
        [SerializeField]
        private AudioClip audioClipHolster;

        [Tooltip("Unholster Audio Clip.")]
        [SerializeField]
        private AudioClip audioClipUnholster;
        
        [Header("Audio Clips Reloads")]

        [Tooltip("Reload Audio Clip.")]
        [SerializeField]
        private AudioClip audioClipReload;
        
        [Tooltip("Reload Empty Audio Clip.")]
        [SerializeField]
        private AudioClip audioClipReloadEmpty;
        
        [Header("Audio Clips Reloads Cycled")]
        
        [Tooltip("Reload Open Audio Clip.")]
        [SerializeField]
        private AudioClip audioClipReloadOpen;
        
        [Tooltip("Reload Insert Audio Clip.")]
        [SerializeField]
        private AudioClip audioClipReloadInsert;
        
        [Tooltip("Reload Close Audio Clip.")]
        [SerializeField]
        private AudioClip audioClipReloadClose;
        
        [Header("Audio Clips Other")]

        [Tooltip("AudioClip played when this weapon is fired without any ammunition.")]
        [SerializeField]
        private AudioClip audioClipFireEmpty;
        
        [Tooltip("")]
        [SerializeField]
        private AudioClip audioClipBoltAction;

        #endregion

        #region FIELDS

        /// <summary>
        /// Weapon Audio Source.
        /// </summary>
        private AudioSource audioSource;
        
        /// <summary>
        /// Weapon Animator.
        /// </summary>
        private Animator animator;
        /// <summary>
        /// Attachment Manager.
        /// </summary>
        private WeaponAttachmentManager attachmentManager;
        
        /// <summary>
        /// Player Inventory.
        /// </summary>
        private PlayerInventory inventory;
        
        /// <summary>
        /// Item Animation Data.
        /// </summary>
        private ItemAnimationData animationData;

        /// <summary>
        /// Amount of ammunition left.
        /// </summary>
        private int ammunitionCurrent;

        #region Attachment Behaviours
        
        /// <summary>
        /// Equipped scope Reference.
        /// </summary>
        private Scope scopeBehaviour;
        
        /// <summary>
        /// Equipped Magazine Reference.
        /// </summary>
        private Magazine magazineBehaviour;
        /// <summary>
        /// Equipped Muzzle Reference.
        /// </summary>
        private Muzzle muzzleBehaviour;

        /// <summary>
        /// Equipped Laser Reference.
        /// </summary>
        private Laser laserBehaviour;
        /// <summary>
        /// Equipped Grip Reference.
        /// </summary>
        private Grip gripBehaviour;

        public int ID { get; set; }
        
        #endregion
        
        /// <summary>
        /// The main player character behaviour component.
        /// </summary>
        private FPSCharacter characterBehaviour;

        /// <summary>
        /// The player character's camera.
        /// </summary>
        private Transform playerCamera;
        
        private bool isReady;
        
        #endregion

        #region UNITY
        
        protected void Awake()
        {
            //Get Audio Source.
            audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
            
            //Get Animator.
            animator = GetComponent<Animator>();
            
            //Get Attachment Manager.
            attachmentManager = GetComponent<WeaponAttachmentManager>();
            
            //Cache the player character.
            characterBehaviour = GetComponentInParent<FPSCharacter>();
            
            //Cache the player inventory.
            inventory = GetComponentInParent<PlayerInventory>();
            
            //Cache the Item Animation Data.
            animationData = GetComponent<ItemAnimationData>();
            
            //Cache the world camera. We use this in line traces.
            playerCamera = characterBehaviour.GetCameraWorld().transform;
        }
        protected void Start()
        {
            #region Cache Attachment References

            //Get Scope.
            scopeBehaviour = attachmentManager.GetScope();
            
            //Get Magazine.
            magazineBehaviour = attachmentManager.GetMagazine();
            //Get Muzzle.
            muzzleBehaviour = attachmentManager.GetMuzzle();

            //Get Laser.
            laserBehaviour = attachmentManager.GetLaser();
            //Get Grip.
            gripBehaviour = attachmentManager.GetGrip();

            #endregion

            //Max Out Ammo.
            ammunitionCurrent = magazineBehaviour.GetAmmunitionTotal();
            
            //Set Ready.
            isReady = true;
        }

        #endregion

        #region GETTERS
        
        public float GetSpread() => spread;

        public float CalculateFinalSpread()
        {
            float finalSpread = spread;
            
            if (characterBehaviour.IsCrouching()) finalSpread *= crouchSpreadMultiplier;
            if (characterBehaviour.IsMoving())
            {
                if (characterBehaviour.IsRunning()) finalSpread *= runSpreadMultiplier;
                else finalSpread *= walkSpreadMultiplier;
            }
            
            if (!characterBehaviour.IsGrounded()) finalSpread *= inAirSpreadMultiplier;
            
            // ShotCount is the amount of shots fired at once. We need to increase the spread by the amount of shots fired.
            finalSpread += spreadIncrease * characterBehaviour.GetShotsFired();
            
            return finalSpread;
        }
        
        public bool IsReady() => isReady;
        
        public string GetWeaponName() => weaponName;

        /// <summary>
        /// GetFieldOfViewMultiplierAim.
        /// </summary>
        public float GetFieldOfViewMultiplierAim()
        {
            //Make sure we don't have any issues even with a broken setup!
            if (scopeBehaviour != null) 
                return scopeBehaviour.GetFieldOfViewMultiplierAim();
            
            //Error.
            // Debug.LogError("Weapon has no scope equipped!");
  
            //Return.
            return 1.0f;
        }
        /// <summary>
        /// GetFieldOfViewMultiplierAimWeapon.
        /// </summary>
        public float GetFieldOfViewMultiplierAimWeapon()
        {
            //Make sure we don't have any issues even with a broken setup!
            if (scopeBehaviour != null) 
                return scopeBehaviour.GetFieldOfViewMultiplierAimWeapon();
            
            //Error.
            // Debug.LogError("Weapon has no scope equipped!");
  
            //Return.
            return 1.0f;
        }
        
        /// <summary>
        /// GetFieldOfViewMultiplierHip.
        /// </summary>
        /// <returns>ItemAnimationData</returns>
        public ItemAnimationData GetAnimationData() => animationData;
        
        /// <summary>
        /// GetAnimator.
        /// </summary>
        public Animator GetAnimator() => animator;
        /// <summary>
        /// CanReloadAimed.
        /// </summary>
        public bool CanReloadAimed() => canReloadAimed;

        /// <summary>
        /// GetSpriteBody.
        /// </summary>
        public Sprite GetSpriteBody() => spriteBody;
        
        /// <summary>
        /// GetMultiplierMovementSpeed.
        /// </summary>
        public float GetMultiplierMovementSpeed() => multiplierMovementSpeed;

        /// <summary>
        /// GetAudioClipHolster.
        /// </summary>
        public AudioClip GetAudioClipHolster() => audioClipHolster;
        /// <summary>
        /// GetAudioClipUnholster.
        /// </summary>
        public AudioClip GetAudioClipUnholster() => audioClipUnholster;

        /// <summary>
        /// GetAudioClipReload.
        /// </summary>
        public AudioClip GetAudioClipReload() => audioClipReload;
        /// <summary>
        /// GetAudioClipReloadEmpty.
        /// </summary>
        public AudioClip GetAudioClipReloadEmpty() => audioClipReloadEmpty;
        
        /// <summary>
        /// GetAudioClipReloadOpen.
        /// </summary>
        public AudioClip GetAudioClipReloadOpen() => audioClipReloadOpen;
        /// <summary>
        /// GetAudioClipReloadInsert.
        /// </summary>
        public AudioClip GetAudioClipReloadInsert() => audioClipReloadInsert;
        /// <summary>
        /// GetAudioClipReloadClose.
        /// </summary>
        public AudioClip GetAudioClipReloadClose() => audioClipReloadClose;

        /// <summary>
        /// GetAudioClipFireEmpty.
        /// </summary>
        public AudioClip GetAudioClipFireEmpty() => audioClipFireEmpty;
        /// <summary>
        /// GetAudioClipBoltAction.
        /// </summary>
        public AudioClip GetAudioClipBoltAction() => audioClipBoltAction;
        
        /// <summary>
        /// GetAudioClipFire.
        /// </summary>
        public AudioClip GetAudioClipFire() => muzzleBehaviour.GetAudioClipFire();
        /// <summary>
        /// GetAmmunitionCurrent.
        /// </summary>
        public int GetAmmunitionCurrent() => ammunitionCurrent;

        /// <summary>
        /// GetAmmunitionTotal.
        /// </summary>
        public int GetAmmunitionTotal() => magazineBehaviour.GetAmmunitionTotal();
        /// <summary>
        /// HasCycledReload.
        /// </summary>
        public bool HasCycledReload() => cycledReload;

        /// <summary>
        /// IsAutomatic.
        /// </summary>
        public bool IsAutomatic() => automatic;
        /// <summary>
        /// IsBoltAction.
        /// </summary>
        public bool IsBoltAction() => boltAction;

        /// <summary>
        /// GetAutomaticallyReloadOnEmpty.
        /// </summary>
        public bool GetAutomaticallyReloadOnEmpty() => automaticReloadOnEmpty;
        /// <summary>
        /// GetAutomaticallyReloadOnEmptyDelay.
        /// </summary>
        public float GetAutomaticallyReloadOnEmptyDelay() => automaticReloadOnEmptyDelay;

        /// <summary>
        /// CanReloadWhenFull.
        /// </summary>
        public bool CanReloadWhenFull() => canReloadWhenFull;
        /// <summary>
        /// GetRateOfFire.
        /// </summary>
        public float GetRateOfFire() => roundsPerMinutes;
        
        /// <summary>
        /// IsFull.
        /// </summary>
        public bool IsFull() => ammunitionCurrent == magazineBehaviour.GetAmmunitionTotal();
        /// <summary>
        /// HasAmmunition.
        /// </summary>
        public bool HasAmmunition() => ammunitionCurrent > 0;

        /// <summary>
        /// GetAnimatorController.
        /// </summary>
        public RuntimeAnimatorController GetAnimatorController() => controller;
        
        
        /// <summary>
        /// GetAnimatorControllerTP.
        /// </summary>
        public RuntimeAnimatorController GetAnimatorControllerTp() => controllerTp;
        
        /// <summary>
        /// GetAttachmentManager.
        /// </summary>
        public WeaponAttachmentManager GetAttachmentManager() => attachmentManager;

        #endregion

        #region METHODS

        /// <summary>
        /// Reload.
        /// </summary>
        public void Reload()
        {
            //Set Reloading Bool. This helps cycled reloads know when they need to stop cycling.
            const string boolName = "Reloading";
            animator.SetBool(boolName, true);
            
            //Try Play Reload Sound.
            // ServiceLocator.Current.Get<IAudioManagerService>().PlayOneShot(HasAmmunition() ? audioClipReload : audioClipReloadEmpty, new AudioSettings(1.0f, 0.0f, false));
            if (AudioManager.Instance) AudioManager.Instance.PlayAudioAtPosition(HasAmmunition() ? audioClipReload : audioClipReloadEmpty, transform.position, AudioType.Sfx);
            
            //Play Reload Animation.
            animator.Play(cycledReload ? "Reload Open" : (HasAmmunition() ? "Reload" : "Reload Empty"), 0, 0.0f);
        }

        /// <summary>
        /// Shooting a raycast.
        /// </summary>
        /// <param name="spreadMultiplier"></param>
        /// <returns></returns>
        public Vector3 FireRayCast(float spreadMultiplier = 1.0f)
        {
            if (!playerCamera) return Vector3.zero;
            
            RaycastHit hit;
            Vector3 direction = playerCamera.forward;
            Vector3 spreadValue = Random.insideUnitSphere * (CalculateFinalSpread() * spreadMultiplier);
            spreadValue.z = 0;
            spreadValue = playerCamera.TransformDirection(spreadValue);
            direction = Quaternion.Euler(spreadValue) * direction;
            
            if (Physics.Raycast(playerCamera.position, direction, out hit, 1000))
            {
                Debug.DrawLine(playerCamera.position, hit.point, Color.red, 1.0f);
                return hit.point;
            }
            
            Debug.DrawRay(playerCamera.position, direction * 1000, Color.green, 1.0f);
            return playerCamera.position + direction * 1000;
        }

        public void Fire(Vector3 targetPos)
        {
            // we shoot a bullet in the direction of the target position
            if (muzzleBehaviour == null)
                return;
            
            //Play the firing animation.
            const string stateName = "Fire";
            animator.Play(stateName, 0, 0.0f);
            //Reduce ammunition! We just shot, so we need to get rid of one!
            ammunitionCurrent = Mathf.Clamp(ammunitionCurrent - 1, 0, magazineBehaviour.GetAmmunitionTotal());
            
            //Set the slide back if we just ran out of ammunition.
            if (ammunitionCurrent == 0)
                SetSlideBack(1);
            
            //Play all muzzle effects.
            muzzleBehaviour.Effect();
            
            // only owner can damage
            bool canDamage = characterBehaviour.isOwned;
            Transform socket = muzzleBehaviour.GetSocket();

            //Spawn as many projectiles as we need.
            for (var i = 0; i < shotCount; i++)
            {
                // Get the direction to the target position
                Vector3 direction = targetPos - socket.position;
                direction.Normalize();
                
                //Spawn projectile from the projectile spawn point.
                Bullet projectile = Instantiate(prefabProjectile, socket.position, Quaternion.LookRotation(direction));
                
                projectile.Init(characterBehaviour.netId, damage, projectileImpulse, force, 3f, canDamage);
                
                // play the firing sound
                if (AudioManager.Instance) AudioManager.Instance.PlayAudio(muzzleBehaviour.GetAudioClipFire(), 1f, audioSource, AudioType.Sfx);
                if (EnemyManager.Instance && characterBehaviour.isServer) EnemyManager.Instance.AlertNearbyEnemies(socket.position, 10f);
                
                // Draw alert debug sphere
                # if UNITY_EDITOR
                DebugExtensions.DebugCircle(socket.position, Vector3.up, Color.red, 10f, 5f);
                # endif
                
                //Add velocity to the projectile.
                projectile.GetComponent<Rigidbody>().linearVelocity = projectile.transform.forward * projectileImpulse;
            }
        }

        /// <summary>
        /// FillAmmunition.
        /// </summary>
        public void FillAmmunition(int amount)
        {
            //Update the value by a certain amount.
            ammunitionCurrent = amount != 0 ? Mathf.Clamp(ammunitionCurrent + amount, 
                0, GetAmmunitionTotal()) : magazineBehaviour.GetAmmunitionTotal();
        }
        /// <summary>
        /// SetSlideBack.
        /// </summary>
        public void SetSlideBack(int back)
        {
            //Set the slide back bool.
            const string boolName = "Slide Back";
            animator.SetBool(boolName, back != 0);
        }

        /// <summary>
        /// EjectCasing.
        /// </summary>
        public void EjectCasing()
        {
            //Spawn casing prefab at spawn point.
            if(prefabCasing != null && socketEjection != null)
                Instantiate(prefabCasing, socketEjection.position, socketEjection.rotation);
        }
        
        public void SetBakesData(Vector3 position, Vector3 rotation)
        {
            bakedTpPosition = position;
            bakedTpRotation = rotation;
        }
        
        public void UseBakedData()
        {
            transform.localPosition = bakedTpPosition;
            transform.localEulerAngles = bakedTpRotation;
        }
        
        [InspectorButton("Clear Baked Data")]
        [ContextMenu("Clear Baked Data")]
        public void ClearBakedData()
        {
            bakedTpPosition = Vector3.zero;
            bakedTpRotation = Vector3.zero;
        }
        
        #endregion

        public float GetCrosshairSize()
        {
            return CalculateFinalSpread() * crosshairSizeMultiplier;
        }
    }
}