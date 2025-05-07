using Mirror;
using UnityEngine;

namespace Utils
{
    public class RagdollController : MonoBehaviour
    {
        [SerializeField] private Animator Animator;
        [SerializeField] private Transform RagdollRoot;
        [SerializeField] private bool StartRagdoll = false;
        private Rigidbody[] Rigidbodies;
        private CharacterJoint[] Joints;

        private void Awake()
        {
            Rigidbodies = RagdollRoot.GetComponentsInChildren<Rigidbody>();
            Joints = RagdollRoot.GetComponentsInChildren<CharacterJoint>();

            if (StartRagdoll)
            {
                EnableRagdoll();
            }
            else
            {
                EnableAnimator();
            }
        }

        public void EnableRagdoll()
        {
            if (Rigidbodies == null || Joints == null)
            {
                Rigidbodies = RagdollRoot.GetComponentsInChildren<Rigidbody>();
                Joints = RagdollRoot.GetComponentsInChildren<CharacterJoint>();
            }

            Animator.enabled = false;

            foreach (CharacterJoint joint in Joints)
            {
                joint.enableCollision = true;
            }
            
            foreach (Rigidbody rigidbody in Rigidbodies)
            {
                rigidbody.detectCollisions = true;
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                rigidbody.linearVelocity = Vector3.zero;
            }
        }

        public void DisableAllRigidbodies()
        {
            foreach (Rigidbody rigidbody in Rigidbodies)
            {
                rigidbody.detectCollisions = false;
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
            }
        }

        public void EnableAnimator()
        {
            if (Rigidbodies == null || Joints == null)
            {
                Rigidbodies = RagdollRoot.GetComponentsInChildren<Rigidbody>();
                Joints = RagdollRoot.GetComponentsInChildren<CharacterJoint>();
            }

            Animator.enabled = true;
            foreach (CharacterJoint joint in Joints)
            {
                joint.enableCollision = false;
            }

            foreach (Rigidbody rigidbody in Rigidbodies)
            {
                //rigidbody.detectCollisions = false;
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
            }
        }
    }
}