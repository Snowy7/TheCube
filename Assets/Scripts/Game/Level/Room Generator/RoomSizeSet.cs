using System;
using UnityEngine;

namespace RoomGenerator
{
    [Serializable] public struct RoomSizeSet
    {
        [ReorderableList] public RoomInfo[] rooms;
        public Transform spawnPosition;
    }
}