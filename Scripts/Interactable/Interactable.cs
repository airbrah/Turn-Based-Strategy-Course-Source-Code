using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public static event EventHandler OnAnyInteractableSpawned;
    public static event EventHandler OnAnyInteractableDestroyed;
    private void Awake()
    {
        OnAnyInteractableSpawned?.Invoke(this, EventArgs.Empty);
    }
    private void OnDestroy()
    {
        OnAnyInteractableDestroyed?.Invoke(this, EventArgs.Empty);
    }
}
