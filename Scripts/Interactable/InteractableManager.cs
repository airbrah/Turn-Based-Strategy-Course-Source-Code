using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager Instance { get; private set; }

    private List<Interactable> interactableList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InteractableManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        interactableList = new List<Interactable>();
    }
    private void Start()
    {
        Interactable.OnAnyInteractableSpawned += Interactable_OnAnyInteractableSpawned;
        Interactable.OnAnyInteractableDestroyed += Interactable_OnAnyInteractableDestroyed;
    }

    private void Interactable_OnAnyInteractableSpawned(object sender, EventArgs e)
    {
        Interactable interactable = sender as Interactable;
        interactableList.Add(interactable);
    }
    private void Interactable_OnAnyInteractableDestroyed(object sender, EventArgs e)
    {
        Interactable interactable = sender as Interactable;
        interactableList.Remove(interactable);
    }

    public List<Interactable> GetInteractableList()
    {
        return interactableList;
    }
}
