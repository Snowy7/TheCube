using TMPro;
using UnityEngine;

namespace Networking.Chat
{
    public class ChatMessage : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        
        public void SetMessage(string message)
        {
            text.text = message;
        }
        
        public void SetColor(Color color)
        {
            text.color = color;
        }
        
        public void SetFontSize(int size)
        {
            text.fontSize = size;
        }

        public void SetUsername(string username)
        {
            text.text = $"<b>[{username}]</b>: {text.text}";
        }
    }
}