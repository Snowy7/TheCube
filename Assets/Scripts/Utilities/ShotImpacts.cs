using System;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public class Impact
    {
        [TagSelector] public string tag;
        [AssetPreview] public GameObject impact;
    }

    [CreateAssetMenu(fileName = "Shot Impacts", menuName = "Snowy/FPS/Shot Impacts")]
    public class ShotImpacts : ScriptableObject
    {
        [ReorderableList] public Impact[] impacts;
    }
}