using System;
using System.Collections.Generic;
using UnityEngine;
using static GridSystemVisual;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld instance;
    [SerializeField] private LayerMask mousePlaneLayerMask;

    private Unit currentSelectedUnit;
    private GridPos currentUnitGridPos;
    private BaseAction selectedAction;
    private bool hidePathfindingLine;
    private Vector3 offsetVector = new Vector3(0, 1f, 0f);

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        GetCurrentUnitGridPos();
    }
    private void Update()
    {
        GridPos mouseGridPos = LevelGrid.Instance.GetGridPos(GetPos());
        if(LevelGrid.Instance.IsValidGridPos(mouseGridPos))
        {
            Vector3 mouseVectorPos = LevelGrid.Instance.GetWorldPos(mouseGridPos);
            transform.position = mouseVectorPos + offsetVector;
        }

        if(!hidePathfindingLine)
        {
            DrawLinePathfinding();
        }
    }
    public static Vector3 GetPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPos());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        
        //Could add condition here if raycast is or isnt hitting anything, then return something else

        return raycastHit.point;
    }
    private void DrawLinePathfinding()
    {
        GridPos mouseGridPos = LevelGrid.Instance.GetGridPos(GetPos());
        GridPos unitGridPos = currentUnitGridPos;

        if(LevelGrid.Instance.IsValidGridPos(mouseGridPos))
        {
            List<GridPos> gridPosList = Pathfinding.Instance.FindPath(unitGridPos, mouseGridPos, out int pathLength);
            if(gridPosList == null)
            {
                return;
            }
            for (int i = 0; i < gridPosList.Count - 1; i++)
            {
                Debug.DrawLine(
                    LevelGrid.Instance.GetWorldPos(gridPosList[i]) + offsetVector,
                    LevelGrid.Instance.GetWorldPos(gridPosList[i + 1]) + offsetVector,
                    Color.blue
                    );
            }
        }
    }
    private void GetCurrentUnitGridPos()
    {
        currentSelectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        currentUnitGridPos = currentSelectedUnit.GetGridPos();
    }
    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case MoveAction moveAction:
                hidePathfindingLine = true;
                break;
        }
        
    }
    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case MoveAction moveAction:
                GetCurrentUnitGridPos();
                hidePathfindingLine = false;
                break;
        }
    }
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        GetCurrentUnitGridPos();
    }
    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        if(selectedAction is MoveAction)
        {
            hidePathfindingLine = false;
        }
        else
        {
            hidePathfindingLine = true;
        }
    }
}