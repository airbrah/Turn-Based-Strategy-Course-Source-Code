using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    private int maxInteractDistance = 1;


    private void Update()
    {
        if (!isActive)
        {
            return;
        }
    }

    public override string GetActionName()
    {
        return "Interact";
    }
    public override List<GridPos> GetValidActionGridPosList()
    {
        List<GridPos> validGridPositionList = new List<GridPos>();

        GridPos unitGridPosition = unit.GetGridPos();

        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
            {
                GridPos offsetGridPosition = new GridPos(x, z);
                GridPos testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPos(testGridPosition))
                {
                    continue;
                }

                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPos(testGridPosition);

                if (interactable == null)
                {
                    // No interactable on this GridPosition
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }
    public override void TakeAction(GridPos gridPosition, Action onActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPos(gridPosition);

        interactable.Interact(OnInteractComplete);

        ActionStart(onActionComplete);
    }
    private void OnInteractComplete()
    {
        ActionComplete();
    }
    //AI
    public override EnemyAIAction GetEnemyAIAction(GridPos gridPosition)
    {
        int testNum;
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPos(gridPosition);
        if(interactable.Name() == "Door" && !interactable.IsActive())
        {
            testNum = 50;
        }
        else
        {
            testNum = 0;
        }
        return new EnemyAIAction
        {
            gridPos = gridPosition,
            actionValue = testNum
        };
    }
}
