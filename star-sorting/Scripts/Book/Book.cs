using Godot;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

[GlobalClass]
public partial class Book : RigidBody3D, IInteractable, IPickable
{
    [Export]
    public BookData Data;
    [Export] 
    private MeshInstance3D coverMesh;

  

    [Export] 
    public bool canInteract { get; set; } = true;
    [Export] public bool CanPickUp { get; set; } = true;
    

    public string promptMessage => $"{Data.Title}";

    [Export]
    private Label3D interactPrompt;

    private uint defaultCollisionLayer;
    private uint defaultCollisionMask;

    public override void _Ready()
    {
        
        defaultCollisionLayer = CollisionLayer;
        defaultCollisionMask = CollisionMask;
        if (Data != null)
        {
            Name = Data.Title;
            if (coverMesh != null && Data.CoverMaterial != null)
            {
                
                coverMesh.MaterialOverride = Data.CoverMaterial;
            }
        }
    }
    public void OnPickUp(Node3D holder)
    {
        Freeze = true;
        CollisionLayer = 0; 
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
        if (!canInteract || Data.ReadingContentScene == null) return;

        HidePrompt();
        var bookInstance = Data.ReadingContentScene.Instantiate<Control>();
        GetTree().Root.AddChild(bookInstance);
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
