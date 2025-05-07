using UnityEngine;

namespace Actors
{
    public class BodyPart : MonoBehaviour, IDamageable
    {
        [SerializeField] private float damageMultiplier = 1f;
        public NetworkActor actor;
        
        private void Awake()
        {
            actor = GetComponentInParent<NetworkActor>();
            if (actor == null)
            {
                Debug.LogError("BodyPart must be a child of an Actor");
                Destroy(gameObject);
            }
        }

        public void SendDamage(float damage, uint id, DamageType damageType = DamageType.ByOther)
        {
            actor.SendDamage(damage * damageMultiplier, id, damageType);
        }

        public void TakeDamage(float damage, DamageType damageType = DamageType.ByOther)
        {
            actor.TakeDamage(damage * damageMultiplier, damageType);
        }

        public void TakeDamage(float damage, uint id, DamageType damageType = DamageType.ByOther)
        {
            actor.TakeDamage(damage * damageMultiplier, id, damageType);
        }

        public void Die()
        {
            // IGNORE
        }

        public void Heal(int amount)
        {
            // IGNORE
        }
    }
}