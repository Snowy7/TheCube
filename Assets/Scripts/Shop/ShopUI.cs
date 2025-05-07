using System;
using Game.WorldSystem;
using Interface;
using Networking;
using Networking.Chat;
using SnInput;
using Snowy.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Shop
{
    public class ShopUI : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private Transform tabsContainer;
        [SerializeField] private ShopWorldTab shopTabPrefab;
        [SerializeField] private HorizontalOrVerticalLayoutGroup tabsButtonsContainer;
        [SerializeField] private SnButton tabButton;
        [SerializeField] private Tabs tabs;
        
        private bool m_initialized;

        private void OnEnable()
        {
            if (m_initialized)
                return;
            
            // TODO: Load the tab for each world
            foreach (var world in WorldsManager.Instance.Worlds)
            {
                var shopTab = Instantiate(shopTabPrefab, tabsContainer);
                shopTab.Initialize(world);
                
                // Spawn button
                var button = Instantiate(tabButton, tabsButtonsContainer.transform);
                button.SetText(world.worldName);
                
                tabs.AddTab(new TabElement
                {
                    actionButton = button,
                    content = shopTab.TabUI,
                    onSelected = new UnityEngine.Events.UnityEvent()
                });
            }
            
            m_initialized = true;
        }

        public void Activate()
        {
            tabsButtonsContainer.enabled = true;
            shopPanel.SetActive(true);

            // Enable UI Input and subscribe to dismiss event
            if (InputManager.Instance)
            {
                InputManager.Instance.SwitchToUIControls();
                InputManager.OnLockCursor += OnDismiss;
            }
            
            if (MenuManager.Instance)
                MenuManager.CanToggle = false;
            
            if (ClientsManager.Instance.LocalClient != null)
                ClientsManager.Instance.LocalClient.Player?.OnLockCursor(false);
        }

        private void OnDismiss(InputAction.CallbackContext context)
        {
            Deactivate();
        }

        void Deactivate()
        {
            shopPanel.SetActive(false);
            tabsButtonsContainer.enabled = false;

            // Disable UI Input and unsubscribe from dismiss event
            if (InputManager.Instance)
            {
                InputManager.Instance.SwitchToGameControls();
                InputManager.OnLockCursor -= OnDismiss;
            }
            
            if (MenuManager.Instance)
                MenuManager.CanToggle = true;
            
            if (ClientsManager.Instance.LocalClient != null)
                ClientsManager.Instance.LocalClient.Player?.OnLockCursor(true);
        }
        
    }
}