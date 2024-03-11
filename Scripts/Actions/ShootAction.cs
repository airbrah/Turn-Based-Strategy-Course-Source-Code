using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Attach to Unit Prefab
//Perform the shoot action and its logic
public class ShootAction : BaseAction
{
    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }
    private State state;
    private float stateTimer;
    private int maxShootDistance = 7;
    private bool canShootBullet;

    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private int maxRNGRange = 85;
    [SerializeField] private int minRNGRange = 65;
    [SerializeField] private float damageDealed;
    private System.Random rnd = new System.Random();
    private float randomNum;

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                Vector3 aimDir = (targetUnit.GetWorldPos() - unit.GetWorldPos()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if(canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }
    private void Shoot()
    {
        //Events
        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });

        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });

        //Damage
        randomNum = rnd.Next(minRNGRange, maxRNGRange);
        damageDealed = randomNum.ConvertTo<float>();

        targetUnit.Damage(damageDealed);
        //In the future, for different guns, different ranges, adjust damage accordingly
        //Also, you could add a randomizer to RNG the shoot based on differnt things
        //like range, what gun, and stats of unit

    }
    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                float coolOffStateTime = 0.5f;
                stateTimer = coolOffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }
    public override string GetActionName()
    {
        return "Shoot";
    }
    public override List<GridPos> GetValidActionGridPosList()
    {
        GridPos unitGridPos = unit.GetGridPos();
        return GetValidActionGridPosList(unitGridPos);
    }
    public List<GridPos> GetValidActionGridPosList(GridPos unitGridPos)
    {
        List<GridPos> validGridPosList = new List<GridPos>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPos offsetGridPos = new GridPos(x, z);
                GridPos testGridPos = unitGridPos + offsetGridPos;

                if (!LevelGrid.Instance.IsValidGridPos(testGridPos))
                {
                    //figure if grid space is valid bound
                    continue;
                }
                
                //How far the shot can go
                int testDist = Mathf.Abs(x) + Mathf.Abs(z);
                if(testDist > maxShootDistance)
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPos(testGridPos))
                {
                    //See if grid position is empty, no unit to shoot
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPos(testGridPos);
                
                if(targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    // Both Units on the same 'team'
                    continue;
                }
                Vector3 unitWorldPos = LevelGrid.Instance.GetWorldPos(unitGridPos);
                Vector3 shootDir = (targetUnit.GetWorldPos() - unitWorldPos).normalized;
                float unitShoulderHeight = 1.7f;
                if(Physics.Raycast(unitWorldPos + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(unitWorldPos, targetUnit.GetWorldPos()),
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
        targetUnit = LevelGrid.Instance.GetUnitAtGridPos(gridPos);

        
        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;

        ActionStart(onActionComplete);
    }
    public Unit GetTargetUnit()
    {
        return targetUnit;
    }
    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }
    //AI
    public override EnemyAIAction GetEnemyAIAction(GridPos gridPos)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPos(gridPos);
        
        return new EnemyAIAction()
        {
            gridPos = gridPos,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
        };
    } //Score
    public int GetTargetCountAtPos(GridPos gridPos)
    {
        return GetValidActionGridPosList(gridPos).Count;
    }
}
