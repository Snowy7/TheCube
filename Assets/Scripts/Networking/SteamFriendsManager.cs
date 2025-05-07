using Networking;
using Steamworks;
using UnityEngine;

namespace Network
{
    public struct SteamFriend
    {
        public string Name;
        public CSteamID SteamID;
    }
    
    public static class SteamFriendsManager
    {
        public static SteamFriend[] GetFriends()
        {
            int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
            SteamFriend[] friends = new SteamFriend[friendCount];
            for (int i = 0; i < friendCount; i++)
            {
                CSteamID friendSteamID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                friends[i] = new SteamFriend
                {
                    Name = SteamFriends.GetFriendPersonaName(friendSteamID),
                    SteamID = friendSteamID
                };
            }
            
            return friends;
        }
        
        public static string GetFriendName(CSteamID steamID)
        {
            return SteamFriends.GetFriendPersonaName(steamID);
        }
        
        public static void InviteFriendToGame(CSteamID steamID)
        {
            if (!SteamLobby.Instance.IsInLobby)
                return;
            
            var lobby = SteamLobby.Instance.CurrentLobbyId;
            SteamMatchmaking.InviteUserToLobby(new CSteamID(lobby), steamID);
        }
        
        public static EPersonaState GetFriendStatus(CSteamID steamID)
        {
            EPersonaState state = SteamFriends.GetFriendPersonaState(steamID);
            return state;
        }
        
        public static Texture2D GetSteamImageAsTexture2D(int iImage) {
            Texture2D ret = null;
            uint imageWidth;
            uint imageHeight;
            bool bIsValid = SteamUtils.GetImageSize(iImage, out imageWidth, out imageHeight);

            if (bIsValid) {
                byte[] image = new byte[imageWidth * imageHeight * 4];

                bIsValid = SteamUtils.GetImageRGBA(iImage, image, (int)(imageWidth * imageHeight * 4));
                if (bIsValid) {
                    ret = new Texture2D((int)imageWidth, (int)imageHeight, TextureFormat.RGBA32, false, false);
                    // flip the image
                    image = FlipTexture(image, (int)imageWidth, (int)imageHeight);
                    ret.LoadRawTextureData(image);
                    ret.Apply();
                }
            } else {
                Debug.Log("Image not valid");
            }

            return ret;
        }
        
        private static byte[] FlipTexture(byte[] original, int width, int height)
        {
            byte[] flipped = new byte[original.Length];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    flipped[i * width * 4 + j * 4 + 0] = original[(height - i - 1) * width * 4 + j * 4 + 0];
                    flipped[i * width * 4 + j * 4 + 1] = original[(height - i - 1) * width * 4 + j * 4 + 1];
                    flipped[i * width * 4 + j * 4 + 2] = original[(height - i - 1) * width * 4 + j * 4 + 2];
                    flipped[i * width * 4 + j * 4 + 3] = original[(height - i - 1) * width * 4 + j * 4 + 3];
                }
            }

            return flipped;
        }
    }
}