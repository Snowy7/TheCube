using Snowy.Inspector;
using UnityEngine;

namespace SnNotification.Test
{
    public class NotificationTest : MonoBehaviour
    {
        [SerializeField] NotificationTypeNames notificationType;
        [SerializeField] string testTitle = "Test Title";
        [SerializeField] string testContent = "Test Content";
        [SerializeField] Sprite testIcon;
        [SerializeField] float testDuration = 3f;

        [InspectorButton]
        public void ShowTestNotification()
        {
            SnNotManager.ShowNotification(notificationType, new SnNotData
            {
                title = testTitle,
                content = testContent,
                icon = testIcon,
                duration = testDuration,
                buttons = new SnButtonData[]
                {
                    new()
                    {
                        text = "Test",
                        onClick = () => Debug.Log("Button Clicked")
                    },
                    new()
                    {
                        text = "Test 2",
                        onClick = () => Debug.Log("Button 2 Clicked")
                    }
                }
            });
        }
    }
}