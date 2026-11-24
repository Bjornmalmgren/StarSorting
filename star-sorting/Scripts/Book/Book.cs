using Godot;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

[GlobalClass]
public partial class Book : RigidBody3D, IInteractable, IPickable, IReadable
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

    private Control activeBookUI;

    private uint defaultCollisionLayer;
    private uint defaultCollisionMask;


    //-------------------------------------------------------------------------------
    //Book specific implementation
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
    //------------------------------------------------------------------------------------
    //IPickable implementation
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
    //--------------------------------------------------------------------------------------
    //IInteractable implementation
    public void Interact(Player player)
    {
        if (!canInteract || Data.ReadingContentScene == null)
        {
            return;
        }
        
        HidePrompt();
      
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
    //----------------------------------------------------------------------------------------
    //IReadable Implementation
    public void OpenReadingUI()
    {
        if (activeBookUI != null || Data?.ReadingContentScene == null)
        {
            return;
        }
        activeBookUI = Data.ReadingContentScene.Instantiate<Control>();
        GetTree().Root.AddChild(activeBookUI);
    }
    public void CloseReadingUI()
    {
        if (activeBookUI != null)
        {
            activeBookUI.QueueFree();
            activeBookUI = null;
        }
    }
}
