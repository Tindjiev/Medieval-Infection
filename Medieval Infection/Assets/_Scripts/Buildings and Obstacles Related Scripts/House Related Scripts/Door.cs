using UnityEngine;

public class Door : MonoBehaviour,Interactable
{
    public House House { get; private set; }

    private void Awake()
    {
        House = this.getvars<House>();
    }

    public void Interact()
    {
        House.EnterBuilding();
    }

    public string TextOnScreen => "Enter House";
}
