using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Turn2D.MapGenerator
{
    public class CrawlerDungeonRoom
    {
        private int width;
        private int height;
        private int pathLenght;
        private Stack<CrawlerData> pathNodes;
        private Queue<CrawlerData> nodesForFinalize;
        private GridSystem<CrawlerData> gridCrawler;
        public CrawlerData[,] GetGridSystem { get => gridCrawler.GridData; }
        public CrawlerDungeonRoom(int width, int height, int pathLenght)
        {
            this.width = width;
            this.height = height;
            this.pathLenght = pathLenght;

            RunCrawler();
        }
        private void RunCrawler()
        {
            gridCrawler = new GridSystem<CrawlerData>(new Vector2Int(width, height), CreateCrawlerData);
            //stack of used nodes
            pathNodes = new();
            //random position as start grid
            int rndX = Random.Range(1, width - 1);
            int rndZ = Random.Range(1, height - 1);
            Vector2Int crawlerPosition = new Vector2Int(rndX, rndZ);
            //add to stack
            pathNodes.Push(gridCrawler.GetNode(crawlerPosition));
            //get start crawler
            CrawlerData currtentCrawler = pathNodes.Peek();
            SetCrawler(currtentCrawler, true, RoomType.start, -Vector2Int.one);
            //set 3/4 nodes unwalkable
            var randomNodes = gridCrawler.GetNeighbours(currtentCrawler).OrderBy(x => Random.value).Take(3).ToList();            
            foreach (var item in randomNodes)
            {
                SetCrawler(item, true, RoomType.normal, crawlerPosition);
                nodesForFinalize.Enqueue(item);
            }
            int safe = 0;
            //draw path until its lenght will be right -> end node
            while (pathLenght > pathNodes.Count && safe < 500)
            {
                //safety check
                safe++;
                if (pathNodes.Count == 0)
                {
                    Debug.LogError("no more nodes!");
                    break;
                }
                //get neighbors with free positions, repeat while there are not free neighnour
                //true -> there is a free neighbour
                if (GetFreeNeighbours(currtentCrawler, out List<CrawlerData> freeNeighbors))
                {
                    //pick random   
                    currtentCrawler = freeNeighbors.RandomElement();
                    SetCrawler(currtentCrawler, true, RoomType.path, currtentCrawler.gridPosition);
                    //add to path
                    pathNodes.Push(gridCrawler.GetNode(currtentCrawler.gridPosition));
                    //add to finalize
                    nodesForFinalize.Enqueue(currtentCrawler);
                }
                else
                {
                    //previous crawler
                    RemoveCrawlerFromPath(currtentCrawler);
                    if(!pathNodes.TryPeek(out currtentCrawler))
                    {
                        Debug.LogError("no more previous nodes!");
                        break;
                    }
                }
            }
            var endNode = pathNodes.Peek();
            SetCrawler(endNode, true, RoomType.end, endNode.parentPosition);
            //make other rooms
            CompleteCrawling();
        }

        private bool GetFreeNeighbours(CrawlerData currtentCrawler, out List<CrawlerData> freeNeighbour)
        {
            freeNeighbour = gridCrawler.GetNeighbours(currtentCrawler).Where(x => !x.used).ToList();
            //no usable parents
            if(freeNeighbour.Count == 0)
            {
                RemoveCrawlerFromPath(currtentCrawler);
                return true;
            }
            return false;
        }

        private void CompleteCrawling()
        {
            //get throught             
            while (nodesForFinalize.Count > 0)
            {
                //take node and try add room
                var node = nodesForFinalize.Dequeue();
                //ignored rooms
                if(node.roomType == RoomType.end) continue;
                //get random free neighbour(if possible)
                if(GetFreeNeighbours(node, out var neighbours))
                {
                    //there is a free node
                    var rndNeighbor = neighbours.RandomElement();
                    SetCrawler(rndNeighbor, true, RoomType.normal, node.gridPosition);
                }
                else
                {
                    continue;
                }
            }
            //todo complete method
            //there is a high chance to unused(unconnected) nodes
            //find them and set until there will be all connected
            //queue - non usable nodes will be add to queue an proceeded at the end
            Queue<CrawlerData> finalizeNodes = new();
            foreach (var finalNode in GetGridSystem)
            {
                if(!finalNode.used) finalizeNodes.Enqueue(finalNode);
            }
            while (finalizeNodes.Count > 0)
            {
                //get node
                var actualNode = finalizeNodes.Dequeue();
                //get random used neighbours
                var parentNodeList = gridCrawler.GetNeighbours(actualNode).Where(x => x.used).ToList();
                if(parentNodeList.Count == 0)
                {
                    finalizeNodes.Enqueue(actualNode);
                    continue;
                } 
                SetCrawler(actualNode, true, RoomType.normal, parentNodeList.RandomElement().gridPosition);
            }
        }

        private void RemoveCrawlerFromPath(CrawlerData crawler)
        {
            pathNodes.Pop();
            SetCrawler(crawler, true, RoomType.normal, -Vector2Int.one);
        }
        private void SetCrawler(CrawlerData currtentCrawler, bool used, RoomType roomType, Vector2Int parent)
        {
            currtentCrawler.used = used;
            currtentCrawler.roomType = roomType;
            currtentCrawler.parentPosition = parent;
            gridCrawler.SetNode(currtentCrawler);
        }
        private CrawlerData CreateCrawlerData(Vector2Int gridPosition)
        {
            return new CrawlerData(gridPosition);
        }
        
    }
}

