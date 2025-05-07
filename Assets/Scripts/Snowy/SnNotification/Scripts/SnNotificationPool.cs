using UnityEngine;

namespace SnNotification
{
    public class SnNotificationPool
    {
        private readonly SnNotObject[] m_notifications;
        private readonly GameObject m_layout;
        private int m_index;

        public SnNotificationPool(SnNotObject[] notifications, GameObject layout)
        {
            m_notifications = notifications;
            m_layout = layout;
            m_index = 0;
            
            foreach (var notification in m_notifications)
            {
                notification.OnHide += OnNotificationHide;
            }
        }

        public SnNotObject GetNext()
        {
            SnNotObject notification = m_notifications[m_index];
            m_index = (m_index + 1) % m_notifications.Length;
            return notification;
        }
        
        // get current notification
        public SnNotObject GetCurrent()
        {
            return m_notifications[m_index];
        }
        
        // On Notification Hide
        public void OnNotificationHide()
        {
            // check if no notification is active
            foreach (var notification in m_notifications)
            {
                if (notification.gameObject.activeSelf)
                {
                    return;
                }
            }
            
            // hide layout
            m_layout.SetActive(false);
        }
    }
}