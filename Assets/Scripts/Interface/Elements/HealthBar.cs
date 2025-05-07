using System;
using Actors.Player;
using FPS;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Elements
{
    /// <summary>
    /// Represents a health bar element.
    /// </summary>
    public class HealthBar : Element
    {
        /// <summary>
        /// Color gradient for the health bar from low to high health.
        /// </summary>
        [SerializeField] private Gradient colorGradient;
        [SerializeField] private Image healthBar;
        [SerializeField] private float smoothSpeed = 0.125f;
        
        private Player player;

        public override void Init(FPSCharacter chr)
        {
            base.Init(chr);
            
            player = chr.GetComponent<Player>();
            
            if (!player)
            {
                Debug.LogError("Player component not found on FPSCharacter.");
            }

            MissionManager.Instance.OnMissionStart.AddListener(OnMissionStart);
            MissionManager.Instance.OnMissionEnd.AddListener(OnMissionEnd);
            
            gameObject.SetActive(MissionManager.Instance.isMissionActive);
        }

        private void OnDestroy()
        {
            if (MissionManager.Instance == null) return;

            MissionManager.Instance.OnMissionStart.RemoveListener(OnMissionStart);
            MissionManager.Instance.OnMissionEnd.RemoveListener(OnMissionEnd);
        }

        private void OnMissionStart()
        {
            gameObject.SetActive(true);
        }
        
        private void OnMissionEnd()
        {
            gameObject.SetActive(false);
        }
        
        public override void Tick()
        {
            UpdateHealthBar(player.health);
        }

        private void UpdateHealthBar(float health)
        {
            var healthPercent = health / player.maxHealth;
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, healthPercent, smoothSpeed);
            var color = colorGradient.Evaluate(healthBar.fillAmount);
            healthBar.color = color;
        }
    }
}