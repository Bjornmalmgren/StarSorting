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

    public void Interact(Node interactor)
    {
         
    }
}
