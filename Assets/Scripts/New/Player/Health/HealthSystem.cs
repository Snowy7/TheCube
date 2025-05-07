using UnityEngine;
using System;

namespace New.Player
{
    public class HealthSystem : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        [SerializeField] private bool canRegenerate = true;
        [SerializeField] private float regenerationRate = 5f; // Health per second
        [SerializeField] private float regenerationDelay = 5f; // Seconds after damage before regen starts
        
        [Header("Damage Modifiers")]
        [SerializeField] private DamageResistance[] damageResistances;
        
        private float lastDamageTime;
        private bool isDead = false;
        
        // Events
        public event Action<float, float> OnHealthChanged; // Current, Max
        public event Action<DamageInfo> OnDamageTaken;
        public event Action OnDeath;
        
        private void Awake()
        {
            currentHealth = maxHealth;
        }
        
        private void Update()
        {
            // Handle health regeneration
            if (canRegenerate && !isDead && currentHealth < maxHealth)
            {
                if (Time.time - lastDamageTime >= regenerationDelay)
                {
                    float regenerationAmount = regenerationRate * Time.deltaTime;
                    Heal(regenerationAmount);
                }
            }
        }
        
        public void TakeDamage(DamageInfo damageInfo)
        {
            if (isDead) return;
            
            // Apply damage resistance
            float modifiedDamage = ApplyResistance(damageInfo);
            
            // Apply damage
            currentHealth = Mathf.Max(0, currentHealth - modifiedDamage);
            lastDamageTime = Time.time;
            
            // Trigger event
            OnDamageTaken?.Invoke(damageInfo);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            // Check for death
            if (currentHealth <= 0 && !isDead)
            {
                isDead = true;
                OnDeath?.Invoke();
            }
            
            // Visual feedback handled by subscribers to events
        }
        
        public void Heal(float amount)
        {
            if (isDead) return;
            
            float previousHealth = currentHealth;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            
            // Only trigger event if health actually changed
            if (previousHealth != currentHealth)
            {
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
        }
        
        public void ResetHealth()
        {
            isDead = false;
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        private float ApplyResistance(DamageInfo damageInfo)
        {
            float damageMultiplier = 1f;
            
            foreach (var resistance in damageResistances)
            {
                if (resistance.damageType == damageInfo.Type)
                {
                    damageMultiplier = 1f - resistance.resistancePercentage;
                    break;
                }
            }
            
            return damageInfo.Amount * damageMultiplier;
        }
        
        // Properties
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float HealthPercentage => currentHealth / maxHealth;
        public bool IsDead => isDead;
    }
    
    [System.Serializable]
    public class DamageResistance
    {
        public DamageType damageType;
        [Range(0f, 1f)] public float resistancePercentage;
    }
    
    public struct DamageInfo
    {
        public float Amount;
        public DamageType Type;
        public Vector3 HitPoint;
        public Vector3 HitDirection;
        public GameObject DamageSource;
        
        public DamageInfo(float amount, DamageType type, Vector3 hitPoint, Vector3 hitDirection, GameObject source = null)
        {
            Amount = amount;
            Type = type;
            HitPoint = hitPoint;
            HitDirection = hitDirection;
            DamageSource = source;
        }
    }
}
