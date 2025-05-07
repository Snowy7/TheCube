using System.Collections;
using System.Collections.Generic;
using SnInput;
using Snowy.Utils;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Networking.Chat
{
    public class SteamChatController : MonoSingleton<SteamChatController>
    {
        public override bool DestroyOnLoad => false;
        
        [Header("References")]
        [SerializeField] private TMP_InputField chatInput;
        [SerializeField] private Image chatBackground;
        [SerializeField] private HorizontalOrVerticalLayoutGroup msgContainer;
        [SerializeField] private ScrollRect scrollbar;
        [SerializeField] private ChatMessage msgPrefab;

        [Header("Settings")] 
        [SerializeField] private int maxMessages = 25;
        [SerializeField] private Color chatActiveColor;
        [SerializeField] private Color chatInactiveColor;
        
        [Header("Messages")]
        [SerializeField] private Color userMessageColor;
        [SerializeField] private Color systemMessageColor;
        [SerializeField] private Color errorMessageColor;

        private List<ChatMessage> messages = new();
        
        # region Unity Callbacks

        private void Start()
        {
            if (SteamLobby.Instance) SteamLobby.Instance.OnJoinSuccess.AddListener(OnJoinSuccess);
            if (SteamLobby.Instance) SteamLobby.Instance.OnMessageReceived.AddListener(OnMessageReceived);
            
            InputManager.OnChatToggle += OnChatToggle;
            
            // Disable
            Deactivate();
        }

        # endregion

        # region Public Methods

        public void Deactivate()
        {
            // send the message
            SendChatMessage();
                    
            // deactivate input field
            chatInput.DeactivateInputField();
            chatInput.text = string.Empty;
                    
            // hide chat
            chatInput.gameObject.SetActive(false);
            chatBackground.color = chatInactiveColor;
                    
            // hide scrollbar
            scrollbar.verticalScrollbar.gameObject.SetActive(false);
        }

        public void Activate()
        {
            // activate input field
            chatInput.ActivateInputField();
            chatInput.Select();
                    
            // show chat
            chatInput.text = string.Empty;
            chatInput.gameObject.SetActive(true);
            chatBackground.color = chatActiveColor;
                    
            // show scrollbar
            scrollbar.enabled = true;
            scrollbar.verticalScrollbar.gameObject.SetActive(true);
        }
        
        public void SendChatMessage()
        {
            var message = chatInput.text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            Debug.Log($"Sending message: {message}");

            SteamLobby.Instance.SendChatMessage(2, message);
        }
        
        public void SendSystemMessage(string message)
        {
            SteamLobby.Instance.SendChatMessage(0, message);
        }

        # endregion

        # region Private Methods

        private void OnMessageReceived(int id, string sender, string message)
        {
            if (messages.Count >= maxMessages)
            {
                Destroy(messages[0].gameObject);
                messages.RemoveAt(0);
            }
            
            switch (id)
            {
                case 0:
                    // System message
                    AddSystemMessage(message);
                    break;
                case 1:
                    // Error message
                    AddErrorMessage(message);
                    break;
                case 2:
                    // Chat message
                    AddChatMessage(sender, message);
                    break;
            }
        }
        
        private void OnJoinSuccess()
        {
            // clear chat
            foreach (var message in messages)
            {
                Destroy(message.gameObject);
            }
            messages.Clear();
            
            // Add a message
            var username = SteamFriends.GetPersonaName();
            SendSystemMessage($"{username} has joined the lobby.");
        }
        
        private void OnChatToggle(InputAction.CallbackContext context)
        {
            if (SteamLobby.Instance == null || !SteamLobby.Instance.IsInLobby) return;
            
            if (context.phase == InputActionPhase.Started)
            {
                bool isActive = !chatInput.gameObject.activeSelf;
                if (isActive)
                {
                    Activate();
                    InputManager.Instance.SwitchToUIControls();
                }
                else
                {
                    Deactivate();
                    InputManager.Instance.SwitchToGameControls();
                }
            }
        }
        
        private void AddChatMessage(string sender, string message)
        {
            var msg = Instantiate(msgPrefab, msgContainer.transform);
            msg.SetMessage(message);
            msg.SetColor(userMessageColor);
            msg.SetUsername(sender);
            
            // Scroll to bottom
            StartCoroutine(ScrollToBottom());
            
            // Add to messages
            messages.Add(msg);
        }
        
        private void AddSystemMessage(string message)
        {
            var msg = Instantiate(msgPrefab, msgContainer.transform);
            msg.SetMessage(message);
            msg.SetColor(systemMessageColor);
            msg.SetUsername("System");
            
            // Scroll to bottom
            StartCoroutine(ScrollToBottom());
            
            // Add to messages
            messages.Add(msg);
        }
        
        private void AddErrorMessage(string message)
        {
            var msg = Instantiate(msgPrefab, msgContainer.transform);
            msg.SetMessage(message);
            msg.SetColor(errorMessageColor);
            msg.SetUsername("Error");
            
            // Scroll to bottom
            StartCoroutine(ScrollToBottom());
            
            // Add to messages
            messages.Add(msg);
        }

        private void AddMessage(string message)
        {
            if (messages.Count >= maxMessages)
            {
                Destroy(messages[0].gameObject);
                messages.RemoveAt(0);
            }

            // code: 0 = user message, 1 = system message
            var split = message.Split(':');
            Debug.Log($"Split: {split[0]} - {split[1]}");
            var type = int.Parse(split[0]);
            var msg = split[1];
            
            AddMessage(type, msg);
        }
        
        private void AddMessage(int type, string message)
        {
            if (messages.Count >= maxMessages)
            {
                Destroy(messages[0].gameObject);
                messages.RemoveAt(0);
            }

            var msg = Instantiate(msgPrefab, msgContainer.transform);
            msg.SetMessage(message);
            
            switch (type)
            {
                case 0:
                    msg.SetUsername(SteamFriends.GetPersonaName());
                    break;
                case 1:
                    msg.SetUsername("System");
                    break;
            }

            // Scroll to bottom
            StartCoroutine(ScrollToBottom());
        }

        IEnumerator ScrollToBottom()
        {
            yield return new WaitForEndOfFrame();
            scrollbar.verticalNormalizedPosition = 0;
            scrollbar.normalizedPosition = new Vector2(0, 0);
        }

        private void OnDestroy()
        {
            if (SteamLobby.Instance) SteamLobby.Instance.OnJoinSuccess.RemoveListener(OnJoinSuccess);
            if (SteamLobby.Instance) SteamLobby.Instance.OnMessageReceived.RemoveListener(OnMessageReceived);
        }

        private void OnApplicationQuit()
        {
            if (SteamLobby.Instance) SteamLobby.Instance.OnJoinSuccess.RemoveListener(OnJoinSuccess);
            if (SteamLobby.Instance) SteamLobby.Instance.OnMessageReceived.RemoveListener(OnMessageReceived);
        }

        # endregion
    }
}