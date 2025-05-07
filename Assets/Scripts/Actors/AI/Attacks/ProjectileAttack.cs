using Actors.Player;
using UnityEngine;

namespace Actors.AI.Attacks
{
    public class ProjectileAttack : BaseAttack
    {
        [SerializeField] Transform bulletSpawnPoint;
        [SerializeField] Bullet bulletPrefab;
        
        [SerializeField] float damage = 10f;
        [SerializeField] float impulseForce = 10f;
        [SerializeField] float spreadAmount = 0.1f;
        [SerializeField] float force = 10f;
        [SerializeField] float lifeTime = 3f;
        
        protected override void OnAttack(int seed = 0)
        {
            Random.InitState(seed);
            
            // spread
            Vector3 spreadV3 = Random.insideUnitSphere * spreadAmount;
            
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            // apply spread at a random axis
            bullet.transform.Rotate(spreadV3);
            bullet.Init(netId, damage, impulseForce, force, lifeTime, isServer);
            
            animator.Play("Fire");
        }
    }
}