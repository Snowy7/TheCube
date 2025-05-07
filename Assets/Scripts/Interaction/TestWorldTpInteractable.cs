using Game.WorldSystem;
using UnityEngine;

namespace Ineraction
{
    public class TestWorldTpInteractable : Interactable
    {
        [SerializeField] private WorldData worldData;
        
        public override void Interact(Interactor actor = null)
        {
            WorldsManager.Instance.LoadWorld(worldData, 5, true);
        }
    }
}