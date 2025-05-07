using Game;
using Level;
using UnityEngine;

namespace Actors.Player
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] Rigidbody rb;
        [SerializeField] TrailRenderer trail;
        [SerializeField] Renderer renderer;
        [SerializeField] Collider collider;
        [SerializeField] float alertRadius = 10f;
        [SerializeField] Color color = Color.red;
        
        private uint m_actorId;
        private float m_damage;
        private float m_force;
        private bool m_canDamage;
        
        public void Init(uint netId, float damage, float impulseForce, float force, float lifeTime = 3f, bool canDamage = false)
        {
            m_actorId = netId;
            m_damage = damage;
            m_force = force;
            m_canDamage = canDamage;
            
            rb.isKinematic = false;
            rb.linearVelocity = transform.forward * impulseForce;
            
            Destroy(gameObject, lifeTime);
            
        }
        
        
        private void OnCollisionEnter(Collision other)
        {
            if (m_canDamage)
            {
                if (other.gameObject.TryGetComponent(out IDamageable damageable))
                {
                    // Debug.Log("Hit " + other.gameObject.name, other.gameObject);
                    
                    // if self ignore collision
                    if (damageable is BodyPart bodyPart && bodyPart.actor.netId == m_actorId)
                    {
                        return;
                    }
                    
                    damageable.SendDamage(m_damage, m_actorId, DamageType.ByOther);
                }
                
                // if hit rigidbody apply force
                if (other.rigidbody)
                {
                    other.rigidbody.AddForce(transform.forward * m_force, ForceMode.Impulse);
                }
                
                if (EnemyManager.Instance)
                {
                    EnemyManager.Instance.AlertNearbyEnemies(transform.position, alertRadius);
                }
            }
            
            // Destroy the sphere and the rigidbody
            rb.isKinematic = true;
            if (renderer) renderer.enabled = false;
            Destroy(collider);
            Destroy(gameObject, trail.time);         
            
            Vector3 position = other.GetContact(0).point;
            Vector3 normal = other.GetContact(0).normal;
            
            GameObject impactPrefab = Global.Instance.GetImpact(other.transform.gameObject);
            if (impactPrefab)
            {
                
                Instantiate(impactPrefab, position,
                    Quaternion.LookRotation(-transform.forward, normal),
                    other.transform);
            }
            
            m_canDamage = false;
        }
        
        # if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, alertRadius);
        }
        
        # endif
    }
}