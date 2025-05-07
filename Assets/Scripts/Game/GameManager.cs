using System.Linq;
using Actors.Player;
using Game.WorldSystem;
using Mirror;
using Networking;
using Snowy.Utils;
using SnTerminal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField, ReorderableList] private GameObject[] cleanupObjects;
        
        public override bool DestroyOnLoad => false;
        private bool m_isSpawningPlayer;
        
        private bool m_inMission;

        private void OnEnable()
        {
            WorldsManager.Instance.OnSceneLoadedEvent.AddListener(OnSceneLoaded);
            
            // If the scene is already loaded, spawn the player
            if (WorldsManager.Instance.CurrentWorldData?.autoPlayerSpawn == true && !m_isSpawningPlayer)
            {
                SpawnPlayer();
            }
        }

        private void OnDisable()
        {
            WorldsManager.Instance.OnSceneLoadedEvent.RemoveListener(OnSceneLoaded);
        }
        
        private void Update()
        {
            if (!NetworkServer.active) return;
            
            if (MissionManager.Instance.isMissionActive && m_inMission)
            {
                CheckMission();
            }
        }

        private void CheckMission()
        {
            // Check how many players are alive
            var alivePlayers = GetAlivePlayers();
            if (alivePlayers.Length == 0)
            {
                Debug.Log("All players are dead");
                // Fail the mission
                FailMission();
            }
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode, WorldData worldData)
        {
            if (worldData.autoPlayerSpawn && !m_isSpawningPlayer)
            {
                // Wait for the client to be ready then spawn it
                SpawnPlayer();
                
                // Set the mission flag
                // if in mission and the mission world is the current world
                ClientsManager.Instance.WaitForAllClients(clients =>
                {
                    m_inMission = MissionManager.Instance.isMissionActive && MissionManager.Instance.CurrentMission.worldData == worldData;
                });
            }
        }
        
        public void SpawnPlayer()
        {
            m_isSpawningPlayer = true;
            // for now remove the loading screen
            LoadingPanel.Instance.Hide();
            
            // Spawn the player
            ClientsManager.Instance.WaitForLocalClient(client =>
            {
                client.SpawnPlayer();
                m_isSpawningPlayer = false;
            });
        }
        
        public Player[] GetAlivePlayers()
        {
            return ClientsManager.Instance.clients.Select(client => client.Player).Where(player => player != null && !player.IsDead).ToArray();
        }

        public void SpawnSpectateCamera()
        {
            ClientsManager.Instance.WaitForLocalClient(client =>
            {
                client.SpawnSpectateCamera();
            });
        }

        public void Cleanup()
        {
            ClientsManager.Instance.Clear();
            // Destroy Shop and PauseMenu
            
            foreach (var cleanupObject in cleanupObjects)
            {
                if (cleanupObject == null) continue;
                Destroy(cleanupObject);
            }
        }
        
        public void FailMission()
        {
            // End the mission
            MissionManager.Instance.MissionEnd();
        }
    }
}