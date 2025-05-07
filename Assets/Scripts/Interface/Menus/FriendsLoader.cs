using System;
using System.Collections.Generic;
using Interface.Menus.Menu_Element;
using Network;
using Snowy.UI;
using Steamworks;
using UnityEngine;

namespace Interface.Menus
{
    public struct FriendHolder
    {
        public SteamFriend friend;
        public FriendItem friendItem;
    }

    public class FriendsLoader : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private FriendItem friendItemPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private SnButton nextPageButton;
        [SerializeField] private SnButton previousPageButton;

        [Title("Pages")]
        [SerializeField] private int friendsPerPage = 10;
        [Disable, SerializeField] private int currentPage = 1;
        [Disable, SerializeField] private int previousPage = 1;
        
        private HashSet<SteamFriend> m_friends = new();
        private Dictionary<int, List<FriendHolder>> m_friendsPages = new();

        private void Awake()
        {
            nextPageButton.OnClick.AddListener(NextPage);
            previousPageButton.OnClick.AddListener(PreviousPage);
        }

        private void OnDestroy()
        {
            nextPageButton.OnClick.RemoveListener(NextPage);
            previousPageButton.OnClick.RemoveListener(PreviousPage);
        }

        private void OnEnable()
        {
            // Load friends
            LoadFriends();
        }

        private void LoadFriends()
        {
            var friends = SteamFriendsManager.GetFriends();
            // sort them offline last and online first after sorting alphabetically
            Array.Sort(friends, (a, b) =>
            {
                var statusA = SteamFriendsManager.GetFriendStatus(a.SteamID);
                var statusB = SteamFriendsManager.GetFriendStatus(b.SteamID);

                if (statusA == EPersonaState.k_EPersonaStateOffline && statusB != EPersonaState.k_EPersonaStateOffline)
                    return 1;
                if (statusA != EPersonaState.k_EPersonaStateOffline && statusB == EPersonaState.k_EPersonaStateOffline)
                    return -1;

                return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
            });
            
            // Create friend items
            foreach (var friend in friends)
            {
                // if the friend is the current user, skip
                if (friend.SteamID == SteamUser.GetSteamID())
                    continue;
                
                // if already added, skip
                if (m_friends.Contains(friend))
                    continue;
                
                var friendItem = Instantiate(friendItemPrefab, content);
                friendItem.SetFriend(friend);
                
                AddFriend(friend, friendItem);
                
                // disable friend item if it's not on the current page
                friendItem.gameObject.SetActive(false);
            }
            
            UpdatePage();
        }
        
        private void AddFriend(SteamFriend friend, FriendItem friendItem)
        {
            m_friends.Add(friend);
            
            var page = Mathf.CeilToInt(m_friends.Count / (float) friendsPerPage);
            if (!m_friendsPages.ContainsKey(page))
                m_friendsPages.Add(page, new List<FriendHolder>());
            
            m_friendsPages[page].Add(new FriendHolder { friend = friend, friendItem = friendItem });
        }
        
        private void UpdatePage()
        {
            if (!m_friendsPages.ContainsKey(currentPage))
                return;
            
            // Loop through all friends and set them active or inactive based on the current page
            foreach (var friend in m_friendsPages[previousPage])
                friend.friendItem.gameObject.SetActive(false);

            foreach (var friend in m_friendsPages[currentPage])
                friend.friendItem.gameObject.SetActive(true);
            
            // Disable buttons if there are no more pages
            nextPageButton.gameObject.SetActive(currentPage < m_friendsPages.Count);
            previousPageButton.gameObject.SetActive(currentPage > 1);
        }
        
        public void NextPage()
        {
            if (currentPage >= m_friendsPages.Count)
                return;

            previousPage = currentPage;
            
            currentPage++;
            UpdatePage();
        }
        
        public void PreviousPage()
        {
            if (currentPage <= 1)
                return;

            previousPage = currentPage;
            
            currentPage--;
            UpdatePage();
        }
    }
}