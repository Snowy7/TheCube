using UnityEngine;

namespace Inventory.Attachments
{
    public class WeaponAttachment : MonoBehaviour
    {
        [Header("Interface")] 
        [Tooltip("Leave empty for now!"), SerializeField] protected Sprite sprite;

        /// <summary>
        /// Sprite.
        /// </summary>
        public Sprite GetSprite() => sprite;
        public void SetSprite(Sprite spr) => sprite = spr;
    }
}