using Inventory;
using Snowy.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopWeapon : MonoBehaviour
    {
        [SerializeField] private SnButton button;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text priceText;
        
        [Disable, SerializeField] private WeaponItem weaponItem;
        
        public WeaponItem WeaponItem => weaponItem;
        public SnButton Button => button;
        
        public void Initialize(WeaponItem item)
        {
            weaponItem = item;
            icon.sprite = item.itemIcon ?? item.weaponPrefab.GetSpriteBody();
            nameText.text = item.itemName;
            priceText.text = item.price.ToString() + " H";
        }
    }
}