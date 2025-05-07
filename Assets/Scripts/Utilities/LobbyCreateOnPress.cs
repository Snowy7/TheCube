using System;
using System.Collections;
using Firebase;
using Firebase.Game;
using Networking;
using SnInput;
using Snowy.Inspector;
using Snowy.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utilities
{
    public class LobbyCreateOnPress : MonoBehaviour
    {
        [SerializeField] private GameObject textObject;
        [SerializeField] private GameObject loadingEffect;
        [SerializeField] private float effectDuration = 5f;
        
        private void Start()
        {
            // Enable cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            InputManager.OnJoinPress += OnJoinPress;
            textObject.SetActive(false);
            LoadingPanel.Instance.Show("");
            StartCoroutine(WaitForServices());
            
            SteamLobby.Instance.OnJoinFailed.AddListener(OnJoinFailed);
        }
        
        private void OnJoinFailed()
        {
            LoadingPanel.Instance.Hide();
            textObject.SetActive(true);
            
            // Resub to join press
            InputManager.OnJoinPress += OnJoinPress;
        }

        IEnumerator WaitForServices()
        {
            yield return new WaitUntil(() => 
                SteamNetworkManager.Instance?.IsReady == true &&
                UserController.Instance?.IsReady == true
                );
            
            LoadingPanel.Instance.Hide();
            textObject.SetActive(true);
        }
        
        private void OnDestroy()
        {
            InputManager.OnJoinPress -= OnJoinPress;
        }
        
        private void OnJoinPress(InputAction.CallbackContext context)
        {
            Debug.Log("Join press");
            if (!textObject.activeSelf) return;
            
            if (context.started && !SteamLobby.Instance.IsInLobby && !SteamNetworkManager.Instance.isNetworkActive)
            {
                loadingEffect.SetActive(true);
                Invoke(nameof(Join), effectDuration);
                InputManager.OnJoinPress -= OnJoinPress;
            }
        }
        
        private void Join()
        {
            SteamLobby.Instance.CreateLobby();
        }
    }
}