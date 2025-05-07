using System.Collections;
using System.Collections.Generic;
using Interface;
using Snowy.UI;
using Snowy.UI.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SnInput;

namespace SnNotification
{
    public struct SnButtonData
    {
        public string text;
        public bool closeOnClick;
        public Sprite icon;
        public System.Action onClick;
    }
    
    public struct SnNotData
    {
        public string title;
        public string content;
        public float duration;
        public Sprite icon;
        public SnButtonData[] buttons;
    }
    
    public class SnNotObject : MonoBehaviour, IEffectsManager
    {
        [Title("Main Elements")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text contentText;
        [SerializeField] private Graphic container;
        
        [Title("Images")]
        [SerializeField] private bool canUseImages;
        [SerializeField, ShowIf(nameof(canUseImages), true)] private Image iconImage;
        
        [Title("Buttons")]
        [SerializeField] private bool canUseButtons;
        [SerializeField, ShowIf(nameof(canUseButtons), true)] private GameObject buttonsContainer;
        [SerializeField, ShowIf(nameof(canUseButtons), true)] private SnButton buttonPrefab;

        [Title("Settings")]
        [SerializeField, Tooltip("If true, the notification can be dismissed by clicking the escape key")] private bool dismissable;
        
        [Title("Effects")]
        [SerializeField] private EffectsCollection onShowEffects;
        [SerializeField] private EffectsCollection onHideEffects;
        
        public MonoBehaviour Mono => this;
        public Transform Transform => container ? container.transform : transform;
        public Graphic TargetGraphic => container;
        
        private bool m_isShowing;
        public bool IsShowing => m_isShowing;
        
        private List<SnButton> m_buttons = new();
        
        public event System.Action OnHide;

        protected virtual void OnEnable()
        {
            if (onShowEffects != null)
            {
                onShowEffects.Initialize(this);
            }
            
            if (onHideEffects != null)
            {
                onHideEffects.Initialize(this);
            }

            if (titleText) titleText.text = "";
            if (contentText) contentText.text = "";
        }
        
        private void OnLockCursor(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
            {
                StopAllCoroutines();
                CancelInvoke(nameof(HideNotification));
                HideNotification();
            }
        }

        public void ShowNotification(float duration = 3f)
        {
            StopAllCoroutines();
            CancelInvoke(nameof(HideNotification));
            StartCoroutine(Show(duration));
        }
        
        public virtual void ShowNotification(SnNotData notification)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            
            if (dismissable)
            {
                MenuManager.CanToggle = false;
                SnInput.InputManager.OnLockCursor += OnLockCursor;
            }
            
            else if (m_isShowing)
            {
                CancelInvoke(nameof(HideNotification));
            }
            
            titleText.text = notification.title;
            if (contentText) contentText.text = notification.content;
            
            // Init the icon
            if (canUseImages && iconImage)
            {
                iconImage.sprite = notification.icon;
                iconImage.gameObject.SetActive(notification.icon);
            }
            
            // Init the buttons
            if (canUseButtons)
            {
                // Spawn the buttons
                // Destroy the old buttons
                foreach (var button in m_buttons)
                {
                    Destroy(button.gameObject);
                }
                m_buttons.Clear();

                if (notification.buttons != null)
                {
                    foreach (var buttonData in notification.buttons)
                    {
                        var button = Instantiate(buttonPrefab, buttonsContainer.transform);
                        button.OnClick.AddListener(() =>
                        {
                            buttonData.onClick?.Invoke();
                            if (buttonData.closeOnClick)
                            {
                                // cancel the hide effects
                                StopAllCoroutines();
                                CancelInvoke(nameof(HideNotification));
                                HideNotification();
                            }
                        });
                        button.SetText(buttonData.text);
                        m_buttons.Add(button);
                    }
                }
            }
            
            ShowNotification(notification.duration);
        }

        IEnumerator Show(float duration)
        {
            m_isShowing = true;
            onShowEffects.ImmediateCancel(this);
            yield return onShowEffects.Apply(this);
            // Run the hide effects after the duration
            if (duration > 0)
                Invoke(nameof(HideNotification), duration);
        }
        
        private void HideNotification()
        {
            if (gameObject.activeSelf)
                StartCoroutine(Hide());

            if (dismissable)
            {
                InputManager.OnLockCursor -= OnLockCursor;
                MenuManager.CanToggle = true;
            }
        }
        
        public IEnumerator Hide()
        {
            yield return onHideEffects.Apply(this);
            m_isShowing = false;
            onHideEffects.ImmediateCancel(this);
            gameObject.SetActive(false);

            OnHide?.Invoke();
        }
        
        public void EditTitle(string title)
        {
            titleText.text = title;
        }
        
        public void EditContent(string content)
        {
            if (contentText) contentText.text = content;
        }
        
        public void EditNotification(SnNotData data)
        {
            if (titleText) titleText.text = data.title;
            if (contentText) contentText.text = data.content;
        }
        
        # if UNITY_EDITOR
        private void OnValidate()
        {
            if (container == null)
                container = GetComponent<Graphic>();
        }
        
        [ContextMenu("Toggle Show")]
        private void ToggleShow()
        {
            if (Application.isPlaying)
            {
                if (m_isShowing)
                    HideNotification();
                else
                    ShowNotification();
            } else
            {
                // Editor courotine
            }
        }
        # endif
    }
}