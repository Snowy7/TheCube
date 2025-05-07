using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Networking
{
public class SteamNetworkManager : NetworkManager
    {
        public static event Action OnSteamInitialized;

        public static SteamNetworkManager Instance => singleton as SteamNetworkManager;
        public bool IsReady { get; private set; }
        
        [FormerlySerializedAs("MainMenuScene")]
        [Header("Scenes")]
        [SerializeField, Scene] string mainMenuScene = "MainMenu";
        
        [Header("Steam Settings")]
        [SerializeField] private bool useSteam = true;
        [SerializeField] private Client playerClient;

        public List<Client> clients = new();

        public override void Start()
        {
            base.Start();
            IsReady = false;
            StartCoroutine(InitSteam());
        }

        IEnumerator InitSteam()
        {
            yield return new WaitUntil(() => SteamManager.Initialized);
            Debug.Log($"Steam is ready: {SteamFriends.GetPersonaName()}");
            OnSteamInitialized?.Invoke();
            IsReady = true;
        }


        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Debug.Log("OnServerAddPlayer");
            
            Client client = Instantiate(playerClient);

            client.connectionID = conn.connectionId;
            client.playerIdNumber = clients.Count + 1;
            client.steamId = useSteam ?
                (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyId,
                    clients.Count) : 0;
            client.playerName = useSteam ? SteamFriends.GetFriendPersonaName(new CSteamID(client.steamId)) : "Unknown-Naggah";

            // Check if there is already a play for this connection
            if (conn.identity != null)
            {
                Debug.Log("There is already a player for this connection");
                return;
            }
            
            NetworkServer.AddPlayerForConnection(conn, client.gameObject);
            
            clients.Add(client);
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            ClientsManager.Instance.Clear();
            clients.Clear();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            ClientsManager.Instance.Clear();
            clients.Clear();
        }
        
        public void Disconnect(ulong currentLobbyId, bool loadMainMenu = true)
        {
            SteamMatchmaking.LeaveLobby(new CSteamID(currentLobbyId));
            
            StopClient();
            StopServer();
            
            if (loadMainMenu)
            {
                ServerChangeScene(mainMenuScene);
            }
        }
    }
}