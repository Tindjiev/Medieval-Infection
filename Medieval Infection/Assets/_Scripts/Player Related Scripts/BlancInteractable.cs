using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlancInteractable : MonoBehaviour, Interactable
{
    public string TextOnScreen => "blanc interaction";

    public void Interact()
    {
        Debug.Log("interacted with " + name);
    }
}
