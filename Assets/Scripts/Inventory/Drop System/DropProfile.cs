using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewDropProfile",menuName = "DropManager/NewDropProfile")]
public class DropProfile : ScriptableObject
{
    public List<DropItem> dropItems;
}

[Serializable]
public class DropItem
{
    public GameObject DropObj;
    public int DropPercentage;
}
