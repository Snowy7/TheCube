using Game.WorldSystem;
using UnityEngine;

namespace Ineraction
{
    public class TestMissionInteractable : Interactable
    {
        [SerializeField] private MissionData missionData;
        
        public override void Interact(Interactor actor = null)
        {
            MissionManager.Instance.MissionStart(missionData);
            
            isInteractable = false;
        }
    }
}