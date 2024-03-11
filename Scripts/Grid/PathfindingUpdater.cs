using System;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    private void Start()
    {
        DestructibleCrate.OnAnyDestroyed += OnAnyDestroyed_OnAnyDestroyed;
    }

    private void OnAnyDestroyed_OnAnyDestroyed(object sender, EventArgs e)
    {
        DestructibleCrate destructibleCrate = sender as DestructibleCrate;
        Pathfinding.Instance.SetIsWalkableGridPos(destructibleCrate.GetGridPos(), true);
    }
}
