using System.Collections.Generic;
using Network;
using SnNotification;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Menus.Menu_Element
{
    public class FriendItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text friendName;
        [SerializeField] private Image friendImage;

        [SerializeField] private Color onlineColor;
        [SerializeField] private Color offlineColor;

        private SteamFriend m_friend;
        private bool m_isOnline;

        public void SetFriend(SteamFriend friend)
        {
            friendName.text = friend.Name;
            var status = SteamFriendsManager.GetFriendStatus(friend.SteamID);

            var avatar = SteamFriends.GetLargeFriendAvatar(friend.SteamID);
            var image = SteamFriendsManager.GetSteamImageAsTexture2D(avatar);

            if (image != null)
            {
                var sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
                friendImage.sprite = sprite;

                friendImage.type = Image.Type.Simple;
                friendImage.preserveAspect = true;
            }

            m_isOnline = status != EPersonaState.k_EPersonaStateOffline;
            friendName.color = friendImage.color = m_isOnline ? onlineColor : offlineColor;

            m_friend = friend;
        }
        
        public void ShowInviteModal()
        {
            List<SnButtonData> buttons = new List<SnButtonData>();

            if (m_friend.SteamID != SteamUser.GetSteamID() && m_isOnline) 
            {
                buttons.Add(
                    new()
                    {
                        text = "Invite",
                        closeOnClick = true,
                        onClick = () => {
                            SteamFriendsManager.InviteFriendToGame(m_friend.SteamID);
                        }
                    });
            }
            
            buttons.Add(
                new()
                {
                    text = "Show Steam Profile",
                    closeOnClick = true,
                    onClick = () =>
                    {
                        SteamFriends.ActivateGameOverlayToUser("steamid", m_friend.SteamID);
                    }
                });
            
            
            SnNotManager.ShowNotification(NotificationTypeNames.Modal,
                new()
                {
                    title = "Player Profile",
                    content = $"{friendName.text}\n" +
                              $"{(m_isOnline ? "<color=green>Online</color>" : "<color=red>Offline</color>")}",
                    duration = -1,
                    icon = friendImage.sprite,
                    buttons = buttons.ToArray()
                });
        }
    }
}