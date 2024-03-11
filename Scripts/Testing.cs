using UnityEngine;

public class Testing : MonoBehaviour
{

    [SerializeField] private Unit unit;
    
    void Start()
    {
        
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            
        }
    }
}
/*
    GridPos mouseGridPos = LevelGrid.Instance.GetGridPos(MouseWorld.GetPos());
    GridPos startGridPos = new GridPos(0, 0);

    List<GridPos> gridPosList = Pathfinding.Instance.FindPath(startGridPos, mouseGridPos);
        
    for(int i = 0; i < gridPosList.Count - 1; i++)
    {
        Debug.DrawLine(
            LevelGrid.Instance.GetWorldPos(gridPosList[i]),
            LevelGrid.Instance.GetWorldPos(gridPosList[i + 1]),
            Color.white,
            10f
            );
    }
*/
