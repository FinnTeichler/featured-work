using System.Collections.Generic;
using UBGKO.Party.Controls;
using UnityEngine;

namespace UBGKO.Interactables
{
    public interface IInteractable
    {
        InteractableID ID { get; }

        string GetInteractableName();
        Vector3 GetInteractionPoint();
        Vector3 GetInteractionFacingPoint();

        List<InteractionType> GetAvailableInteractions();
        void Interact(InteractionType type);
    }
}