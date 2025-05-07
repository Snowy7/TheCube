using Game;
using Interface;
using Mirror;
using SnInput;
using UnityEngine;
using Utils;

namespace Actors.Player
{
    public class Player : NetworkActor
    {
        [Header("References")]
        [SerializeField] FPSCharacter character;
        [SerializeField] RagdollController ragdollController;
        [SerializeField] private GameObject tpsColliderRoot;
        [SerializeField] private GameObject tpsCharacter;
        [SerializeField] private GameObject fpsCharacter;
        [SerializeField] private Component[] fpsComponents;
        [SerializeField] private GameObject fpsHead;
        [SerializeField] private GameObject deathCamHead;

        [Header("On Death Settings")]
        [SerializeField] private float deathDuration = 5f;
        [SerializeField] private Component[] onDeathComponents;
        [SerializeField] private GameObject[] onDeathGameObjects;
        
        [Header("AI Aim Assist Settings")]
        [SerializeField] private float normalHeadHeight = 1.7f;
        [SerializeField] private float crouchingHeadHeight = 1.2f;
        
        public FPSCharacter Character => character;

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if (!isOwned)
            {
                foreach (var component in fpsComponents)
                {
                    Destroy(component);
                }
            }
            
            tpsCharacter.SetActive(!isOwned);
            // tpsColliderRoot.SetActive(!isOwned);
            fpsCharacter.SetActive(isOwned);

            if (isOwned)
            {
                OnLockCursor(true);
                
                MenuManager.Instance.CanToggleMenu = true;
                InputManager.Instance.SwitchToGameControls();
            }
        }

        public override void OnTakeDamage(float damage, DamageType damageType)
        {
            base.OnTakeDamage(damage, damageType);
        }

        public override void OnHeal(float amount)
        {
            base.OnHeal(amount);
        }

        public override void OnDeath()
        {
            base.OnDeath();
            
            // The camera needs to be moved to the back, behind the player and the third person character needs to be enabled.
            tpsCharacter.SetActive(true);
            ragdollController.EnableRagdoll();
            
            try 
            {
                // Move the death camera to the head position
                deathCamHead.transform.position = fpsHead.transform.position;
                deathCamHead.transform.rotation = fpsHead.transform.rotation;
            } catch (System.Exception e)
            {
                // ignored
            }
            
            // Disable the FPS character
            fpsCharacter.SetActive(false);
            
            // Destroy all network transforms & network behaviours
            foreach (var nt in GetComponentsInChildren<NetworkBehaviour>())
            {
                if (nt == this) continue;
                Destroy(nt);
            }
            
            // Destroy all components that are not needed anymore
            foreach (var component in onDeathComponents)
            {
                Destroy(component);
            }
            
            // Destroy all game objects that are not needed anymore
            foreach (var go in onDeathGameObjects)
            {
                Destroy(go);
            }
            
            // Spawn the spectate camera
            if (isOwned)
            {
                GameManager.Instance.SpawnSpectateCamera();
            }
            
            if (isServer)
            {
                Invoke(nameof(OnDeathComplete), deathDuration);
            }
        }
        
        private void OnDeathComplete()
        {
            if (isServer)
            {
                NetworkServer.Destroy(gameObject);
            }
        }

        public override void OnHealthUpdate()
        {
            base.OnHealthUpdate();
        }
        
        public Vector3 GetHeadPosition()
        {
            Debug.Assert(character != null, "Character reference is null.");
            if (character == null) return Vector3.zero;
            return character.IsCrouching() ?
                character.transform.position + Vector3.up * crouchingHeadHeight
                : character.transform.position + Vector3.up * normalHeadHeight;
        }
        
        public bool Ping()
        {
            // Check if reference to this object is still valid.
            return true;
        }
        
        public GameObject GetPlayerDeathHead()
        {
            return IsDead || isOwned ? deathCamHead : fpsHead;
        }

        public void OnLockCursor(bool state)
        {
            Debug.Log($"Cursor lock state: {state}, Character: {Character}");
            if (Character) Character.OnLockCursor(state);
            //Update cursor visibility.
            Cursor.visible = !state;
            //Update cursor lock state.
            Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        # if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (character == null || IsDead) return;
            // Draw a sphere at the head position
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(GetHeadPosition(), 0.2f);
        }
        # endif
        
        public void SetFreeze(bool state)
        {
            if (character) character.SetFreeze(state);
        }
    }
}