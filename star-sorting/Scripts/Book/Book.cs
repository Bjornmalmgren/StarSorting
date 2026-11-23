using Godot;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

[GlobalClass]
public partial class Book : StaticBody3D, IInteractable
{
    [Export]
    public string BookTitle { get; set; } = "DefaultName";

    [Export] 
    public bool CanInteract { get; set; } = true;

    public string PromptMessage => $"Read '{BookTitle}'";

    [Export]
    private Label3D interactPrompt;

    public void Interact(Node interactor)
    {
         
    }
    public void ShowPrompt()
    {
        if (!CanInteract)
        {
            return;
        }
        interactPrompt.Text = PromptMessage;
        interactPrompt.Visible = true;
    }

    public void HidePrompt()
    {
        interactPrompt.Visible = false;
    }
}
