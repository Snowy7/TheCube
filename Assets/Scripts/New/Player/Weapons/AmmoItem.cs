using System;
using UnityEngine;

namespace New.Player
{
    [Serializable]
    public class AmmoItem : Item
    {
        [SerializeField] private string ammoType;
        
        public string AmmoType => ammoType;
    }
}