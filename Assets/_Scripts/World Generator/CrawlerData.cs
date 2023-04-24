using UnityEngine;

namespace Turn2D.MapGenerator
{
    public struct CrawlerData : GridData
    {
        private Vector2Int _gridPosition;
        public Vector2Int gridPosition => _gridPosition;
        public bool used;
        public Vector2Int parentPosition;
        public RoomType roomType;
        public CrawlerData(Vector2Int gridPosition)
        {
            _gridPosition = gridPosition;
            used = false;
            parentPosition = -Vector2Int.one;
            roomType = RoomType.normal;
        }
    }
    public enum RoomType
    {
        start, normal, path, end
    }
}