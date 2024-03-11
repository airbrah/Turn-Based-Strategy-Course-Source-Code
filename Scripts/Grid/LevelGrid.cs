using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public event EventHandler OnAnyUnitMovedGridPos;

    public static LevelGrid Instance { get; private set; }
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    private GridSystem<GridObject> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one LevelGrid! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystem = new GridSystem<GridObject>(width, height, cellSize,
            (GridSystem<GridObject> g,
            GridPos gridPos) => new GridObject(g, gridPos));
        //gridSystem.CreatDebugObjects(gridDebugObjectPrefab);
    } //Construct the Level Grid
    private void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize);
    }
    void Update()
    {
        
    }
    public void AddUnitAtGridPos(GridPos gridPos, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPos);
        gridObject.AddUnit(unit);
    }
    public List<Unit> GetListUnitAtGridPos(GridPos gridPos)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPos);
        return gridObject.GetUnitList();
    }
    public void RemoveUnitAtGridPos(GridPos gridPos, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPos);
        gridObject.RemoveUnit(unit);
    }
    public void UnitMovedGridPos(Unit unit, GridPos fromGridPos, GridPos toGridPos)
    {
        RemoveUnitAtGridPos(fromGridPos, unit);
        AddUnitAtGridPos(toGridPos, unit);
        OnAnyUnitMovedGridPos?.Invoke(this, EventArgs.Empty);
    }
    //Pass through functions
    public GridPos GetGridPos(Vector3 worldPos) => gridSystem.GetGridPos(worldPos);
    public Vector3 GetWorldPos(GridPos gridPos) => gridSystem.GetWorldPos(gridPos);
    public bool IsValidGridPos(GridPos gridPos) => gridSystem.IsValidGridPos(gridPos);
    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
    public bool HasAnyUnitOnGridPos(GridPos gridPos)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPos);
        return gridObject.HasAnyUnit();
    }
    public Unit GetUnitAtGridPos(GridPos gridPos)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPos);
        return gridObject.GetUnit();
    }

    public IInteractable GetInteractableAtGridPos(GridPos gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetInteractable();
    }
    public void SetInteractableAtGridPos(GridPos gridPosition, IInteractable interactable)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.SetInteractable(interactable);
    }

}
