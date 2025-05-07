using System;
using SnInput;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Utilities
{
    public class InputImage : MonoBehaviour
    {
        [SerializeField] private Image inputImage;
        [SerializeField] private Sprite keyboardImage;
        [SerializeField] private Sprite gamepadImage;

        private void OnEnable()
        {
            if (InputManager.Instance)
            {
                InputManagerBase.onControlsChanged += OnControlsChanged;
                
                // set the initial image
                OnControlsChanged(InputManager.Instance.PlayerInput);
            }
        }

        private void OnDisable()
        {
            if (InputManager.Instance)
            {
                InputManagerBase.onControlsChanged -= OnControlsChanged;
            }
        }
        
        private void OnControlsChanged(PlayerInput input)
        {
            if (input.currentControlScheme == "KeyboardMouse")
            {
                inputImage.sprite = keyboardImage;
            }
            else if (input.currentControlScheme == "Gamepad")
            {
                inputImage.sprite = gamepadImage;
            }
            
            Debug.Log($"Current control scheme: {input.currentControlScheme}");
        }
    }
}