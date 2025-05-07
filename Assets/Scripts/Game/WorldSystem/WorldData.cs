using Inventory;
using UnityEngine;

namespace Game.WorldSystem
{
    [CreateAssetMenu(fileName = "WorldData", menuName = "Game/WorldSystem/WorldData")]
    public class WorldData : ScriptableObject
    {
        [Title("World Data (UI Stuff)")]
        public string worldName;
        public string worldDescription;
        public Sprite worldImage;
        
        [Title("World Settings")]
        [Tooltip("If true, the side UI showing the world name and description will be shown when the player joins this world.")]
        public bool showUIOnJoin;
        [Tooltip("If true, the player will spawn automatically in this world.")]
        public bool autoPlayerSpawn;
        [Tooltip("If true, the player will spawn with weapons in this world.")]
        public bool spawnWithWeapons;
        [Title("The scene name in which the world is located.")]
        [SceneName] public string worldScene;
        
        [Title("Items")]
        [Tooltip("All the weapons for this world."), ReorderableList, InLineEditor]
        public WeaponItem[] weapons;
    }
}