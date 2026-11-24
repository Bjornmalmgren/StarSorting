using Godot;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

[GlobalClass]
public partial class Book : RigidBody3D, IInteractable, IPickable, IReadable
{
    [Export]
    public BookData Data;
    [Export]
    private Node3D modelPivot;
    [Export] private PackedScene bookReaderScene;

    [Export] 
    public bool canInteract { get; set; } = true;
    [Export] public bool CanPickUp { get; set; } = true;
    

    public string promptMessage => $"{Data.Title}";

    [Export]
    private Label3D interactPrompt;

    private BookReader activeBookUI; // to be replaced with bookreader

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
            if (modelPivot != null && Data.VisualModelScene != null)
            {

                Node3D model = Data.VisualModelScene.Instantiate<Node3D>();
                modelPivot.AddChild(model);
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
        player.StartReading(this);
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
        if (activeBookUI != null || bookReaderScene==null|| Data == null)
        {
            return;
        }
        activeBookUI = bookReaderScene.Instantiate<BookReader>(); //replace canvaslayer with bookreader later
        GetTree().Root.AddChild(activeBookUI);
        activeBookUI.Initialize(Data);
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
