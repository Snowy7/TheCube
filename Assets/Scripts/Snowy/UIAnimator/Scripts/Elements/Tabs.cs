using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Snowy.UI
{
    /// <summary>
    /// Represents a tab in the UI.
    /// </summary>
    [Serializable]
    public class TabElement
    {
        /// <summary>
        /// The button associated with the tab.
        /// </summary>
        public SnButton actionButton;

        /// <summary>
        /// The content GameObject associated with the tab.
        /// </summary>
        public TabUI content;

        /// <summary>
        /// Event triggered when the tab is selected.
        /// </summary>
        public UnityEvent onSelected;
    }
    
    /// <summary>
    /// Manages a collection of tabs in the UI.
    /// </summary>
    public class Tabs : MonoBehaviour
    {
        /// <summary>
        /// Array of tabs managed by this component.
        /// </summary>
        [SerializeField] private TabElement[] tabs;

        /// <summary>
        /// The index of the currently selected tab.
        /// </summary>
        private int m_index;

        /// <summary>
        /// Event triggered when the selected tab index changes.
        /// </summary>
        public UnityEvent<int> onIndexChanged;

        private void Awake()
        {
            for (var i = 0; i < tabs.Length; i++)
            {
                var index = i;
                tabs[i].actionButton?.OnClick.AddListener(() => SetIndex(index));
            }
        }

        /// <summary>
        /// Initializes the tabs and sets the default selected tab.
        /// </summary>
        private void OnEnable()
        {
            if (tabs.Length > 0)
            {
                EnableNewTab(0);
            }
        }



        /// <summary>
        /// Sets the selected tab by index.
        /// </summary>
        /// <param name="index">The index of the tab to select.</param>
        public void SetIndex(int index)
        {
            if (index < 0 || index >= tabs.Length)
                return;
            
            if (index == m_index) return;
            
            // First disable the current tab
            if (tabs[m_index].content)
            {
                tabs[m_index].content.Switch(false);
                float duration = tabs[m_index].content.GetAnimationDuration();
                StartCoroutine(InvokeCallback(() => EnableNewTab(index), duration));
            }
            if (tabs[index].actionButton) tabs[m_index].actionButton.SetSelected(true, true);
            if (tabs[m_index].actionButton) tabs[m_index].actionButton.SetSelected(false);
        }
        
        
        IEnumerator InvokeCallback(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback();
        }

        /// <summary>
        /// Enables the new tab.
        /// </summary>
        /// <param name="index"></param>
        public void EnableNewTab(int index)
        {
            m_index = index;
            
            // turn on
            if (tabs[m_index].content) tabs[m_index].content.Switch(true);
            if (tabs[index].actionButton) tabs[m_index].actionButton.SetSelected(true, true);

            onIndexChanged?.Invoke(m_index);
        }
        
        /// <summary>
        /// Gets the index of the currently selected tab.
        /// </summary>
        /// <returns>The index of the currently selected tab.</returns>
        public int GetIndex() => m_index;
        
        /// <summary>
        /// Sets the tabs managed by this component.
        /// </summary>
        /// <param name="newTabs">The new array of tabs.</param>
        public void SetTabs(TabElement[] newTabs)
        {
            tabs = newTabs;
            
            Awake();
            OnEnable();
        }
        
        /// <summary>
        /// Adds a new tab to the collection.
        /// </summary>
        /// <param name="tabElement">The tab to add.</param>
        public void AddTab(TabElement tabElement)
        {
            Array.Resize(ref tabs, tabs.Length + 1);
            tabs[^1] = tabElement;
            int index1 = tabs.Length - 1;   
            if (tabElement.actionButton) tabElement.actionButton.OnClick.AddListener(() => SetIndex(index1));
            else Debug.LogWarning("Tab action button is null!");
            
            if (tabs.Length == 1)
            {
                EnableNewTab(0);
            }
            else
            {
                // turn off
                if (tabs[index1].content) tabs[index1].content.Switch(false);
                if (tabs[index1].actionButton) tabs[index1].actionButton.SetSelected(false);
            }
            
            // refresh the rendering of the tabs so they are all in the correct state
            for (var i = 0; i < tabs.Length; i++)
            {
                if (tabs[i].actionButton) tabs[i].actionButton.SetSelected(i == m_index);
            }
        }
        
        /// <summary>
        /// Removes a tab from the collection by index.
        /// </summary>
        /// <param name="index">The index of the tab to remove.</param>
        public void RemoveTab(int index)
        {
            if (index < 0 || index >= tabs.Length)
            {
                return;
            }
            var newTabs = new TabElement[tabs.Length - 1];
            for (var i = 0; i < newTabs.Length; i++)
            {
                newTabs[i] = tabs[i < index ? i : i + 1];
            }
            tabs = newTabs;
        }
        
        /// <summary>
        /// Selects the next tab in the collection.
        /// </summary>
        public void SelectNext()
        {
            SetIndex(Mathf.Min(m_index + 1, tabs.Length - 1));
        }
        
        /// <summary>
        /// Selects the previous tab in the collection.
        /// </summary>
        public void SelectPrevious()
        {
            SetIndex(Mathf.Max(m_index - 1, 0));
        }
        
        /// <summary>
        /// Selects the first tab in the collection.
        /// </summary>
        public void SelectFirst()
        {
            SetIndex(0);
        }
        
        /// <summary>
        /// Selects the last tab in the collection.
        /// </summary>
        public void SelectLast()
        {
            SetIndex(tabs.Length - 1);
        }
        
        /// <summary>
        /// Selects a tab by index.
        /// </summary>
        /// <param name="index">The index of the tab to select.</param>
        public void Select(int index)
        {
            SetIndex(index);
        }

        /// <summary>
        /// Clears all tabs from the collection.
        /// </summary>
        public void Clear()
        {
            foreach (var tab in tabs)
            {
                tab.actionButton?.OnClick.RemoveAllListeners();
            }
            
            // Destroy all tabs
            foreach (var tab in tabs)
            {
                if (tab.content) Destroy(tab.content);
                if (tab.actionButton) Destroy(tab.actionButton.gameObject);
            }
            
            // Clear tabs
            tabs = Array.Empty<TabElement>();
        }
    }
}