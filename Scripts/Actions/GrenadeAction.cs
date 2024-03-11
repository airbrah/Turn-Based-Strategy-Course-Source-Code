using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Transform grenadeProjectilePrefab;
    private int maxThrowDistance = 4;
    [SerializeField] private LayerMask obstaclesLayerMask;


    private void Update()
    {
        if(!isActive)
        {
            return;
        }
    }

    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPos gridPos)
    {
        return new EnemyAIAction
        {
            gridPos = gridPos,
            actionValue = 0,
        };
    }

    public override List<GridPos> GetValidActionGridPosList()
    {
        List<GridPos> validGridPosList = new List<GridPos>();

        GridPos unitGridPos = unit.GetGridPos();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPos offsetGridPos = new GridPos(x, z);
                GridPos testGridPos = unitGridPos + offsetGridPos;

                float unitShoulderHeight = 1.7f;

                if (!LevelGrid.Instance.IsValidGridPos(testGridPos))
                {
                    //figure if grid space is valid bound
                    continue;
                }

                if(testGridPos == unitGridPos)
                {
                    //Can't throw grenade onto yourself
                    continue;
                }

                int testDist = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDist > maxThrowDistance)
                {
                    //circle radius to throw to
                    continue;
                }

                Vector3 unitWorldPos = LevelGrid.Instance.GetWorldPos(unitGridPos);
                Vector3 gridWorldPos = new Vector3(testGridPos.x, 0f, testGridPos.z) * 2;
                if (Physics.Raycast(unitWorldPos + Vector3.up * unitShoulderHeight,
                    (gridWorldPos - unitWorldPos).normalized,
                    Vector3.Distance(unitWorldPos, gridWorldPos),
                    obstaclesLayerMask))
                {
                    // Blocked by an obstacle
                    continue;
                }

                validGridPosList.Add(testGridPos);
            }
        }

        return validGridPosList;
    }

    public override void TakeAction(GridPos gridPos, Action onActionComplete)
    {
        Transform grenadeProjectileTranform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPos(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTranform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.SetUp(gridPos, OnGrenadeBehaviourComplete);
        ActionStart(onActionComplete);
    }

    public override int GetActionPointCost()
    {
        return 3;
    }
    public int GetMaxThrowDistance()
    {
        return maxThrowDistance;
    }

    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }

    private void OnDrawGizmos()
    {
        //GridPos unitGridPos = unit.GetGridPos();

        //for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        //{
        //    for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
        //    {
        //        Vector3 unitWorldPos = LevelGrid.Instance.GetWorldPos(unitGridPos);
        //        GridPos offsetGridPos = new GridPos(x, z);


        //        GridPos testGridPos = unitGridPos + offsetGridPos;

        //        Vector3 testWorldPos = new Vector3(testGridPos.x, 0f, testGridPos.z) * 2;

        //        Vector3 worldPos = LevelGrid.Instance.GetWorldPos(offsetGridPos);
        //        float unitShoulderHeight = 1.7f;

        //        Debug.DrawRay(unitWorldPos + Vector3.up * unitShoulderHeight,
        //            (testWorldPos - unitWorldPos).normalized * Vector3.Distance(unitWorldPos, testWorldPos),
        //            Color.red);

        //    }
        //}
    }
}
