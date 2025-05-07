using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using Networking;
using Snowy.Utils;
using Steamworks;
using UnityEngine;

# if UNITY_EDITOR
using Unity.Multiplayer.Playmode;
# endif

namespace Firebase.Game
{
    /// <summary>
    /// Controller for the user data.
    /// </summary>
    public class UserController : MonoSingleton<UserController>
    {
        public event Action<UserData> OnUserDataLoaded; 
        
        public override bool DestroyOnLoad => false;
        
        /// <summary>
        /// User data for the current user.
        /// </summary>
        public UserData UserData { get; set; }
        
        /// <summary>
        /// Flag to check if the user is ready to use.
        /// </summary>
        public bool IsReady { get; set; }

		private void Start()
        {
            // in editor mode, do not initialize Firestore for clients
# if UNITY_EDITOR
            if (CurrentPlayer.ReadOnlyTags().Contains("Client"))
            {
                return;
            }
# endif
            StartCoroutine(Init());
        }

		/// <summary>
		/// Wait for Firestore and Steam to be ready before initializing the user.
		/// </summary>
		/// <returns></returns>
		IEnumerator Init()
        {
            yield return new WaitUntil(() => FirestoreManager.Instance?.IsReady == true && SteamNetworkManager.Instance?.IsReady == true);
            Debug.Log("User is ready to use!");
            LoadUserData();
        }
        
        /// <summary>
        /// Load the user data from Firestore.
        /// </summary>
        private async void LoadUserData()
        {
            var userId = SteamUser.GetSteamID().m_SteamID;
            await FirestoreManager.Instance.GetDocument<UserData>("users", userId.ToString(), OnLoadUserData);
        }

        /// <summary>
        /// Callback for loading user data.
        /// </summary>
        /// <param name="userData"></param>
        private async void OnLoadUserData(UserData userData)
        {
            var userId = SteamUser.GetSteamID().m_SteamID;
            
            if (userData != null)
            {
                Debug.Log("Loading user data...");
                UserData = userData;
                // update the last login time and username if changed
                if (UserData.Username != SteamFriends.GetPersonaName())
                {
                    UserData.Username = SteamFriends.GetPersonaName();
                }
                UserData.LastLogin = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                
                // Check if the user has an inventory
                if (UserData.PrivateStorage == null)
                {
                    Debug.Log("User has no inventory, creating a new one...");
                    
                    // Create a new inventory for the user
                    var invRef = await AddDefaultInventory(userId.ToString());
                    UserData.InventoryRef = invRef;
                    
                    // Add Default Items
                    await AddDefaultItems();
                }
                else
                {
                    // Check if the user does not have any items
                    if (UserData.PrivateStorage.Items.Length == 0)
                    {
                        Debug.Log("User has no items, adding default items...");
                        await AddDefaultItems();
                    }
                }
                
                await FirestoreManager.Instance.UpdateDocument("users", UserData.Id, UserData);
                
                Debug.Log("User data loaded successfully!");
            }
            else
            {
                Debug.Log("User data not found, creating a new user...");
                // Create a new inventory for the user
                var invRef = await AddDefaultInventory(userId.ToString());
                Debug.Log("Inventory created successfully!");
                
                // default ttl is 7 days from now
                var ttl = DateTime.Now.AddDays(14);
                Debug.Log($"Gave the user 14 days to live: {ttl}");
                
                // create a new user
                UserData = new UserData
                {
                    Id = userId.ToString(),
                    Username = SteamFriends.GetPersonaName(),
                    Level = 1,
                    Experience = 0,
                    // ttl to int
                    TimeToLive = ttl.ToString("yyyy-MM-dd HH:mm:ss"),
                    LastLogin = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    InventoryRef = invRef
                };
                    
                await FirestoreManager.Instance.AddDocument("users", UserData);
                
                // Add Default Items
                await AddDefaultItems();
                Debug.Log("New user created successfully!");
            }
                
            OnUserDataLoaded?.Invoke(UserData);
            IsReady = true;
        }
        
        private Task AddDefaultItems()
        {
            var items = new[]
            {
                new StorageItem()
                {
                    Id = 0,
                    Metadata = "0:0:0",
                    Type = (int)ItemType.Weapon,
                    Quantity = 1
                }
            };
            
            return AddItemsToInventory(items);
        }
        
        /// <summary>
        /// Add a default inventory for the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private Task<DocumentReference> AddDefaultInventory(string userId)
        {
            var inventory = new PrivateStorage
            {
                Id = userId,
                Items = new StorageItem[0]
            };
            
            return FirestoreManager.Instance.AddDocument("inventories", inventory);
        }

        /// <summary>
        /// Get the user's death date and time.
        /// If the user data is null, return the current date and time.
        /// </summary>
        /// <example>UserController.Instance.GetDeathDateTime(); // 2069-11-31 12:69:69</example>
        /// <returns>DateTime</returns>
        public DateTime GetDeathDateTime()
        {
            if (UserData == null) return DateTime.Now;
            
            var ttl = UserData.TimeToLive;
            var date = DateTime.Parse(ttl);
            return date;
        }
        
        public async Task AddItemsToInventory(StorageItem[] items)
        {
            UserData.PrivateStorage.AddRange(items);
            await FirestoreManager.Instance.UpdateDocument("inventories", UserData.PrivateStorage.Id, UserData.PrivateStorage);
        }
        
        /// <summary>
        /// Add an item to the user's inventory.
        /// </summary>
        /// <param name="item"></param>
        public async Task AddItemToInventory(StorageItem item)
        {
            UserData.PrivateStorage.AddItem(item);
            await FirestoreManager.Instance.UpdateDocument("inventories", UserData.PrivateStorage.Id, UserData.PrivateStorage);
        }
        
        /// <summary>
        /// Remove an item from the inventory.
        /// </summary>
        /// <param name="item"></param>
        public async Task RemoveItemFromInventory(StorageItem item)
        {
            UserData.PrivateStorage.RemoveItem(item);
            await FirestoreManager.Instance.UpdateDocument("inventories", UserData.PrivateStorage.Id, UserData.PrivateStorage);
        }
        
        /// <summary>
        /// Update an item in the inventory.
        /// </summary>
        /// <param name="item"></param>
        public async Task UpdateItemInInventory(StorageItem item)
        {
            UserData.PrivateStorage.UpdateItem(item);
            await FirestoreManager.Instance.UpdateDocument("inventories", UserData.PrivateStorage.Id, UserData.PrivateStorage);
        }
        
        /// <summary>
        /// Update the user data in Firestore.
        /// </summary>
        public async Task UpdateUserData()
        {
            await FirestoreManager.Instance.UpdateDocument("users", UserData.Id, UserData);
        }
        
        /// <summary>
        /// Delete the user data from Firestore.
        /// </summary>
        /// <param name="userData"></param>
        public async Task DeleteUserData(UserData userData)
        {
            await FirestoreManager.Instance.DeleteDocument("users", userData.Id);
        }
        
        /// <summary>
        /// Get the user data from Firestore.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserData> GetUserData(string userId)
        {
            return await FirestoreManager.Instance.GetDocument<UserData>("users", userId, null);
        }
        
        /// <summary>
        /// Update the time to live for the user.
        /// </summary>
        /// <param name="ttl"></param>
        public async Task UpdateTimeToLive(DateTime ttl)
        {
            UserData.TimeToLive = ttl.ToString("yyyy-MM-dd HH:mm:ss");
            await FirestoreManager.Instance.UpdateDocument("users", UserData.Id, UserData);
        }
        
        public static string GetTimeToLive()
        {
            // to days:hours:minutes:seconds
            return GetTimeToLive(Instance.UserData);
        }
        
        public static string GetTimeToLive(UserData userData)
        {
            // to days:hours:minutes:seconds
            var ttl = userData.TimeToLive;
            var date = DateTime.Parse(ttl);
            var diff = date - DateTime.Now;
            return $"{diff.Days}:{diff.Hours}:{diff.Minutes}:{diff.Seconds}";
        }
        
        public static string GetRemainingUntilReset()
        {
            // to hours:minutes:seconds
            return GetRemainingUntilReset(Instance.UserData);
        }
        
        public static string GetRemainingUntilReset(UserData userData)
        {
            // to hours
            var ttl = userData.TimeToLive;
            var date = DateTime.Parse(ttl);
            var diff = date - DateTime.Now;
            
            // if more than 24 hrs return days else return hours
            if (diff.Days > 0)
            {
                return diff.Days.ToString() + " days";
            }
            
            // if more than 1 hr return hours else return minutes
            if (diff.Hours > 0)
            {
                return diff.Hours.ToString() + " hours";
            }
            
            // if more than 1 min return minutes else return seconds
            if (diff.Minutes > 0)
            {
                return diff.Minutes.ToString() + " minutes";
            }
            
            return diff.Seconds.ToString() + " seconds";
        }
    }
}