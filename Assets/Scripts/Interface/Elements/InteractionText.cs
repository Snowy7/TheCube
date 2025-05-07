using Actors.Player;
using Ineraction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Elements
{
    public class InteractionText : Element
    {
        #region FIELDS SERIALIZED

        [Title(label: "References")]
        [Tooltip("Text that gets modified when looking at something to pick up.")]
        [SerializeField]
        private TextMeshProUGUI interactionText;
        [SerializeField] private GameObject holdVisual;
        [SerializeField] private Image holdFill;

        [Title(label: "Setup")] [Tooltip("Name of the boolean to set when changing state.")] [SerializeField]
        private string stateName = "Visible";

        #endregion

        #region FIELDS

        /// <summary>
        /// Animator.
        /// </summary>
        private Animator animator;

        /// <summary>
        /// Interactor Behaviour.
        /// </summary>
        private Interactor interactorBehaviour;

        #endregion

        #region UNITY

        /// <summary>
        /// Awake.
        /// </summary>
        public override void Init(FPSCharacter chr)
        {
            //Base.
            base.Init(chr);

            //Cache Animator.
            animator = GetComponent<Animator>();

            //Cache interactor.
            interactorBehaviour = character.GetInteractor();
        }

        private void OnEnable()
        {
            if (holdFill) holdFill.fillAmount = 0;
        }

        #endregion

        public override void Tick()
        {
            base.Tick();

            var interactor = character.GetInteractor();
            if (interactor == null)
            {
                animator.SetBool(stateName, false);
                return;
            }

            //Cache Interactor Behaviour.
            if (interactorBehaviour == null)
                interactorBehaviour = character.GetInteractor();

            //Check if we can interact.
            if (interactorBehaviour != null && interactorBehaviour.CanInteract())
            {
                //Get Interactable.
                Interactable interactable = interactorBehaviour.GetInteractable();
                if (interactable != null)
                {
                    //Show.
                    animator.SetBool(stateName, true);
                    
                    holdVisual.SetActive(interactable.IsHoldInteraction());

                    //Modify Text.
                    if (interactionText != null)
                        interactionText.text = interactable.IsHoldInteraction()
                            ? "(Hold) " + interactable.GetInteractionText()
                            : interactable.GetInteractionText();

                    if (interactable.IsHoldInteraction())
                    {
                        holdFill.fillAmount = interactorBehaviour.GetHoldProgress();
                    }
                    else
                    {
                        holdFill.fillAmount = 0;
                    }
                }

                //Hide.
                else
                    animator.SetBool(stateName, false);
            }
            else
            {
                animator.SetBool(stateName, false);
            }
        }
    }
}