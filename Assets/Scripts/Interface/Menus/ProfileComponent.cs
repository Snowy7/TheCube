using Firebase.Game;
using Network;
using SnTerminal;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Menus
{
    public class ProfileComponent : MonoBehaviour
    {
        [SerializeField] private Image profileImage;
        [SerializeField] private TMP_Text profileName;
        [SerializeField] private TMP_Text remainingTime;
        
        private void OnEnable()
        {
            // Load the profile picture
            // Load the profile name
            LoadProfile();
            // Load the remaining time
            LoadRemainingTime();
        }
        
        public void LoadProfile()
        {
            profileName.text = SteamFriends.GetPersonaName();
            
            var steamID = SteamUser.GetSteamID();
            var avatar = SteamFriends.GetLargeFriendAvatar(steamID);
            if (avatar == -1)
            {
                Terminal.Log(TerminalLogType.Warning, "Failed to load profile image.");
                return;
            }
            var image = SteamFriendsManager.GetSteamImageAsTexture2D(avatar);
            
            var sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            
            profileImage.sprite = sprite;
        }
        
        public void LoadRemainingTime()
        {
            var time = UserController.GetRemainingUntilReset();
            remainingTime.text = $"{time} before death!";
        }
    }
}