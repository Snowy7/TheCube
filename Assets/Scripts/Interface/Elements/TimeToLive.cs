using Actors.Player;
using Firebase.Game;
using TMPro;
using UnityEngine;

namespace Interface.Elements
{
    public class TimeToLive : Element
    {
        [Title("References")]
        [SerializeField] private TMP_Text text;
        
        public override void Init(FPSCharacter chr)
        {
            base.Init(chr);
            
            MissionManager.Instance.OnMissionStart.AddListener(OnMissionStart);
            MissionManager.Instance.OnMissionEnd.AddListener(OnMissionEnd);
            
            gameObject.SetActive(!MissionManager.Instance.isMissionActive);
        }
        
        private void OnDestroy()
        {
            if (MissionManager.Instance == null) return;
            
            MissionManager.Instance.OnMissionStart.RemoveListener(OnMissionStart);
            MissionManager.Instance.OnMissionEnd.RemoveListener(OnMissionEnd);
        }
        
        private void OnMissionStart()
        {
            gameObject.SetActive(false);
        }
        
        private void OnMissionEnd()
        {
            gameObject.SetActive(true);
        }

        public override void Tick()
        {
            text.text = $"{UserController.GetTimeToLive()}";
        }
    }
}