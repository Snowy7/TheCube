using System;
using Actors.Player;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

namespace Networking
{
    public class Client : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private NetworkIdentity playerPrefab;
        [SerializeField] private SpectatePlayer spectatePrefab;
        
        [Header("Client Info")]
        [SyncVar, Disable] public int connectionID;
        [SyncVar, Disable] public ulong steamId;
        [SyncVar, Disable] public int playerIdNumber;
        [SyncVar, Disable] public string playerName;
        [SyncVar(hook = nameof(OnCharacterNetIdChange))] public uint characterNetId;
        
        private NetworkIdentity m_currentPlayer;
        private Player m_player;
        
        [CanBeNull]
        public Player Player
        {
            // check if the player is null / valid / not missing
            get
            {
                if (m_player != null)
                {
                    try
                    {
                        m_player.Ping();
                        return m_player;
                    }
                    catch (MissingReferenceException)
                    {
                        return null;
                    }
                }

                return null;
            }
        }
        
        public event Action<Player> OnCharacterSpawned;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
        }


        public override void OnStartClient()
        {
            base.OnStartClient();
            
            ClientsManager.Instance.RegisterClient(this, isLocalPlayer);
            
            OnCharacterNetIdChange(0, characterNetId);
        }
        
        public override void OnStopClient()
        {
            base.OnStopClient();
            
            ClientsManager.Instance.UnRegisterClient(this);
        }
        
        private void OnCharacterNetIdChange(uint oldNetId, uint newNetId)
        {
            if (NetworkClient.spawned.TryGetValue(newNetId, out NetworkIdentity identity))
            {
                m_player = identity.GetComponent<Player>();
                if (m_player) m_currentPlayer = identity;
                else m_currentPlayer = null;

                if (m_player)
                {
                    this.m_player.Character.SetActorId(playerIdNumber);
                }
                
                // if local client, raise the event
                if (isOwned)
                {
                    OnCharacterSpawned?.Invoke(m_player);
                }
            }
        }
        
        [Server]
        private void ServerSpawnPlayer()
        {
            if (!playerPrefab) return;
            try
            {
                if (m_currentPlayer && this.m_player && this.m_player.Ping())
                {
                    Debug.LogError("You cannot be Naruto!");
                    return;
                }   
            } catch (Exception)
            {
                // ignored, this is expected if the player is not found
            }

            Transform spawnPoint = SteamNetworkManager.Instance.GetStartPosition();
            if (spawnPoint == null)
            {
                Debug.LogError("No spawn point found");
                return;
            }
            
            GameObject player = Instantiate(playerPrefab.gameObject, spawnPoint.position, spawnPoint.rotation);
            
            NetworkServer.Spawn(player.gameObject, connectionToClient);
            
            m_currentPlayer = player.GetComponent<NetworkIdentity>();
            
            characterNetId = m_currentPlayer.netId;
            
            this.m_player = m_currentPlayer.GetComponent<Player>();
            this.m_player.Character.SetActorId(playerIdNumber);
            
            if (isOwned)
            {
                OnCharacterSpawned?.Invoke(m_player);
            }
        }
        
        [Command]
        private void CmdSpawnPlayer()
        {
            ServerSpawnPlayer();
        }
        
        public void SpawnPlayer()
        {
            if (!playerPrefab) return;

            if (!isServer) CmdSpawnPlayer();
            else ServerSpawnPlayer();
        }

        public void SpawnSpectateCamera()
        {
            if (!spectatePrefab) return;
            
            Transform spawnPoint = SteamNetworkManager.Instance.GetStartPosition();
            if (spawnPoint == null)
            {
                Debug.LogError("No spawn point found");
                return;
            }
            
            SpectatePlayer spectateCamera = Instantiate(spectatePrefab, spawnPoint.position, spawnPoint.rotation);
            spectateCamera.SetTarget(m_player);
        }
    }
}