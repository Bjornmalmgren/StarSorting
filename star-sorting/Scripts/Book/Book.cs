using Godot;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

[GlobalClass]
public partial class Book : RigidBody3D, IInteractable, IPickable
{
    [Export]
    public string BookTitle { get; set; } = "DefaultName";

    [Export] 
    public bool CanInteract { get; set; } = true;
    [Export] public bool CanPickUp { get; set; } = true;
    
    [Export]
    public PackedScene CustomBookScene { get; set; }

    public string PromptMessage => $"Read '{BookTitle}'";

    [Export]
    private Label3D interactPrompt;

    private uint defaultCollisionLayer;
    private uint defaultCollisionMask;

    public override void _Ready()
    {
        // Save original physics layers so we can restore them on drop
        defaultCollisionLayer = CollisionLayer;
        defaultCollisionMask = CollisionMask;
    }
    public void OnPickUp(Node3D holder)
    {
        Freeze = true;
        CollisionLayer = 0; // Prevents the held book from pushing the player or hitting walls
        CollisionMask = 0;
        HidePrompt();

        Reparent(holder);
        GlobalPosition = holder.GlobalPosition;
        GlobalRotation = holder.GlobalRotation;
    }

    public void OnDrop()
    {
        Reparent(GetTree().CurrentScene);
        Freeze = false;
        CollisionLayer = defaultCollisionLayer;
        CollisionMask = defaultCollisionMask;
    }

    public void Interact(Node interactor)
    {
        if (!CanInteract || CustomBookScene == null) return;

        HidePrompt();
        var bookInstance = CustomBookScene.Instantiate<Control>();
        GetTree().Root.AddChild(bookInstance);
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
