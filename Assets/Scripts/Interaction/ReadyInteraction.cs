using Ineraction;
using UnityEngine;

public class ReadyInteraction : Interactable
{


    public override void Interact(Interactor actor = null)
    {
        ElevatorTeleporter.Instance.AddPlayer(actor.gameObject);
    }
}
