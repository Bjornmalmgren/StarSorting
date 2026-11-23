using Godot;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;

public partial class InteractionRay : RayCast3D
{
    private IInteractable currentInteractable;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		 IInteractable hitInteractable = null;

        if (IsColliding())
        {
            var collidingObject = GetCollider() as Node;
            // Checks the collider first, or its parent (matching your current node setup)
            var target = collidingObject is IInteractable ? collidingObject : collidingObject?.GetParent();

            if (target is IInteractable interactable && interactable.CanInteract)
            {
                hitInteractable = interactable;
                currentInteractable = interactable;
            }
            currentInteractable.ShowPrompt();
        }
        else
        {
            currentInteractable?.HidePrompt();
            currentInteractable = null;
            
        }
        if (currentInteractable != null)
        {
           
        }

        
      
	}
	public Node Interact(Node interactor)
	{
        // Reuse the interactable already detected in _Process
        if (currentInteractable != null && currentInteractable.CanInteract)
        {
            GD.Print("Interacted with interactable");
            currentInteractable.Interact(interactor);
            return currentInteractable as Node;
        }

        GD.Print("Failed to interact");
        return null;
    }
}
