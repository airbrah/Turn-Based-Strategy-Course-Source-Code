using System.Collections.Generic;
using System;
using UnityEngine;

public class SpinAction : BaseAction
{
    private const float MAX_SPIN = 360;
    private float totalSpinAmount;
    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        float spinAddAmount = 360f * Time.deltaTime;
        totalSpinAmount += spinAddAmount;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

        if (totalSpinAmount >= MAX_SPIN)
        {
            ActionComplete();
        }
    }
    public override void TakeAction(GridPos gridPos, Action onActionComplete)
    {
        totalSpinAmount = 0;
        ActionStart(onActionComplete);
    }
    public override string GetActionName()
    {
        return "Spin";
    }
    public override List<GridPos> GetValidActionGridPosList()
    {
        GridPos unitGridPos = unit.GetGridPos();
        return new List<GridPos>
        {
            unitGridPos
        };
    }
    public override int GetActionPointCost()
    {
        return 1;
    }
    //AI
    public override EnemyAIAction GetEnemyAIAction(GridPos gridPos)
    {
        return new EnemyAIAction()
        {
            gridPos = gridPos,
            actionValue = 0,
        };
    }
}
