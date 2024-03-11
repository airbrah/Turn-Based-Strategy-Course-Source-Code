using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    //This shouldn't be here. this is too specific
    string Name();
    bool IsActive();

    //This is fine
    void Interact(Action onInteractionComplete);

}
