using Game.WorldSystem;
using UnityEngine;

[CreateAssetMenu(fileName ="New Mission",menuName ="Missions/MissionData")]
public class MissionData : ScriptableObject
{
    public string MissionName;
    public WorldData worldData;
    public string Description;
}
