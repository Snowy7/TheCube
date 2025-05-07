using System;
using Actors;
using Actors.Player;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class HealthDisplay : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private NetworkActor actor;
        [SerializeField, ShowIf(nameof(showHealth), true)] private Image healthBar;
        [SerializeField, ShowIf(nameof(showUsername), true)] private TMP_Text nameText;

        [Title("Settings")]
        [SerializeField] private Gradient colorGradient;
        [SerializeField] private float smoothSpeed = 0.125f;
        [SerializeField] private bool showHealth;
        [SerializeField] private bool showUsername;
        
        private float m_targetHealth;
        
        Client client;

        private void Start()
        {
            if (showUsername)
            {
                // if NetworkActor is player 
                if (actor is Player player)
                {
                    // if is local player then disable the name text
                    if (player.isOwned)
                    {
                        gameObject.SetActive(false);
                        return;
                    }
                    
                    ClientsManager.Instance.WaitForClient(player, c =>
                    {
                        client = c;
                        nameText.text = client.playerName;
                    });
                }
            }
            
            if (showHealth) actor.OnHealthChanged += OnHealthChanged;
            actor.OnActorDeath += OnDeath;
            OnHealthChanged(actor.health);
        }   
        
        private void OnDestroy()
        {
            if (showHealth) actor.OnHealthChanged -= OnHealthChanged;
            actor.OnActorDeath -= OnDeath;
        }

        private void LateUpdate()
        {
            if (showHealth)
            {
                var healthPercent = m_targetHealth / actor.maxHealth;
                healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, healthPercent, smoothSpeed);
                var color = colorGradient.Evaluate(healthBar.fillAmount);
                healthBar.color = color;
            }

            if (showHealth && client != null)
            {
                nameText.text = client.playerName;
            }
        }

        private void OnHealthChanged(float health)
        {
            m_targetHealth = health;
        }
        
        private void OnDeath(NetworkActor _)
        {
            Destroy(gameObject);
        }
    }
}