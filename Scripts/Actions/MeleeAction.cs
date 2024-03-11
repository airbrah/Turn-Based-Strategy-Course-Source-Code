using System;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAction : BaseAction
{
    public static event EventHandler OnAnyMeleeHit;
    public event EventHandler OnMeleeActionStarted;
    public event EventHandler OnMeleeActionCompleted;
    private enum State
    {
        SwingingMeleeBeforeHit,
        SwingingMeleeAfterHit,
    }

    private int maxMeleeDistance = 1;
    private State state;
    private float stateTimer;


    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.SwingingMeleeBeforeHit:
                Vector3 aimDir = (targetUnit.GetWorldPos() - unit.GetWorldPos()).normalized;

                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                break;
            case State.SwingingMeleeAfterHit:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }
    private void NextState()
    {
        switch (state)
        {
            case State.SwingingMeleeBeforeHit:
                state = State.SwingingMeleeAfterHit;
                float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(100);
                OnAnyMeleeHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingingMeleeAfterHit:
                OnMeleeActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }
    public int GetMaxMeleeDistance()
    {
        return maxMeleeDistance;
    }
    public override string GetActionName()
    {
        return "Melee";
    }
    public override List<GridPos> GetValidActionGridPosList()
    {
        GridPos unitGridPos = unit.GetGridPos();
        return GetValidActionGridPosList(unitGridPos);
    }
    public List<GridPos> GetValidActionGridPosList(GridPos unitGridPos)
    {
        List<GridPos> validGridPosList = new List<GridPos>();

        for (int x = -maxMeleeDistance; x <= maxMeleeDistance; x++)
        {
            for (int z = -maxMeleeDistance; z <= maxMeleeDistance; z++)
            {
                GridPos offsetGridPosition = new GridPos(x, z);
                GridPos testGridPosition = unitGridPos + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPos(testGridPosition))
                {
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

                validGridPosList.Add(testGridPosition);
            }
        }

        return validGridPosList;
    }
    public override void TakeAction(GridPos gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPos(gridPosition);

        state = State.SwingingMeleeBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;

        OnMeleeActionStarted?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }
    //AI
    public override EnemyAIAction GetEnemyAIAction(GridPos gridPos)
    {

        return new EnemyAIAction
        {
            gridPos = gridPos,
            actionValue = 200,
        };
    }
}
