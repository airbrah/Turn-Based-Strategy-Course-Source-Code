using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    //const are multiplied by 10
    private const int MOVE_STRAIGHT_COST = 10; //1
    private const int MOVE_DIAGONAL_COST = 14; //1.4 (square root of 2) [to move diagonal, must use two boxs. 1+1=2 then square root it]

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;

    [Header("TEST")]
    [SerializeField] private bool pathFindingDebugObjects;

    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one Pathfinding! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystem = new GridSystem<PathNode>(10, 10, 2f, 
                    (GridSystem<PathNode> gameObject,GridPos gridPos) 
                    => new PathNode(gridPos));
        if(pathFindingDebugObjects)
        {
            gridSystem.CreatDebugObjects(gridDebugObjectPrefab);
        }
    }
    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize,
            (GridSystem<PathNode> gridSystem, GridPos gridPos) => new PathNode(gridPos));

        if(pathFindingDebugObjects)
        {
            gridSystem.CreatDebugObjects(gridDebugObjectPrefab);
        }

        for(int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPos gridPos = new GridPos(x, z);
                Vector3 worldPos = LevelGrid.Instance.GetWorldPos(gridPos);
                float raycastOffsetDistance = 5f;
                if(Physics.Raycast(worldPos + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2, obstaclesLayerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }
    public List<GridPos> FindPath(GridPos startGridPos, GridPos endGridPos, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>(); // searching
        List<PathNode> closedList = new List<PathNode>();// already searched

        PathNode startNode = gridSystem.GetGridObject(startGridPos);
        PathNode endNode = gridSystem.GetGridObject(endGridPos);

        openList.Add(startNode);

        for(int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for(int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPos gridPos = new GridPos(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPos);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPos, endGridPos));
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);
        
            if(currentNode == endNode)
            {
                // Reached final node
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if(closedList.Contains(neighbourNode))
                {
                    continue;
                }

                if(!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPos(), neighbourNode.GetGridPos());
                if(tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPos(), endGridPos));
                    neighbourNode.CalculateFCost();

                    if(!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        // No path found
        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPos gridPosA, GridPos gridPosB)
    {
        GridPos gridPosDistance = gridPosA - gridPosB;
        int xDistance = Mathf.Abs(gridPosDistance.x);
        int zDistance = Mathf.Abs(gridPosDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for(int i = 0; i< pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }
    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPos(x, z));
    }
    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPos gridPos = currentNode.GetGridPos();

        if(gridPos.x - 1 >= 0)
        {
            //Left
            neighbourList.Add(GetNode(gridPos.x - 1, gridPos.z + 0));
            if(gridPos.z - 1 >= 0)
            {
                //Left Down
                neighbourList.Add(GetNode(gridPos.x - 1, gridPos.z - 1));
            }
            if (gridPos.z + 1 < gridSystem.GetHeight())
            {
                //Left Up
                neighbourList.Add(GetNode(gridPos.x - 1, gridPos.z + 1));
            }
        }
        if (gridPos.x + 1 < gridSystem.GetWidth())
        {
            //Right
            neighbourList.Add(GetNode(gridPos.x + 1, gridPos.z + 0));
            if (gridPos.z - 1 >= 0)
            {
                //Right Down
                neighbourList.Add(GetNode(gridPos.x + 1, gridPos.z - 1));
            }
            if (gridPos.z + 1 < gridSystem.GetHeight())
            {
                //Right Up
                neighbourList.Add(GetNode(gridPos.x + 1, gridPos.z + 1));
            }   
        }

        
        if (gridPos.z - 1 >= 0)
        {
            //Down
            neighbourList.Add(GetNode(gridPos.x + 0, gridPos.z - 1));
        }
        if (gridPos.z + 1 < gridSystem.GetHeight())
        {
            //Up
            neighbourList.Add(GetNode(gridPos.x + 0, gridPos.z + 1));
        }
        return neighbourList;
    }
    private List<GridPos> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPos> gridPosList = new List<GridPos>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPosList.Add(pathNode.GetGridPos());
        }
        return gridPosList;
    }

    public void SetIsWalkableGridPos(GridPos gridPos, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPos).SetIsWalkable(isWalkable);
    }
    public bool IsWalkableGridPos(GridPos gridPos)
    {
        return gridSystem.GetGridObject(gridPos).IsWalkable();
    }
    public bool HasPath(GridPos startGridPos, GridPos endGridPos)
    {
        return FindPath(startGridPos, endGridPos, out int pathLength) != null;
    }
    public int GetPathLength(GridPos startGridPos, GridPos endGridPos)
    {
        FindPath(startGridPos, endGridPos, out int pathLength);
        return pathLength;
    }
}
