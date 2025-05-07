using System.Collections.Generic;
using System.Linq;
using Firebase.Game;
using Snowy.Utils;

namespace DataManagers.Storage
{
    public class StorageManager : MonoSingleton<StorageManager>
    {
        public List<StorageItem> StorageItems { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            
            UserController.Instance.OnUserDataLoaded += OnUserDataLoaded;
        }
        
        private void OnUserDataLoaded(UserData userData)
        {
            StorageItems = userData.PrivateStorage.Items.ToList();
        }
    }
}