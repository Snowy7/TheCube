using Actors.Player;
using Inventory;
using Inventory.Attachments;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Elements
{
    public class WeaponSlotUI : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Colors")] [Tooltip("Color applied to all images.")] [SerializeField]
        private Color imageColor = Color.white;

        [Header("Settings")] [Tooltip("Weapon Body Image.")] [SerializeField]
        private Image imageWeaponBody;

        [Tooltip("Weapon Grip Image.")] [SerializeField]
        private Image imageWeaponGrip;

        [Tooltip("Weapon Laser Image.")] [SerializeField]
        private Image imageWeaponLaser;

        [Tooltip("Weapon Silencer Image.")] [SerializeField]
        private Image imageWeaponMuzzle;

        [Tooltip("Weapon Magazine Image.")] [SerializeField]
        private Image imageWeaponMagazine;

        [Tooltip("Weapon Scope Image.")] [SerializeField]
        private Image imageWeaponScope;

        [Tooltip("Weapon Scope Default Image.")] [SerializeField]
        private Image imageWeaponScopeDefault;
        
        [Tooltip("Weapon Ammo Text.")] [SerializeField]
        private TMP_Text textWeaponAmmo;

        #endregion

        #region FIELDS

        /// <summary>
        /// Weapon Attachment Manager.
        /// </summary>

        #endregion

        #region METHODS

        public void Tick(Weapon weapon)
        {
            // Update ammo text.
            textWeaponAmmo.text = weapon.GetAmmunitionCurrent().ToString();
            
            //Calculate what color and alpha we need to apply.
            Color toAssign = imageColor;
            foreach (Image image in GetComponents<Image>())
                image.color = toAssign;

            //Update the main body sprite!
            imageWeaponBody.sprite = weapon.GetSpriteBody();

            //Get Attachment Manager.
            var attachmentManagerBehaviour = weapon.GetAttachmentManager();
            //Update the weapon's body sprite!
            imageWeaponBody.sprite = weapon.GetSpriteBody();

            //Sprite.
            Sprite sprite = default;

            //Scope Default.
            Scope scopeDefaultBehaviour = attachmentManagerBehaviour.GetScopeDefault();
            //Get Sprite.
            if (scopeDefaultBehaviour != null)
                sprite = scopeDefaultBehaviour.GetSprite();
            //Assign Sprite!
            AssignSprite(imageWeaponScopeDefault, sprite, scopeDefaultBehaviour == null);

            //Scope.
            Scope scopeBehaviour = attachmentManagerBehaviour.GetScope();
            //Get Sprite.
            if (scopeBehaviour != null)
                sprite = scopeBehaviour.GetSprite();
            //Assign Sprite!
            AssignSprite(imageWeaponScope, sprite, scopeBehaviour == null || scopeBehaviour == scopeDefaultBehaviour);

            //Magazine.
            Magazine magazineBehaviour = attachmentManagerBehaviour.GetMagazine();
            //Get Sprite.
            if (magazineBehaviour != null)
                sprite = magazineBehaviour.GetSprite();
            //Assign Sprite!
            AssignSprite(imageWeaponMagazine, sprite, magazineBehaviour == null);

            //Laser.
            Laser laserBehaviour = attachmentManagerBehaviour.GetLaser();
            //Get Sprite.
            if (laserBehaviour != null)
                sprite = laserBehaviour.GetSprite();
            //Assign Sprite!
            AssignSprite(imageWeaponLaser, sprite, laserBehaviour == null);

            //Grip.
            Grip gripBehaviour = attachmentManagerBehaviour.GetGrip();
            //Get Sprite.
            if (gripBehaviour != null)
                sprite = gripBehaviour.GetSprite();
            //Assign Sprite!
            AssignSprite(imageWeaponGrip, sprite, gripBehaviour == null);

            //Muzzle.
            Muzzle muzzleBehaviour = attachmentManagerBehaviour.GetMuzzle();
            //Get Sprite.
            if (muzzleBehaviour != null)
                sprite = muzzleBehaviour.GetSprite();
            //Assign Sprite!
            AssignSprite(imageWeaponMuzzle, sprite, muzzleBehaviour == null);
        }

        /// <summary>
        /// Assigns a sprite to an image.
        /// </summary>
        private static void AssignSprite(Image image, Sprite sprite, bool forceHide = false)
        {
            //Update.
            image.sprite = sprite;
            //Disable image if needed.
            image.enabled = sprite != null && !forceHide;
        }

        #endregion
    }
}