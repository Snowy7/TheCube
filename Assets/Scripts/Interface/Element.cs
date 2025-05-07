using System;
using Actors.Player;
using UnityEngine;

namespace Interface
{
    public class Element : MonoBehaviour
    {
        protected FPSCharacter character;

        public virtual void Init(FPSCharacter chr)
        {
            character = chr;
        }

        public virtual void Tick(){}
    }
}