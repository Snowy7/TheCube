using UnityEngine;

namespace Plugins.Snowy.SnLevelDesigner
{
    [CreateAssetMenu(fileName = "TileSet", menuName = "Snowy/TileSet", order = 0)]
    public class SnTileSet : ScriptableObject
    {
        public SnTileComponent floorTile;
        public SnTileComponent wallTile;
        public SnTileComponent archTile;
        public SnTileComponent windowTile;
        public SnTileComponent pillarTitle;
    }
}