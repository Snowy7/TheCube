using System;
using Mirror;
using UnityEngine;

namespace Actors
{
    /// <summary>
    /// a damageable actor that can be controlled by the network
    /// </summary>
    public class NetworkActor : NetworkBehaviour, IDamageable 
    {
        public event System.Action<float> OnHealthChanged;
        public event Action<uint, DamageType> OnSelfDamage; 
        public event System.Action<NetworkActor> OnActorDeath;
        public event System.Action OnActorDestroy;
        [SerializeField] bool startWithMaxHealth = true;
        [SyncVar(hook = nameof(OnHealthChange))] public float health = 100;
        [SyncVar] public float maxHealth = 100;
        
        public bool IsDead => health <= 0;
        
        public Vector3 position => transform.position;
        public Quaternion rotation => transform.rotation;
        public Transform root => transform;
        
        protected bool m_calledDeath = false; 
        
        protected GameObject m_lastAttacker;
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            health = startWithMaxHealth ? maxHealth : health;
        }

        private void OnDestroy()
        {
            OnActorDestroy?.Invoke();
        }

        public void OnHealthChange(float oldHealth, float newHealth)
        {
            // Call the OnTakeDamage method when the health lower or higher than the old health
            if (newHealth < oldHealth)
            {
                OnTakeDamage(oldHealth - newHealth, DamageType.ByOther);
            }
            else
            {
                OnHeal(newHealth - oldHealth);
            }

            OnHealthUpdate();
            
            OnHealthChanged?.Invoke(newHealth);
        }
        
        public void SendDamage(float damage, uint id, DamageType damageType)
        {
            if (isServer)
                TakeDamage(damage, id, damageType);
            else
                CmdTakeDamage(damage, id, damageType);
        }
        
        [Server]
        public void TakeDamage(float damage, DamageType damageType)
        {
            if (!isServer)
                return;
            
            health -= damage;
            if (health <= 0 && !m_calledDeath)
            {
                Die();
                m_calledDeath = true;
            }
        }
        
        [Server]
        public void TakeDamage(float damage, uint id, DamageType damageType)
        {
            TakeDamage(damage, damageType);
            
            // Get the last attacker Using netId
            m_lastAttacker = NetworkServer.spawned[id].gameObject;
            
            // send to the client that the actor has taken damage
            if (connectionToClient != null)
                TargetTakeDamage(connectionToClient, id, damageType);
            
            // TODO: implement this method
        }

        [TargetRpc]
        public void TargetTakeDamage(NetworkConnection conn, uint id, DamageType damageType)
        {
            m_lastAttacker = NetworkClient.spawned[id].gameObject;
            OnSelfDamage?.Invoke(id, damageType);
        }

        [Server]
        public void Heal(int amount)
        {
            health += amount;
            if (health > maxHealth)
                health = maxHealth;
        }
        
        [Command(requiresAuthority = false)]
        private void CmdTakeDamage(float damage, uint id, DamageType damageType)
        {
            TakeDamage(damage, id, damageType);
        }

        [Server]
        public void Die()
        {
            RpcDie();
        }
        
        [ClientRpc]
        public void RpcDie()
        {
            OnDeath();
        }
        
        # region callbacks to override
        
        public virtual void OnTakeDamage(float damage, DamageType damageType)
        {
            // override this method to add custom behavior when taking damage
        }
        
        public virtual void OnHeal(float amount)
        {
            // override this method to add custom behavior when healing
        }
        
        public virtual void OnDeath()
        {
            // override this method to add custom behavior when dying
            OnActorDeath?.Invoke(this);
        }
        
        public virtual void OnHealthUpdate()
        {
            // override this method to add custom behavior when health changes
        }
        
        # endregion
        
        public GameObject GetLastAttacker()
        {
            return m_lastAttacker;
        }
    }
}