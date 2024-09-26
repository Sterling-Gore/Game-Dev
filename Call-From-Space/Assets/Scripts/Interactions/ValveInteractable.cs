using UnityEngine;

public class ValveInteractable : Interactable
{
    public ValveController valveController;

    public override string GetDescription()
    {
        return "Press [E] to turn the valve";
    }

    public override void Interact()
    {
        valveController.ToggleValve();
    }
}