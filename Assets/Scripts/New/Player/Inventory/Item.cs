using System;
using UnityEngine;

namespace New.Player
{
    [Serializable]
    public abstract class Item
    {
        [SerializeField] private string id;
        [SerializeField] private string itemName;
        [SerializeField] private Sprite icon;
        [SerializeField] private float weight = 1f;
        [SerializeField] private bool isStackable = false;
        [SerializeField] private int stackCount = 1;
        [SerializeField] private bool consumeOnUse = false;
        [SerializeField] private string description;
        
        public virtual bool Use()
        {
            // Base implementation does nothing
            return false;
        }
        
        // Properties
        public string ID => id;
        public string Name => itemName;
        public Sprite Icon => icon;
        public float Weight => weight;
        public bool IsStackable => isStackable;
        public int StackCount { get; set; }
        public bool ConsumeOnUse => consumeOnUse;
        public string Description => description;
    }
}