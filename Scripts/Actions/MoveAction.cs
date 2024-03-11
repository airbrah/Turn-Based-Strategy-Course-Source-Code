using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField] private int maxMoveDistance = 4;
    private List<Vector3> posList;
    private int currentPosIndex;
    private const int VIEW_DIST = 1;
    

    void Update()
    {
        if (!isActive)
        {
            return;
        }
        Vector3 targetPos = posList[currentPosIndex];
        Vector3 moveDirection = (targetPos - transform.position).normalized;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        float stoppingDist = .1f;
        
        if (Vector3.Distance(transform.position, targetPos) > stoppingDist)
        {
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPosIndex++;
            if(currentPosIndex >= posList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
    }
    public override void TakeAction(GridPos gridPos, Action onActionComplete)
    {
        List<GridPos> pathGridPosList = Pathfinding.Instance.FindPath(unit.GetGridPos(), gridPos, out int pathLength);

        currentPosIndex = 0;
        posList = new List<Vector3>();
        foreach(GridPos pathGridPos in pathGridPosList)
        {
            posList.Add(LevelGrid.Instance.GetWorldPos(pathGridPos));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }
    public override List<GridPos> GetValidActionGridPosList()
    {
        List<GridPos> validGridPosList = new List<GridPos>();

        GridPos unitGridPos = unit.GetGridPos();

        for(int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPos offsetGridPos = new GridPos(x, z);
                GridPos testGridPos = unitGridPos + offsetGridPos;

                if(!LevelGrid.Instance.IsValidGridPos(testGridPos))
                {
                    //figure if grid space is valid bound
                    continue;
                }
                if(unitGridPos == testGridPos)
                {
                    //don't count the space the current unit is currently on
                    continue;
                }
                if(LevelGrid.Instance.HasAnyUnitOnGridPos(testGridPos))
                {
                    //see if a unit occupis that space
                    continue;
                }
                if(!Pathfinding.Instance.IsWalkableGridPos(testGridPos))
                {
                    //see if an obsticle obstructs this gridPos based on pathfinding
                    continue;
                }
                if(!Pathfinding.Instance.HasPath(unitGridPos, testGridPos))
                {
                    //see if gridPos can be reached based on pathfinding
                    continue;
                }
                int pathfindingDistanceMultiplier = 10;
                
                if (Pathfinding.Instance.GetPathLength(unitGridPos, testGridPos) > maxMoveDistance * pathfindingDistanceMultiplier)
                {
                    //Path length is too long
                    continue;
                }
                validGridPosList.Add(testGridPos); //Add the grid spot
            }
        }

        return validGridPosList;
    }
    public override string GetActionName()
    {
        return "Move";
    }
    //AI
    private List<GridPos> CheckAdjacentGridForUnit(GridPos gridPos)
    {
        List<GridPos> adjacentGrids = new List<GridPos>();

        for (int x = -VIEW_DIST; x <= VIEW_DIST; x++)
        {
            for (int z = -VIEW_DIST; z <= VIEW_DIST; z++)
            {
                GridPos offsetGridPosition = new GridPos(x, z);
                GridPos testGridPosition = gridPos + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPos(testGridPosition))
                {
                    //Is it part of the LevelGrid?
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPos(testGridPosition))
                {
                    // Grid Position is empty, no Unit
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPos(testGridPosition);
                

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    // Both Units on same 'team'
                    continue;
                }

                adjacentGrids.Add(testGridPosition);
            }
        }

        return adjacentGrids;
    }
    public override EnemyAIAction GetEnemyAIAction(GridPos gridPos)
    {
        //figure score base on Units in move path.
        List<GridPos> adjacentGrids = CheckAdjacentGridForUnit(gridPos);


        int countUnits = adjacentGrids.Count;

        return new EnemyAIAction()
        {
            gridPos = gridPos,
            actionValue = 10 * countUnits,
        };
    } //Score
}