using System.Collections.Generic;
using System.Linq;
using Actors.Player;
using DataManagers.Presets;
using Game;
using Game.WorldSystem;
using Mirror;
using Networking;
using Snowy.Engine;
using SnTerminal;
using UnityEngine;

// Want to make it so there is two types of storage.
// One is the weapons.
// The other is the items.
// The weapons are the ones that are equipped.
// The items are the ones that are stored in the inventory.
// The weapons are stored in a list.
// The items are stored in a dictionary.

namespace Inventory
{
    public struct NetworkItem
    {
        public int id;
        public int amount;
    }
    
    public class PlayerInventory : NetworkBehaviour
    {
        public event System.Action<int> OnEquip;
        
        # region Serialized Fields
        
        [Header("References")]
        [SerializeField] private Transform localWeaponParent;
        [SerializeField] private Transform remoteWeaponParent;
        
        [Header("Settings")]
        [SerializeField] private int maxWeapons = 2;
        [SerializeField] private int maxItems = 10;
        
        # endregion
        
        # region Fields
        
        /// <summary>
        /// Currently equipped index.
        /// </summary>
        [SyncVar(hook = nameof(OnEquipped)), SerializeField] int equippedIndex = -1;
        
        [SerializeField] private SyncDictionary<int, NetworkItem> m_syncedItems = new();
        
        /// <summary>
        /// Array of all weapons. These are gotten in the order that they are parented to this object.
        /// </summary>
        private List<Weapon> m_weapons = new();
        
        /// <summary>
        /// Currently equipped WeaponBehaviour.
        /// </summary>
        private Weapon m_equipped;

        private Weapon Equipped
        {
            get
            {
                if ((m_equipped == null && equippedIndex >= 0 && equippedIndex < m_weapons.Count) || 
                    (equippedIndex >= 0 && equippedIndex < m_weapons.Count && m_equipped != m_weapons[equippedIndex]))
                    m_equipped = m_weapons[equippedIndex];
                return m_equipped;
            }
        }
        
        private FPSCharacter m_character;
        
        # endregion
        
        # region Network Behaviour
        
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!m_character) m_character = GetComponentInParent<FPSCharacter>();
            m_syncedItems.OnChange += OnItemsChanged;
            
            foreach (var item in m_syncedItems)
            {
                OnItemsChanged(SyncDictionary<int, NetworkItem>.Operation.OP_ADD, item.Key, item.Value);
            }
        }
        
        public override void OnStopClient()
        {
            base.OnStopClient();
            m_syncedItems.OnChange -= OnItemsChanged;
        }
        
        # endregion
        
        public void Init(FPSCharacter fpsCharacter, int equippedAtStart = 0)
        {
            //Cache character.
            m_character = fpsCharacter;
            
            //Cache all weapons. Beware that weapons need to be parented to the object this component is on!
            m_weapons = GetComponentsInChildren<Weapon>(true).ToList();
            
            //Disable all weapons. This makes it easier for us to only activate the one we need.
            Debug.Log($"Disabling all weapons, count: {m_weapons.Count}");
            foreach (Weapon weapon in m_weapons)
                weapon.gameObject.SetActive(false);
            
            // Hide the hands if we have no weapons.
            if (m_weapons.Count == 0 || m_equipped == null)
                m_character.TriggerArms(false);
            
            // Equip the default preset if can
            Debug.Log($"Equip the default preset if can, isOwned: {isOwned}, WorldsManager.Instance.CurrentWorldData?.spawnWithWeapons: {WorldsManager.Instance.CurrentWorldData?.spawnWithWeapons}");
            if (isOwned && WorldsManager.Instance.CurrentWorldData?.spawnWithWeapons == true)
            {
                // Equip the default preset
                UsePreset(0);
            }
            
            //Equip.
            Equip(equippedAtStart);
        }
        
        public void OnEquipped(int oldIndex, int newIndex)
        {
            // if not owner
            // equip the new weapon
            if (newIndex > -1 && newIndex < m_weapons.Count && m_weapons[newIndex] != null)
            {
                Equip(newIndex);
            }
        }
        
        private void OnItemsChanged(SyncDictionary<int, NetworkItem>.Operation op, int key, NetworkItem item)
        {
            // when adding or removing items, update physical inventory
            if (op == SyncDictionary<int, NetworkItem>.Operation.OP_ADD)
            {
                // add item to inventory
                AddWeaponToInventory(item.id);
            } else if (op == SyncDictionary<int, NetworkItem>.Operation.OP_REMOVE)
            {
                // remove item from inventory
                Debug.Log($"Removed {item.amount}x {Global.GetWeapon(item.id).GetWeaponName()}");
            }
        }
        
        # region Weapon Management
        
        public Weapon Equip(int index)
        {
            Debug.Log($"Equipping weapon at index {index}");
            //If we have no weapons, we can't really equip anything.
            if (m_weapons == null)
                return m_equipped;
            
            //The index needs to be within the array's bounds.
            if (index > m_weapons.Count - 1)
                return m_equipped;

            //No point in allowing equipping the already-equipped weapon.
            if (equippedIndex == index)
                return m_equipped;
            
            // disable all weapons
            Debug.Log("Disabling all weapons 2");
            foreach (Weapon weapon in m_weapons)
                weapon.gameObject.SetActive(false);

            //Update index.
            equippedIndex = index;
            //Update equipped.
            m_equipped = m_weapons[equippedIndex];
            //Activate the newly-equipped weapon.
            m_equipped.gameObject.SetActive(true);
            
            //Notify.
            OnEquip?.Invoke(equippedIndex);
            
            //Return.
            return m_equipped;
        }
        
        [Server]
        private void AddWeaponToSynced(int id)
        {
            Weapon weapon = Global.GetWeapon(id);
            if (weapon == null)
                return;

            if (m_syncedItems.ContainsKey(id))
            {
                // already have this item, increase amount
                Debug.Log($"You already have {weapon.GetWeaponName()}");
            } else
            {
                // add new item
                m_syncedItems.Add(id, new NetworkItem { id = id, amount = 1 });
            }
        }
        
        [Command]
        private void CmdAddWeaponToSynced(int id)
        {
            AddWeaponToSynced(id);
        }
        
        private void AddWeaponToInventory(int id)
        {
            Weapon weapon = Global.GetWeapon(id);
            if (weapon == null)
                return;

            // Spawn the weapon in the world
            if (isOwned)
            {
                Weapon newWeapon = Instantiate(weapon, localWeaponParent);
                newWeapon.transform.localPosition = Vector3.zero;
                newWeapon.transform.localRotation = Quaternion.identity; 
                newWeapon.transform.SetLayer(Global.GetFpViewLayerMask);
                newWeapon.gameObject.SetActive(false);
                
                // Add the weapon to the inventory
                m_weapons.Add(newWeapon);
                
                // Equip the weapon if we have none
                // Show the hands
                
                if (m_equipped == null && (equippedIndex == -1 || equippedIndex == m_weapons.Count - 1))
                {
                    m_character.TryEquipWeapon(m_weapons.Count - 1);
                }
            }
            else
            {
                Weapon newWeapon = Instantiate(weapon, remoteWeaponParent);
                newWeapon.transform.localPosition = Vector3.zero;
                newWeapon.transform.localRotation = Quaternion.identity;
                newWeapon.gameObject.SetActive(false);
                newWeapon.transform.SetLayer(Global.GetTpViewLayerMask);
                newWeapon.UseBakedData();

                // Add the weapon to the inventory
                m_weapons.Add(newWeapon);
                
                // Equip the weapon if we have none
                // Show the hands
                if (m_equipped == null && (equippedIndex == -1 || equippedIndex == m_weapons.Count - 1))
                {
                    m_character.TryEquipWeaponTp(m_weapons.Count - 1);
                    
                    newWeapon.gameObject.SetActive(true);
                }
            }
        }
        
        public void AddWeapon(int id)
        {
            if (isServer)
            {
                AddWeaponToSynced(id);
            } else
            {
                CmdAddWeaponToSynced(id);
            }
        }
        
        # endregion
        
        #region Getters

        public int GetLastIndex()
        {
            //Get last index with wrap around.
            int newIndex = equippedIndex - 1;
            if (newIndex < 0)
                newIndex = m_weapons.Count - 1;

            //Return.
            return newIndex;
        }

        public int GetNextIndex()
        {
            //Get next index with wrap around.
            int newIndex = equippedIndex + 1;
            if (newIndex > m_weapons.Count - 1)
                newIndex = 0;

            //Return.
            return newIndex;
        }

        public Weapon GetEquipped() => Equipped;
        
        public int GetEquippedIndex() => equippedIndex;
        
        public List<Weapon> GetWeapons() => m_weapons;

        #endregion
        
        # region Commands
        
        [RegisterCommand(Name = "add_weapon", Help = "Adds a weapon to the player's inventory", MinArgCount = 1, MaxArgCount = 1)]
        public static void CommandAddWeapon(CommandArg[] args)
        {
            if (args.Length < 1)
            {
                Terminal.Log("Usage: add_weapon <weapon_id>");
                return;
            }

            if (!int.TryParse(args[0].String, out int id))
            {
                Terminal.Log("Invalid weapon id.");
                return;
            }

            if (ClientsManager.Instance.LocalClient?.Player != null)
            {
                PlayerInventory inventory = ClientsManager.Instance.LocalClient.Player.Character.GetInventory();
                if (inventory == null)
                {
                    Terminal.Log("Player inventory not found.");
                    return;
                }

                inventory.AddWeapon(id);
            }
        }
        
        # endregion

        public void UsePreset(int presetId)
        {
            // Make sure it us, the owner, before applying the preset
            Debug.Log($"Using preset {presetId}, isOwned: {isOwned}, PresetManager: {PresetManager.Instance}");
            if (!isOwned) return;
            
            // Get the preset
            Preset preset = PresetManager.Instance.CurrentPreset;
            Debug.Log($"Preset: {preset}, primary: {preset.PrimaryWeaponId}, secondary: {preset.SecondaryWeaponId}");
            
            // Equip the primary weapon
            if (preset.PrimaryWeaponId >= 0)
            {
                AddWeapon(preset.PrimaryWeaponId);
            }
            
            // Equip the secondary weapon
            if (preset.SecondaryWeaponId >= 0)
            {
                AddWeapon(preset.SecondaryWeaponId);
            }
        }
    }
}