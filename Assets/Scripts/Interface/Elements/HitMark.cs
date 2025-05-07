using UnityEngine;

namespace Interface.Elements
{
    public class HitMark : MonoBehaviour
    {
        private static readonly int Hit = Animator.StringToHash("Hit");
        [SerializeField] private float hitMarkDuration = 3.3f;
        [SerializeField] private GameObject visuals;
        [SerializeField] private Animator animator;
        [SerializeField, Disable] private GameObject target;
        [SerializeField, Disable] private GameObject character;
        
        private float m_lastHitTime;

        public void Init(GameObject source, GameObject chr)
        {
            this.target = source;
            this.character = chr;
            
            UpdateRot();
            visuals.SetActive(true);
            animator.SetTrigger(Hit);
            
            Invoke(nameof(Die), hitMarkDuration);
        }
        
        void Die()
        {
            Destroy(gameObject);
        }

        void Update()
        {
            if (target == null || character == null) return;
            
            UpdateRot();
        }
        
        void UpdateRot()
        {
            // Rotate hit marker towards attacker from the player movement direction
            Vector3 direction = character.transform.position - target.transform.position;
            var rot = Quaternion.LookRotation(direction);
            rot.z = -rot.y;
            rot.x = 0;
            rot.y = 0;
            
            Vector3 north = new Vector3(0, 0, character.transform.eulerAngles.y);
            
            transform.localRotation = rot * Quaternion.Euler(north);
        }
    }
}