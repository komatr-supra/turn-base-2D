using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Turn2D.MapGenerator
{

    public class LevelGenerator : MonoBehaviour
    {
        #region Variables
        [Header("Size")]
        [Tooltip("Rooms count horizontal.")]
        [SerializeField] private int width = 4;
        [Tooltip("Rooms count vertical.")]
        [SerializeField] private int height = 4;
        [Space]
        [Header("Path lenght")]
        [Tooltip("How many rooms you must pass.")]
        [SerializeField] private int pathLenght = 10;        
        
        #endregion
        
        public void GenerateMap()
        {
            //rooms data
            CrawlerDungeonRoom crawlerDungeonRoom = new CrawlerDungeonRoom(width, height, pathLenght);
            var crawlerGridData = crawlerDungeonRoom.GetGridSystem;
            //populate rooms in grid tileset
            
        }
        
        
    }
}