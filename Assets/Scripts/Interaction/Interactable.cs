using Mirror;
using UnityEngine;

namespace Ineraction
{
    /// <summary>
    /// Interactable.
    /// </summary>
    public abstract class Interactable : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Title("Network Settings")]
        [SerializeField,
        Tooltip("If you want the action to only be performed by the host")]
        protected bool isHostOnly = false;
        
        [Title("Interaction Settings")]
        [SerializeField]
        protected bool isInteractable = true;
        [SerializeField,Tooltip("If you want to use hold interaction use this boolean")]
        protected bool isHoldInteractable = false;
        [SerializeField, EnableIf(nameof(isHoldInteractable), true)]
        protected float holdTime = 1.0f;
        [SerializeField, Tooltip("Start with 'to' ex: 'To Open'")]
        protected string interactionText;
        #endregion
        
        #region UNITY

        /// <summary>
        /// Awake.
        /// </summary>
        protected virtual void Awake(){}

        /// <summary>
        /// Start.
        /// </summary>
        protected virtual void Start(){}

        /// <summary>
        /// Update.
        /// </summary>
        protected virtual void Update(){}

        /// <summary>
        /// Fixed Update.
        /// </summary>
        protected virtual void FixedUpdate(){}

        /// <summary>
        /// Late Update.
        /// </summary>
        protected virtual void LateUpdate(){}

        #endregion
        
        #region METHODS
        
        /// <summary>
        /// Called to interact with this object.
        /// </summary>
        /// <param name="actor">The actor starting the interaction.</param>
        public abstract void Interact(Interactor actor = null);
        
        public void SetInteractable(bool value)
        {
            isInteractable = value;
        }
        
        #endregion

        #region GETTERS

        //TODO
        public virtual string GetInteractionText() => interactionText;
        public virtual bool IsHoldInteraction()
        {
            return isHoldInteractable && isInteractable && enabled && (!isHostOnly || NetworkServer.active);
        }
        public virtual bool CanInteract()
        {
            return isInteractable && enabled && (!isHostOnly || NetworkServer.active);
        }
        
        public virtual float GetHoldTime()
        {
            return holdTime;
        }

        #endregion
    }
}