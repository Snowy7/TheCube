using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Snowy.UI
{
    public class SnSelector : MonoBehaviour
    {
        [SerializeField] private SnButton leftButton;
        [SerializeField] private SnButton rightButton;
        [SerializeField] private TMP_Text text;
        [SerializeField] private bool loop;
        public UnityEvent onValueChanged;
        
        private int m_index;
        private string[] m_options = {"Option 1", "Option 2", "Option 3"};
        
        private void Start()
        {
            leftButton.OnClick.AddListener(OnLeftButtonClicked);
            rightButton.OnClick.AddListener(OnRightButtonClicked);
            UpdateText();
        }
        
        private void OnLeftButtonClicked()
        {
            m_index--;
            if (m_index < 0)
            {
                m_index = loop ? m_options.Length - 1 : 0;
            }
            UpdateText();
        }
        
        private void OnRightButtonClicked()
        {
            m_index++;
            if (m_index >= m_options.Length)
            {
                m_index = loop ? 0 : m_options.Length - 1;
            }
            UpdateText();
        }
        
        private void UpdateText()
        {
            if (m_options.Length == 0) return;
            
            if (m_options.Length <= m_index)
            {
                m_index = m_options.Length - 1;
            }
            
            text.text = m_options[m_index];

            if (!loop)
            {
                leftButton.Interactable = m_index > 0;
                rightButton.Interactable = m_index < m_options.Length - 1;
            }
            
            onValueChanged.Invoke();
        }
        
        public void SetOptions(string[] options)
        {
            m_options = options;
            m_index = 0;
            UpdateText();
        }
        
        public void SetIndex(int index)
        {
            m_index = index;
            UpdateText();
        }
        
        public void SetIndex(string option)
        {
            m_index = System.Array.IndexOf(m_options, option);
            UpdateText();
        }
        
        public string GetSelectedOption()
        {
            return m_options[m_index];
        }
        
        public int GetSelectedIndex()
        {
            return m_index;
        }
    }
}