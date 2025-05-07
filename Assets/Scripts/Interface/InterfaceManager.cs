using System;
using Actors.Player;
using Snowy.Utils;
using UnityEngine;

namespace Interface
{
    public class InterfaceManager : MonoSingleton<InterfaceManager>
    {
        [Header("References")]
        [SerializeField] Canvas canvas;
        [SerializeField, ReorderableList] Element[] elements;
        
        [Header("Setup")]
        [SerializeField] float distance = 5f;
        
        private bool initialized;
        
        public void Init(FPSCharacter character, Camera uiCamera = null)
        {
            foreach (var element in elements)
            {
                element.Init(character);
            }


            if (canvas && uiCamera)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = uiCamera;
                canvas.planeDistance = distance;
            }
            
            initialized = true;
        }

        private void Update()
        {
            if (!initialized) return;
            
            foreach (var element in elements)
            {
                if (element.enabled) element.Tick();
            }
        }
    }
}