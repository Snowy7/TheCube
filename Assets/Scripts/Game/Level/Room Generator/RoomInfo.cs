using UnityEngine;
using RoomGenerator;
[CreateAssetMenu(fileName = "Room", menuName = "Create Room Info")]
public class RoomInfo : ScriptableObject
{
    public RoomType roomType;
    public GameObject roomPrefab;
    public int minAmount;
    public int maxAmount;
}
