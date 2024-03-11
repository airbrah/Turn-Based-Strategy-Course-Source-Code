using System;
using System.Collections.Generic;
using UnityEngine;
using static GridSystemVisual;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    public static GridSystemVisual Instance { get; private set; }
    [Serializable]public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow,
        Green
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
                LevelGrid.Instance.GetWidth(),
                LevelGrid.Instance.GetHeight()
            ];
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPos gridPos = new GridPos(x, z);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPos(gridPos), Quaternion.identity);

                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPos += LevelGrid_OnAnyUnitMovedGridPos;

        UpdateGridVisual();
    }
    
    public void HideAllGridPos()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }
    private void ShowGridPosRange(GridPos gridPos, int range, GridVisualType gridVisualType)
    {
        List<GridPos> gridPosList = new List<GridPos>();
        for(int x = -range; x <= range; x++)
        {
            for(int z = -range; z <= range; z++)
            {
                GridPos testGridPos = gridPos + new GridPos(x, z);
                if (!LevelGrid.Instance.IsValidGridPos(testGridPos))
                {
                    continue;
                }
                int testDist = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDist > range)
                {
                    continue;
                }
                gridPosList.Add(testGridPos);
            }
        }
        ShowGridPosList(gridPosList, gridVisualType);
    }
    private void ShowGridPosRangeSquare(GridPos gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPos> gridPositionList = new List<GridPos>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPos testGridPosition = gridPosition + new GridPos(x, z);
                if (!LevelGrid.Instance.IsValidGridPos(testGridPosition))
                {
                    continue;
                }
                gridPositionList.Add(testGridPosition);
            }
        }
        ShowGridPosList(gridPositionList, gridVisualType);
    } //range of 1
    public void ShowGridPosList(List<GridPos> gridPosList, GridVisualType gridVisualType)
    {
        foreach(GridPos gridPos in gridPosList)
        {
            gridSystemVisualSingleArray[gridPos.x, gridPos.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }
    private void UpdateGridVisual()
    {
        HideAllGridPos();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;
        switch(selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPosRange(selectedUnit.GetGridPos(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;
                ShowGridPosRange(selectedUnit.GetGridPos(), grenadeAction.GetMaxThrowDistance(), GridVisualType.RedSoft);
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPosRangeSquare(selectedUnit.GetGridPos(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
                break;
            case MeleeAction meleeAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPosRangeSquare(selectedUnit.GetGridPos(), meleeAction.GetMaxMeleeDistance(), GridVisualType.RedSoft);
                break;
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;

        }

        ShowGridPosList(selectedAction.GetValidActionGridPosList(), gridVisualType);
        
    }
    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    private void LevelGrid_OnAnyUnitMovedGridPos(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach(GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if(gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }
        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }
}
