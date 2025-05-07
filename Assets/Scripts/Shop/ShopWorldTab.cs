using System;
using System.Collections.Generic;
using DataManagers.Presets;
using Firebase.Game;
using Game.WorldSystem;
using Inventory;
using Snowy.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopWorldTab : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private GameObject emptyPanel;
        [SerializeField] private GameObject contentPanel;
        [SerializeField] private Transform shopItemsParent;
        [SerializeField] private ShopWeapon shopItemPrefab;
        [SerializeField] private TabUI tabUI;
        [SerializeField] private Image iconImage;
        [SerializeField] private SnButton buyButton;
        [SerializeField] private SnButton bought;
        [SerializeField] private SnButton loadButton;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text priceText;
        
        [Title("Debug")]
        [Disable, SerializeField] private WorldData worldData;
        [Disable, SerializeField] private WeaponItem selectedItem;
        [Disable, SerializeField] private List<ShopWeapon> shopItems;
        
        private bool loading;
        
        public TabUI TabUI => tabUI;

        private void OnEnable()
        {
            // Select first item
            if (shopItems.Count > 0)
                OnSelect(shopItems[0].WeaponItem);
            
            emptyPanel.SetActive(shopItems.Count == 0);
            contentPanel.SetActive(shopItems.Count > 0);
        }

        public void Initialize(WorldData data)
        {
            worldData = data;
            
            // Load shop items
            foreach (var item in data.weapons)
            {
                var shopItem = Instantiate(shopItemPrefab, shopItemsParent);
                shopItem.Initialize(item);
                
                shopItem.Button.OnClick.AddListener(() => OnSelect(item));
                
                shopItems.Add(shopItem);
            }
            
            emptyPanel.SetActive(shopItems.Count == 0);
            contentPanel.SetActive(shopItems.Count > 0);
            
            // Select first item
            if (shopItems.Count > 0)
                OnSelect(shopItems[0].WeaponItem);
        }

        public void OnSelect(WeaponItem item)
        {
            iconImage.sprite = item.itemIcon ?? item.weaponPrefab.GetSpriteBody();
            titleText.text = item.itemName;
            descriptionText.text = item.itemDescription;
            priceText.text = item.price.ToString() + " H";
            
            selectedItem = item;
            
            // if is owned
            bool isOwned = UserItems.Instance.HasItem(item.itemID);
            buyButton.gameObject.SetActive(!isOwned);
            bought.gameObject.SetActive(isOwned);
            loadButton.gameObject.SetActive(false);
            
            // Unslect all other items
            foreach (var shopItem in shopItems)
            {
                if (shopItem.WeaponItem.itemID != item.itemID)
                    shopItem.Button.SetSelected(false);
                else
                    shopItem.Button.SetSelected(true, true);
            }
        }
        
        public void Buy()
        {
            if (selectedItem == null)
                return;
            
            loading = true;
            loadButton.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(false);
            bought.gameObject.SetActive(false);
            
            UserItems.Instance.BuyItem(selectedItem, selectedItem.price).ContinueWith(task =>
            {
                if (task.Result)
                {
                    // Update UI
                    loading = false;
                    loadButton.gameObject.SetActive(false);
                    buyButton.gameObject.SetActive(false);
                    bought.gameObject.SetActive(true);
                } 
                else
                {
                    loading = false;
                    loadButton.gameObject.SetActive(false);
                    buyButton.gameObject.SetActive(true);
                    bought.gameObject.SetActive(false);
                }
            });
        }
    }
}