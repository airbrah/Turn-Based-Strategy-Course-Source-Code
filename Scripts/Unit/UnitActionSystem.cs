using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionTaken;
    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;
    
    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }
    private void Update()
    {
        if(isBusy)
        {
            return;
        }
        if(!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if(TryHandleUnitSelection())
        {
            return;
        }
        HandleSelectedAction();
    }
    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            GridPos mouseGridPos = LevelGrid.Instance.GetGridPos(MouseWorld.GetPos());
            if(!selectedAction.IsValidActionGridPos(mouseGridPos))
            { 
                return; 
            }
            
            if(!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
            {
                return;    
            }

            SetBusy();
            selectedAction.TakeAction(mouseGridPos, ClearBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty);

            //figure a way to invoke this to only play when move is performed. can we do something different here?
            OnActionTaken?.Invoke(this, EventArgs.Empty);
        }
    }
    private void SetBusy()
    {
        isBusy = true;

        OnBusyChanged?.Invoke(this, isBusy);
    }
    private void ClearBusy()
    {
        isBusy = false;

        OnBusyChanged?.Invoke(this, isBusy);
    }
    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPos());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if(unit == selectedUnit)
                    {
                        return false;
                    }

                    if (unit.IsEnemy())
                    {
                        return false;
                    }
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }
    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }
    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
}