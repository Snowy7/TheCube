using System;
using UnityEngine;

namespace New.Player
{
    [Serializable]
    public class ConsumableItem : Item
    {
        [SerializeField] private float healthRestoreAmount;
        
        public override bool Use()
        {
            // Logic for using consumable would go here
            // E.g., find player and restore health
            return true;
        }
        
        public float HealthRestoreAmount => healthRestoreAmount;
    }
}