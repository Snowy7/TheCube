using System;
using System.Collections.Generic;
using Snowy.Utils;
using UnityEngine;

namespace SnNotification
{
    public class SnNotManager : MonoSingleton<SnNotManager>
    {
        public override bool DestroyOnLoad => false;
        
        [SerializeField] private SnNotTypesContainer notificationsContainer;
        
        // Pool
        private Dictionary<NotificationTypeNames, SnNotificationPool> m_pools = new ();
        
        // Init the pools
        protected override void Awake()
        {
            m_pools = new Dictionary<NotificationTypeNames, SnNotificationPool>();
            
            foreach (var notificationType in notificationsContainer.notificationTypes)
            {
                string typeName = notificationType.name;
                int poolSize = notificationType.poolSize;
                SnNotObject[] notifications = new SnNotObject[poolSize];
                
                // Init the layout group
                Transform layoutGroup = transform;
                // Old code when there was a layout group prefab
                if (notificationType.layoutGroupPrefab && notificationType.hasLayoutGroup)
                {
                    layoutGroup = Instantiate(notificationType.layoutGroupPrefab, transform).transform;
                    layoutGroup.localScale = notificationType.layoutGroupPrefab.transform.localScale;
                    layoutGroup.localPosition = notificationType.layoutGroupPrefab.transform.localPosition;
                    layoutGroup.localRotation = notificationType.layoutGroupPrefab.transform.localRotation;
                    layoutGroup.gameObject.SetActive(false);
                }
                
                for (int i = 0; i < poolSize; i++)
                {
                    notifications[i] = Instantiate(notificationType.notificationObject, layoutGroup);
                    notifications[i].transform.localScale = notificationType.notificationObject.transform.localScale;
                    notifications[i].transform.localPosition = notificationType.notificationObject.transform.localPosition;
                    notifications[i].transform.localRotation = notificationType.notificationObject.transform.localRotation;
                    notifications[i].gameObject.SetActive(false);
                    
                }
                m_pools.Add((NotificationTypeNames) Enum.Parse(typeof(NotificationTypeNames), typeName), new SnNotificationPool(notifications, layoutGroup.gameObject));
            }
            
            base.Awake();
        }
        
        private SnNotObject ShowNot(NotificationTypeNames type, SnNotData data)
        {
            SnNotificationPool pool = m_pools[type];
            SnNotObject notification = pool.GetNext();
            // make sure the layout group is active
            if (notification && !notification.transform.parent.gameObject.activeSelf)
                notification.transform.parent.gameObject.SetActive(true);
            
            if (notification)
                notification.ShowNotification(data);
            
            return notification;
        }
        
        private SnNotObject EditCurrentNot(NotificationTypeNames type, SnNotData data)
        {
            SnNotificationPool pool = m_pools[type];
            SnNotObject notification = pool.GetCurrent();

            if (notification)
            {
                notification.EditNotification(data);
            }
            
            return notification;
        }
        
        public static SnNotObject ShowNotification(NotificationTypeNames type, string title, string content = "", float duration = 3f)
        {
            if (!Instance)
            {
                Debug.LogError("SnNotManager is not found in the scene.");
                return null;
            }
            
            return Instance.ShowNot(type, new SnNotData
            {
                title = title,
                content = content,
                duration = duration
            });
        }
        
        public static SnNotObject ShowNotification(NotificationTypeNames type, SnNotData data)
        {
            if (!Instance)
            {
                Debug.LogError("SnNotManager is not found in the scene.");
                return null;
            }
            
            return Instance.ShowNot(type, data);
        }
        
        public static SnNotObject EditCurrentNotification(NotificationTypeNames type, string title, string content = "")
        {
            if (!Instance)
            {
                Debug.LogError("SnNotManager is not found in the scene.");
                return null;
            }
            
            return Instance.EditCurrentNot(type, new SnNotData
            {
                title = title,
                content = content
            });
        }
        
        public static SnNotObject EditCurrentNotification(NotificationTypeNames type, SnNotData data)
        {
            if (!Instance)
            {
                Debug.LogError("SnNotManager is not found in the scene.");
                return null;
            }
            
            return Instance.EditCurrentNot(type, data);
        }

    }
}