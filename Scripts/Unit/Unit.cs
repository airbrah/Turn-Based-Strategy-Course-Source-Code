using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 7;
    private const int VISION_POINTS = 7;
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    private GridPos gridPos;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    [SerializeField] private int actionPonts = ACTION_POINTS_MAX;
    [SerializeField] private bool isEnemy;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>(); // all the actions that derive
    }
    private void Start()
    {
        gridPos = LevelGrid.Instance.GetGridPos(transform.position);
        LevelGrid.Instance.AddUnitAtGridPos(gridPos, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }
    private void Update()
    {
        GridPos newGridPos = LevelGrid.Instance.GetGridPos(transform.position);
        GridPos oldGridPos;

        if (newGridPos != gridPos)
        {
            oldGridPos = gridPos;
            gridPos = newGridPos;
            LevelGrid.Instance.UnitMovedGridPos(this, oldGridPos, newGridPos);
        }

        Quaternion holdXRotation = this.transform.rotation;
        holdXRotation.x = 0f;
        this.transform.rotation = holdXRotation;
    }
    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }
    private void SpendActionPoints(int amount)
    {
        actionPonts -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }
    public void Damage(float totalDamageAmount)
    {
        healthSystem.Damage(totalDamageAmount);
    }

    //VALUE
    public GridPos GetGridPos()
    {
        return gridPos;
    }
    public Vector3 GetWorldPos()
    {
        return transform.position;
    }
    public Quaternion GetWorldRotation()
    {
        return transform.rotation;
    }
    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }
    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CanSpendActionPointsToTakeAction(BaseAction BaseAction)
    {
        if (actionPonts >= BaseAction.GetActionPointCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsEnemy()
    {
        return isEnemy;
    }

    public int GetActionPoints()
    {
        return actionPonts;
    }
    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }
    
    //EVENTS
    private void TurnSystem_OnTurnChanged(object sender, EventArgs eventArgs)
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPonts = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        } //Reset action points to max points when changing from either player's turn or eneimees turn
    }
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPos(gridPos, this);

        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }
}













////CUSTOM FUNCTION OF MINE
//public bool IsDead()
//{
//    return isDead;
//}
////CUSTOM FUNCTION OF MINE