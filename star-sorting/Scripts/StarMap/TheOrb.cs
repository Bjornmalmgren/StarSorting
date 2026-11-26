using Godot;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;


public partial class TheOrb : MeshInstance3D, IInteractable
{
    [Export]
    private Label3D interactPrompt;

   

    //add reference to starmap here 

    public string promptMessage => $"The ORB";
    public bool canInteract { get; set; } = true;
    //IInteractable implementation
    public void Interact()
    {
        GD.Print("The orb was interacted with");
        GameEvents.Instance.EmitStarmapStarted();

    }

    public void ShowPrompt()
    {
        if (!canInteract)
        {
            return;
        }
        interactPrompt.Text = promptMessage;
        interactPrompt.Visible = true;
    }

    public void HidePrompt()
    {
        interactPrompt.Visible = false;
    }
}
