using System.Collections;
using Game;
using Unity.Cinemachine;
using UnityEngine;

namespace Actors.Player
{
    public class SpectatePlayer : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private CinemachineThirdPersonFollow zoomCamera;
        
        [Title("Death Cam")]
        [SerializeField] private float deathCamDuration = 3f;
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private AnimationCurve positionCurve;
        
        [Title("Debug")]
        [SerializeField, Disable] Player target;

        public void SetTarget(Player player)
        {
            if (target != null)
            {
                target.OnActorDestroy -= OnTargetDestroy;
            }
            
            if (player == null)
            {
                target = null;
                return;
            }
            
            Debug.Log("Set target: " + player);
            target = player;
            
            // position the camera to the target head
            virtualCamera.Follow = target.GetPlayerDeathHead().transform;
            virtualCamera.LookAt = target.GetPlayerDeathHead().transform;
            
            zoomCamera.ShoulderOffset = startPosition;
            
            // if is dead then 3rd person camera
            if (target.IsDead || target.health <= 0 || target.isOwned)
            {
                Debug.Log("Target is dead");
                OnTargetDeath(target);
            }
            
            // if is alive then 1st person camera
            else
            {
                Debug.Log("Target is alive");
                target.OnActorDeath += OnTargetDeath;
            }
            
            // On Destroy -> Switch to the next player
            target.OnActorDestroy += OnTargetDestroy;
        }

        private void OnTargetDeath(NetworkActor actor)
        {
            virtualCamera.Follow = target.GetPlayerDeathHead().transform;
            virtualCamera.LookAt = target.GetPlayerDeathHead().transform;
            
            // Switch to death cam animation
            StartCoroutine(StartDeathCam());
        }
        
        private void OnTargetDestroy()
        {
            target.OnActorDestroy -= OnTargetDestroy;
            
            if (GameManager.Instance == null) return;
            
            // Switch to the next player
            Player[] players = GameManager.Instance.GetAlivePlayers();
            
            if (players.Length == 0)
            {
                // No players left
                Debug.Log("No players left");
                SetTarget(null);
                return;
            }
            
            // Stop all coroutines
            StopAllCoroutines();
            
            // Get the next player
            Debug.Log("Switch to the next player");
            Player nextPlayer = players[0];
            SetTarget(nextPlayer);
        }
        
        IEnumerator StartDeathCam()
        {
            // use zoomCamera.FollowOffset
            float duration = deathCamDuration;
            float time = 0;
            
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                
                zoomCamera.ShoulderOffset = Vector3.Lerp(startPosition, positionOffset, positionCurve.Evaluate(t));
                
                yield return null;
            }
        }
    }
}