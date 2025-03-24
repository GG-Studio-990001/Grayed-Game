using UnityEngine;

namespace Runtime.CH3.Main
{
    public class Tile
    {
        public Vector2Int Position { get; private set; }
        public Sprite TileSprite { get; private set; }
        
        public Tile(Vector2Int position, Sprite tileSprite)
        {
            Position = position;
            TileSprite = tileSprite;
        }
    }
}
