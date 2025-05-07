using System.Collections;
using Actors.Player;
using Mirror;
using Networking;
using Snowy.CSharp;
using SnNotification;
using Snowy.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game.WorldSystem
{
    public class WorldsManager : NetworkSingleton<WorldsManager>
    {
        public override bool DestroyOnLoad => false;
        [SerializeField, ReorderableList, InLineEditor] private WorldData[] worlds;
        
        public UnityEvent<Scene, LoadSceneMode, WorldData> OnSceneLoadedEvent = new();

        private WorldData currentWorldData;

        public WorldData[] Worlds => worlds;
        
        public WorldData CurrentWorldData
        {
            get
            {
                if (currentWorldData == null)
                {
                    currentWorldData = GetWorldData(SceneManager.GetActiveScene());
                }
                
                return currentWorldData;
            }
        }
   
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SteamLobby.Instance.OnJoinSuccess.AddListener(OnJoinSuccess);
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SteamLobby.Instance.OnJoinSuccess.RemoveListener(OnJoinSuccess);
        }
        
        private void OnJoinSuccess()
        {
            // Spawn this network identity on the server
            
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            WorldData worldData = GetWorldData(scene);
            if (worldData == null) return;

            currentWorldData = worldData;
            
            
            // On Player spawn show the level ui
            if (worldData.showUIOnJoin)
                ClientsManager.Instance.WaitForLocalClient(client => client.OnCharacterSpawned += OnCharacterSpawned);
            
            OnSceneLoadedEvent.Invoke(scene, mode, worldData);
        }

        private void OnCharacterSpawned(Player player)
        {
            // Show UI stuff on join
            SnNotManager
                .ShowNotification(NotificationTypeNames.WorldMessage,
                    currentWorldData.worldName, currentWorldData.worldDescription, 5f);

            ClientsManager.Instance.LocalClient.OnCharacterSpawned -= OnCharacterSpawned;
        }

        
        
        public WorldData GetWorldData(string sceneName)
        {
            return worlds.Find(world => world.worldScene == sceneName);
        }
        
        public WorldData GetWorldData(Scene scene)
        {
            return GetWorldData(scene.name);
        }
        
        public WorldData GetWorldData(int index)
        {
            return index >= 0 && index < worlds.Length ? worlds[index] : null;
        }

        [Server]
        public void LoadWorld(WorldData worldData, int delay, bool showCountdown)
        {
            if (worldData == null) return;
            
            // Do countdown then load scene
            RpcLoadCountdown(worlds.IndexOf(worldData), delay, showCountdown);
        }
        
        [Server]
        public void LoadWorld(int worldDataIndex, int delay, bool showCountdown)
        {
            WorldData worldData = GetWorldData(worldDataIndex);
            Debug.Log("Loading world: " + worldData);
            if (worldData == null) return;
            
            // Do countdown then load scene
            RpcLoadCountdown(worldDataIndex, delay, showCountdown);
        }
        
        [ClientRpc]
        public void RpcLoadCountdown(int index, int delay, bool showCountdown)
        {
            if (index < 0 || index >= worlds.Length) return;
            
            WorldData worldData = worlds[index];
            if (worldData == null) return;
            
            StartCoroutine(Countdown(worldData, delay, showCountdown));
        }
        
        IEnumerator Countdown(WorldData worldData, int delay, bool showCountdown)
        {
            if (showCountdown)
            {
                // Show countdown UI
                SnNotManager.ShowNotification(NotificationTypeNames.Title,
                    worldData.worldName, $"Teleporting in {delay}", delay + 1);

                for (int i = delay; i > 0; i--)
                {
                    SnNotManager.EditCurrentNotification(NotificationTypeNames.Title, worldData.worldName, $"Teleporting in {i}");
                    yield return new WaitForSeconds(1);
                }
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }
            
            // Hide screen
            LoadingPanel.Instance.Show("");
            yield return LoadingPanel.Instance.FadeInRoutine();
            yield return new WaitForSeconds(1.5f);
            
            if (isServer)
            {
                // change the scene on the server
                NetworkManager.singleton.ServerChangeScene(worldData.worldScene);
            }
        }
    }
}