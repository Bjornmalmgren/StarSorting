using Godot;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;

public partial class InteractionRay : RayCast3D
{
    private IInteractable currentInteractable;
    private IPickable currentPickable;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}
    public void Enable()
    {
        Enabled = true;
    }

    public void Disable()
    {
        Enabled = false;
        currentInteractable?.HidePrompt();
        currentInteractable = null;
        currentPickable = null;
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!Enabled) return;

        IInteractable hitInteractable = null;
        IPickable hitPickable = null;

        if (IsColliding())
        {
            var collider = GetCollider() as Node;
            var target = (collider is IInteractable || collider is IPickable) ? collider : collider?.GetParent();

            if (target is IInteractable interactable && interactable.CanInteract)
                hitInteractable = interactable;

            if (target is IPickable pickable && pickable.CanPickUp)
                hitPickable = pickable;
        }

        if (hitInteractable != currentInteractable)
        {
            currentInteractable?.HidePrompt();
            currentInteractable = hitInteractable;
            currentInteractable?.ShowPrompt();
        }

        currentPickable = hitPickable;
    }

    public void Interact(Node interactor)
    {
        if (currentInteractable != null && currentInteractable.CanInteract)
        {
            currentInteractable.Interact(interactor);
        }
    }

    public IPickable GetPickable()
    {
        return currentPickable;
    }
}
