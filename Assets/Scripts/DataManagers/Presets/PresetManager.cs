using System.Collections;
using Firebase;
using Firebase.Game;
using Networking;
using Snowy.Utils;
using UnityEngine;

namespace DataManagers.Presets
{
    public class PresetManager : MonoSingleton<PresetManager>
    {
        public delegate void PresetChanged(Preset preset);
        public event PresetChanged OnPresetChanged;
        
        private Preset m_currentPreset;
        public Preset CurrentPreset {
            get => m_currentPreset;
            private set {
                m_currentPreset = value;
                IsReady = true;
            }
        }
        
        public bool IsReady { get; set; }

        private void Start()
        {
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            yield return new WaitUntil(() => UserController.Instance?.IsReady == true && SteamNetworkManager.Instance?.IsReady == true);
            LoadPreset();
        }

        private void LoadPreset()
        {
            // For now from local data
            if (PlayerPrefs.HasKey("Preset"))
            {
                CurrentPreset = JsonUtility.FromJson<Preset>(PlayerPrefs.GetString("Preset"));
            }
            else
            {
                CurrentPreset = new Preset
                {
                    PrimaryWeaponId = 0,
                    SecondaryWeaponId = -1
                };
                
                PlayerPrefs.SetString("Preset", JsonUtility.ToJson(CurrentPreset));
            }
            
            OnPresetChanged?.Invoke(CurrentPreset);
            
            Debug.Log("Preset is ready to use!");
        }
        
        public void SavePreset()
        {
            PlayerPrefs.SetString("Preset", JsonUtility.ToJson(CurrentPreset));
            OnPresetChanged?.Invoke(CurrentPreset);
        }
        
        public void SetPreset(Preset preset)
        {
            CurrentPreset = preset;
            SavePreset();
        }
        
        public void SetPrimaryWeapon(int weaponId)
        {
            SetPreset(new Preset
            {
                PrimaryWeaponId = weaponId,
                SecondaryWeaponId = CurrentPreset.SecondaryWeaponId
            });
        }
        
        public void SetSecondaryWeapon(int weaponId)
        {
            SetPreset(new Preset
            {
                PrimaryWeaponId = CurrentPreset.PrimaryWeaponId,
                SecondaryWeaponId = weaponId
            });
        }
    }
}