using Godot;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

[Tool]
[GlobalClass]
public partial class Book : RigidBody3D, IInteractable, IPickable, IReadable
{

    [ExportCategory("BookData")]
    [Export]
    public BookData data;
    [ExportCategory("References")]
    [Export] private CollisionShape3D physicsCollision; 
    [Export] private CollisionShape3D areaCollision;

    [Export] private PackedScene bookReaderScene;

    [Export] private Node3D modelPivot;

    [Export]
    private Label3D interactPrompt;

    [ExportCategory("Parameters")]
    [Export]
    public Vector3 modelRotationDegrees { get; set; } = Vector3.Zero;

    public bool canInteract { get; set; } = true;
    public bool CanPickUp { get; set; } = true;
    

    public string promptMessage => $"{data.Title}";
    [ExportToolButton("Refresh Book Visuals")]
    public Callable RefreshVisualsButton => Callable.From(UpdateBookVisuals);

    private BookReader activeBookUI; // to be replaced with bookreader

    private uint defaultCollisionLayer;
    private uint defaultCollisionMask;


    //-------------------------------------------------------------------------------
    //Book specific implementation
    public override void _Ready()
    {
        
        defaultCollisionLayer = CollisionLayer;
        defaultCollisionMask = CollisionMask;
        
        if (data != null)
        {
            UpdateBookVisuals();
        }
    }
    private void UpdateBookVisuals()
    {
        //if node is not ready
        if (modelPivot == null)
        {
            return;
        }
        foreach (Node child in modelPivot.GetChildren())
        {
            child.QueueFree();
        }
        if (data == null)
        {
            return;
        }
        if (!string.IsNullOrEmpty(data.Title))
            Name = data.Title;

        // 2. Spawn the new visual model
        if (data.VisualModelScene != null)
        {
            Node3D model = data.VisualModelScene.Instantiate<Node3D>();
            modelPivot.AddChild(model);
            modelPivot.RotationDegrees =modelRotationDegrees;
            FitCollisionToModel(model);
        }
        else if (data.BookMesh != null)
        {
            MeshInstance3D meshInstance = new MeshInstance3D { Mesh = data.BookMesh };
            modelPivot.AddChild(meshInstance);
            modelPivot.RotationDegrees = modelRotationDegrees;
            FitCollisionToModel(meshInstance);


        }

    }
    private void FitCollisionToModel(Node modelRoot)
    {
        // Ensure exports are resolved even when running in [Tool] mode before _Ready()
        physicsCollision ??= GetNodeOrNull<CollisionShape3D>("CollisionShape3D");
        areaCollision ??= GetNodeOrNull<CollisionShape3D>("Area3D/CollisionShape3D");

        Aabb combinedAabb = new Aabb();
        bool hasAabb = false;

        // Pass modelPivot.Transform so modelPivot's rotation & scale are included!
        CollectMeshAabbs(modelRoot, modelPivot.Transform, ref combinedAabb, ref hasAabb);

        if (!hasAabb) return;

        // Reset collision rotation to Identity because combinedAabb is already world/root-aligned
        BoxShape3D box = new BoxShape3D { Size = combinedAabb.Size };
        Vector3 centerOffset = combinedAabb.GetCenter();

        if (physicsCollision != null)
        {
            physicsCollision.Shape = box;
            physicsCollision.Transform = new Transform3D(Basis.Identity, centerOffset);
        }

        if (areaCollision != null)
        {
            areaCollision.Shape = new BoxShape3D { Size = combinedAabb.Size * 1.5f };
            // Convert from Book root space to Area3D local space in case Area3D isn't at (0,0,0)
            if (areaCollision.GetParent() is Node3D areaNode)
                areaCollision.Position = areaNode.ToLocal(ToGlobal(centerOffset));
            else
                areaCollision.Position = centerOffset;
        }
    }
    private void CollectMeshAabbs(Node current, Transform3D parentTransform, ref Aabb totalAabb, ref bool hasAabb)
    {
        Transform3D currentTransform = parentTransform;
        if (current is Node3D node3D)
        {
            currentTransform = parentTransform * node3D.Transform;
        }

        if (current is MeshInstance3D meshInstance && meshInstance.Mesh != null)
        {
            // Transform the mesh's local AABB into _modelPivot space
            Aabb meshAabb = currentTransform * meshInstance.GetAabb();
            totalAabb = hasAabb ? totalAabb.Merge(meshAabb) : meshAabb;
            hasAabb = true;
        }

        foreach (Node child in current.GetChildren())
        {
            CollectMeshAabbs(child, currentTransform, ref totalAabb, ref hasAabb);
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
        if (activeBookUI != null || bookReaderScene==null|| data == null)
        {
            return;
        }
        activeBookUI = bookReaderScene.Instantiate<BookReader>(); //replace canvaslayer with bookreader later
        GetTree().Root.AddChild(activeBookUI);
        activeBookUI.Initialize(data);
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
