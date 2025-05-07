using Actors;
using Mirror;
using UnityEngine;

namespace Actors.AI
{
    public abstract class BaseAttack : NetworkBehaviour
    {
        [SerializeField] protected Enemy self;
        [SerializeField] protected Animator animator;
        [SerializeField] protected float attackRate = 1f;
        private float m_attackLastTime;
        
        protected abstract void OnAttack(int seed = 0);

        [Server]
        public void Attack(NetworkActor actor)
        {
            if (!isServer) return;
            
            if (Time.time - m_attackLastTime > attackRate)
            {
                m_attackLastTime = Time.time;
                // generate a random seed for the attack
                int seed = Random.Range(0, 1000);
                RpcAttack(seed);
            }
        }
        
        [ClientRpc]
        private void RpcAttack(int seed)
        {
            if (!isClient) return;
            OnAttack(seed);
        }
    }
}