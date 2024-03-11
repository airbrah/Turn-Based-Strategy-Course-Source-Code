using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;
    protected Unit unit;
    protected Unit targetUnit;
    protected Action onActionComplete;
    protected bool isActive;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }
    protected virtual void Start()
    {
        
    }
    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }
    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }
    public abstract string GetActionName();
    public abstract void TakeAction(GridPos gridPos, Action onActionComplete); //We could seperate gridPos so that not all actions need a gridPos
    public abstract List<GridPos> GetValidActionGridPosList();
    public virtual bool IsValidActionGridPos(GridPos gridPos)
    {
        List<GridPos> validGridPosList = GetValidActionGridPosList();
        return validGridPosList.Contains(gridPos);
    }
    public virtual int GetActionPointCost()
    {
        return 1;
    }
    public Unit GetUnit()
    {
        return unit;
    }
    //AI
    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPos> validActionGridPosList = GetValidActionGridPosList();

        foreach(GridPos gridPos in validActionGridPosList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPos); //GetEnemyAIAction is abstract, this will check in whatever action calls this and preform its duties
            enemyAIActionList.Add(enemyAIAction);
        } //This adds all possibilities to enemy AI actions it can take to a list. scores accordinly

        if(enemyAIActionList.Count > 0) 
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActionList[0]; //The first one in the list after sorting would be the best option to take based on score
        } //If there are actions in the list that are possiable, select actions.

        else 
        {
            return null;
        } //Theres no actions

    }
    public abstract EnemyAIAction GetEnemyAIAction(GridPos gridPos);
}