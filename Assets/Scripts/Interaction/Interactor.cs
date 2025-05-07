using System.Collections;
using Actors.Player;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using SnInput;

namespace Ineraction
{
    public class Interactor : NetworkBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("References")] [SerializeField]
        private Player player;

        [Tooltip("Used to determine where to trace the interaction from, and what direction it should go in.")]
        [SerializeField]
        private Transform interactor;

        [Header("Settings")] [Tooltip("Mask used to trace for interactions.")] [SerializeField]
        private LayerMask mask;

        [Tooltip("Radius of the trace.")] [SerializeField]
        private float radius = 1.0f;

        [Tooltip("Maximum interaction distance.")] [SerializeField]
        private float maxDistance = 5.0f;

        private float m_firstPressTime;

        #endregion

        #region FIELDS

        /// <summary>
        /// Main Hit Result.
        /// </summary>
        private RaycastHit hitResult;

        /// <summary>
        /// Interactable.
        /// </summary>
        private Interactable interactable;

        #endregion

        #region UNITY

        private void Start()
        {
            //Make sure we have a reference to the interactor.
            if (interactor == null)
                interactor = transform;

            InputManager.OnInteract += TryInteract;
        }

        private void OnDestroy()
        {
            InputManager.OnInteract -= TryInteract;
        }

        /// <summary>
        /// Update.
        /// </summary>
        protected void Update()
        {
            //Interaction Trace.
            if (Physics.SphereCast(interactor.position, radius,
                    interactor.forward, out hitResult, maxDistance, mask, QueryTriggerInteraction.Collide))
            {
                //If we hit a collider.
                if (hitResult.collider != null)
                {
                    //Try to get the interactable.
                    interactable = hitResult.collider.GetComponent<Interactable>();
                }
                else
                    interactable = null;
            }
            else
                interactable = null;
        }

        #endregion

        #region INPUT

        /// <summary>
        /// Interact.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void TryInteract(InputAction.CallbackContext context)
        {
            //Switch.
            switch (context)
            {
                //Performed.
                case { phase: InputActionPhase.Started } when CanInteract():
                case { phase: InputActionPhase.Performed } when CanInteract():
                    // if is hold
                    if (interactable.IsHoldInteraction())
                    {
                        // stop other couroutine:
                        StopAllCoroutines();
                        StartCoroutine(InteractHold());
                        return;
                    }

                    m_firstPressTime = 0;
                    //Try Interact.
                    interactable.Interact(this);
                    break;

                case { phase: InputActionPhase.Canceled }:
                    StopAllCoroutines();
                    m_firstPressTime = 0;
                    break;
            }
        }

        IEnumerator InteractHold()
        {
            m_firstPressTime = Time.time;
            
            while (interactable != null && Time.time - m_firstPressTime < interactable.GetHoldTime())
            {
                if (interactable.CanInteract() == false)
                    yield break;

                yield return null;
            }

            if (interactable != null && interactable.CanInteract())
            {
                interactable.Interact(this);
            }

            m_firstPressTime = 0;
        }

        #endregion

        #region GETTERS

        /// <summary>
        /// Can Interact
        /// </summary>
        public bool CanInteract()
        {
            //TODO: Add this.
            //Block while the cursor is unlocked.
            // if (!cursorLocked)
            //  return;

            //Return.
            return interactable != null && interactable.CanInteract();
        }

        /// <summary>
        /// Get Hit Result.
        /// </summary>
        /// <returns></returns>
        public RaycastHit GetHitResult() => hitResult;

        /// <summary>
        /// Get Interactable.
        /// </summary>
        public Interactable GetInteractable() => interactable;

        public float GetHoldProgress()
        {
            return m_firstPressTime > 0
                ? Mathf.Clamp01((Time.time - m_firstPressTime) / interactable.GetHoldTime())
                : 0;
        }

        #endregion

        #region Editor

# if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            //Trace Ray.
            var ray = new Ray(interactor.position, interactor.forward);
            //Draw Ray.
            Gizmos.color = interactable != null ? Color.yellow : Color.green;
            Gizmos.DrawRay(ray.origin, ray.direction * (maxDistance - radius));
            Gizmos.DrawWireSphere(ray.origin + ray.direction * maxDistance, radius);
        }

# endif

        #endregion
    }
}