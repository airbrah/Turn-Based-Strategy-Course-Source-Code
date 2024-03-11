using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    }
    private State state;
    private float timer;
    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }
    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }
    private void Update()
    {
        //Debug.Log(state);
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return; 
        } //Its the player's turn. ignore rest of code till its false
        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break; //Stays in this state when player's turn 
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = State.Busy;
                    if(TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    } //We are performing an action, stay in Busy state
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                        state = State.WaitingForEnemyTurn;
                    } //no more actions to preform, end enemy turn
                }
                break; //triggered to this state when OnTurnChanged is triggered
            case State.Busy:
                break;
        }
    }
    //Custom Functions
    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }
    //Events
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
    }
    //Return Values
    /// <summary>
    /// This looks at each unit to see if it can preform an action
    /// </summary>
    /// <param name="onEnemyAIActionComplete"></param>
    /// <returns></returns>
    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if(TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }
        return false;
    } 
    /// <summary>
    /// This checks all actions for unit, see which is the best option at the time, and send back to TryTakeEnemyAIAction
    /// </summary>
    /// <param name="enemyUnit"></param>
    /// <param name="onEnemyAIActionComplete"></param>
    /// <returns></returns>
    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach(BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if(!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                continue;
            } //Enemy is out of action points

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }//If there is no set of possiable actions, start getting possiable actions to do so. And then set which action it is
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if(testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                } // to find the best of the best actions to do
            } //If there is actions to be had, continue
        } //This loop cycles through all the actions we have on given Unit and 'ask' BaseAction if there are any possiable task that can be performed for said action


        //DO or DONT take action
        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPos, onEnemyAIActionComplete);
            return true;
        } //Preform the action. get 'um!
        else
        {
            return false;
        } //no more actions. fresh out for this unit
    } 
}
