using UnityEngine;
using UnityEngine.Events;

namespace Ineraction
{
    public class TestInteractable : Interactable
    {
        [SerializeField] private UnityEvent onInteract;
        
        public override void Interact(Interactor actor = null)
        {
            onInteract.Invoke();
        }
    }
}