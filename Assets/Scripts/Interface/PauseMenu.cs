using System;
using Networking;
using Networking.Chat;
using SnInput;
using Snowy.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interface
{
    public class PauseMenu : MonoSingleton<PauseMenu>
    {
        public override bool DestroyOnLoad => false;

        private float m_triggerDelay = 0.2f;
        private float m_triggerTime;

        [Title("References")]
        [SerializeField] private GameObject pauseMenu;

        
        private void Start()
        {
            InputManager.OnLockCursor += OnLockCursor;
            
            // Disable
            Deactivate();
        }

        private void OnDestroy()
        {
            if (InputManager.Instance)
            {
                InputManager.OnLockCursor -= OnLockCursor;
            }
        }

        private void OnLockCursor(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                if (Time.time < m_triggerTime + m_triggerDelay) return;
                m_triggerTime = Time.time;
                bool isActive = !pauseMenu.activeSelf;
                if (isActive)
                {
                    Activate();
                }
                else
                {
                    Deactivate();
                }
            }
        }
        
        private void Activate()
        {
            if (!MenuManager.CanToggle) return;
            
            pauseMenu.SetActive(true);
            
            // Deactivate the chat if it's active
            SteamChatController.Instance.Deactivate();
            
            if (ClientsManager.Instance.LocalClient != null && ClientsManager.Instance.LocalClient.Player != null)
                ClientsManager.Instance.LocalClient.Player.OnLockCursor(false);
            else
            {
                // Make sure the cursor is unlocked
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
            InputManager.Instance.SwitchToUIControls();
        }

        public void Deactivate()
        {
            if (!MenuManager.CanToggle) return;
            
            pauseMenu.SetActive(false);
            if (ClientsManager.Instance.LocalClient != null && ClientsManager.Instance.LocalClient.Player != null)
                ClientsManager.Instance.LocalClient.Player.OnLockCursor(true);
            else
            {
                // Make sure the cursor is locked
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            
            
            InputManager.Instance.SwitchToGameControls();
        }
    }
}