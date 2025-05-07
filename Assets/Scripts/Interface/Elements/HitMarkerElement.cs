using System;
using System.Collections.Generic;
using Actors;
using Actors.Player;
using Networking;
using UnityEngine;

namespace Interface.Elements
{
    public class HitMarkerElement : Element
    {
        [SerializeField] private HitMark hitMarkerPrefab;
        
        Dictionary<uint, GameObject> m_hitMarkers = new Dictionary<uint, GameObject>();
        Player player;
        
        private float m_lastHitTime;
        
        public override void Init(FPSCharacter chr)
        {
            base.Init(chr);
            
            // Get local player
            player = ClientsManager.Instance.LocalClient?.Player;
            if (player != null) player.OnSelfDamage += OnSelfDamage;
        }

        private void OnDestroy()
        {
            if (player != null) player.OnSelfDamage -= OnSelfDamage;
        }

        private void OnDisable()
        {
            foreach (var hitMarker in m_hitMarkers)
            {
                Destroy(hitMarker.Value);
            }
            m_hitMarkers.Clear();
        }

        private void OnSelfDamage(uint id, DamageType arg2)
        {
            // Show hit marker
            GameObject attacker = player.GetLastAttacker();
            if (attacker != null)
            {
                if (m_hitMarkers.ContainsKey(id))
                {
                    Destroy(m_hitMarkers[id]);
                    m_hitMarkers.Remove(id);
                }
                
                // Instantiate hit marker
                var hitMarker = Instantiate(hitMarkerPrefab, transform);
                hitMarker.Init(attacker, character.gameObject);
                
                m_hitMarkers.Add(id, hitMarker.gameObject);
            }
        }
    }
}