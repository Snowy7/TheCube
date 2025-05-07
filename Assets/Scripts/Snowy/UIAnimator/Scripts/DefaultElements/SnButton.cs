using System;
using Audio;
using Snowy.UIAnimator.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Snowy.UI
{
    [Serializable]
    public class SnButtonEvent : UnityEvent
    {
    }

    public class SnButton : SnSelectable, IPointerClickHandler, ISubmitHandler
    {
        [Title("Button Settings")] [Space] [BeginGroup] [SerializeField]
        private bool playAudio = true;

        [SerializeField, ShowIf(nameof(playAudio), true)]
        private ButtonClips clips;

        [SerializeField] private TMP_Text buttonText;
        [EndGroup] [SerializeField] private SnButtonEvent onClick = new SnButtonEvent();

        public SnButtonEvent OnClick
        {
            get => onClick;
            set => onClick = value;
        }

        protected override void Awake()
        {
            base.Awake();

            base.OnHover += OnHoverAudio;
            base.OnClick += OnClickAudio;
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            onClick.Invoke();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;
        }

        private void OnHoverAudio()
        {
            if (playAudio)
            {
                AudioManager.Instance.PlayUIAudio(clips.buttonHover);
            }
        }

        private void OnClickAudio()
        {
            if (playAudio)
            {
                if (IsInteractable() && clips) AudioManager.Instance.PlayUIAudio(clips.buttonClick);
                else AudioManager.Instance.PlayUIAudio(clips.buttonDisabledClick);
            }
        }

        public void SetText(string text)
        {
            if (buttonText) buttonText.text = text;
        }

# if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!buttonText)
            {
                buttonText = GetComponentInChildren<TMP_Text>();
            }
        }
# endif
    }
}