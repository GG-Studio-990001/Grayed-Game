using System;
using UnityEngine;

namespace Runtime.CH3.Main
{
    public class TilemapManager: MonoBehaviour
    {
        private Tile[,] _tiles;

        private void Start()
        {
            Initialize(10, 10);
            SetTile(0, 0, null);
            SetTile(1, 1, null);
            SetTile(2, 2, null);
            SetTile(3, 3, null);
            SetTile(4, 4, null);
            SetTile(5, 5, null);
            SetTile(6, 6, null);
            SetTile(7, 7, null);
            SetTile(8, 8, null);
            SetTile(9, 9, null);
        }

        public void Initialize(int width, int height)
        {
            _tiles = new Tile[width, height];
        }
        
        public void SetTile(int x, int y, Sprite sprite)
        {
            if (_tiles[x, y] == null)
            {
                _tiles[x, y] = new Tile(new Vector2Int(x, y), sprite);
                CreateTileGameObject(x, y, sprite);
            }
        }

        private void CreateTileGameObject(int x, int y, Sprite sprite)
        {
            GameObject tileObject = new GameObject($"Tile_{x}_{y}");
            tileObject.transform.position = new Vector3(x, y * 0.5f, y);
            SpriteRenderer spr = tileObject.AddComponent<SpriteRenderer>();
            spr.sprite = sprite;
        }
        
        public Tile GetTile(int x, int y)
        {
            return _tiles[x, y];
        }
    }
}