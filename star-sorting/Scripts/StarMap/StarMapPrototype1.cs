using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Linq;
//using System.Numerics;

//had to create a custom array as i didnt wanna work with Vector2
public struct Vec2
{
    public float x;
    public float y;

    public Vec2(float X, float Y)
    {
        x = X; y = Y;
    }
}



public partial class StarMapPrototype1 : Node3D
{   
    public int Index = 0;
    int CurrentHoveringStar = -1;
    bool CurrentlyHoveringStar = false;
    [Export]
    Node3D CurrentlyHeldStar = null;
    RayCast3D RayCast;
    //
    bool MouseControlls = false;
    int CurrentGrid = 165;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        RayCast = new RayCast3D();
        AddChild(RayCast);


        
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Grid TemporaryGrid = (GetNode<MeshInstance3D>("/root/StarMapPrototype1/StarMapGrid") as Grid);
        CurrentGrid = TemporaryGrid.GridSquares.Length / 2 +11;
        CurrentGrid = TemporaryGrid.GridPosition(CurrentGrid, new Vector2(GetNode<Camera3D>("Camera3D").GlobalPosition.X, GetNode<Camera3D>("Camera3D").GlobalPosition.Y));
        GetNode<Label3D>("/root/StarMapPrototype1/Camera3D/Label3D").Text = "Grid: " + TemporaryGrid.GridSquares[CurrentGrid].Designation;

       
        GetNode<Camera3D>("Camera3D").GlobalPosition = new Vector3(TemporaryGrid.GridSquares[CurrentGrid].TLCorner.X, TemporaryGrid.GridSquares[CurrentGrid].TLCorner.Y, 0) + new Vector3(0.5f, 0.5f, 0);
    }

    

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        //move cam according to where mouse is
        if (MouseControlls)
        {
            Vector2 MousePos = GetViewport().GetMousePosition();
            Rect2 View = GetViewport().GetVisibleRect();
            Vector2 Center = View.GetCenter();

            if (MousePos.X > Center.X)
            {
                GetNode<Camera3D>("Camera3D").Position += new Vector3(0.02f, 0, 0);
            }
            if (MousePos.X < Center.X)
            {
                GetNode<Camera3D>("Camera3D").Position -= new Vector3(0.02f, 0, 0);
            }
            if (MousePos.Y > Center.Y)
            {
                GetNode<Camera3D>("Camera3D").Position -= new Vector3(0, 0.02f, 0);
            }
            if (MousePos.Y < Center.Y)
            {
                GetNode<Camera3D>("Camera3D").Position += new Vector3(0, 0.02f, 0);
            }
        }

        if (!MouseControlls)
        {
            if (Input.IsActionPressed("move_left"))
            {
                GetNode<Camera3D>("Camera3D").Position -= new Vector3(0.02f, 0, 0);
            }
            if (Input.IsActionPressed("move_forward"))
            {
                GetNode<Camera3D>("Camera3D").Position += new Vector3(0, 0.02f, 0);
            }
            if (Input.IsActionPressed("move_right"))
            {
                GetNode<Camera3D>("Camera3D").Position += new Vector3(0.02f, 0, 0);
            }
            if (Input.IsActionPressed("move_backward"))
            {
                GetNode<Camera3D>("Camera3D").Position -= new Vector3(0, 0.02f, 0);
            }

            Grid TemporaryGrid = (GetNode<MeshInstance3D>("/root/StarMapPrototype1/StarMapGrid") as Grid);
            CurrentGrid = TemporaryGrid.GridPosition(CurrentGrid, new Vector2(GetNode<Camera3D>("Camera3D").GlobalPosition.X, GetNode<Camera3D>("Camera3D").GlobalPosition.Y));
            GetNode<Label3D>("/root/StarMapPrototype1/Camera3D/Label3D").Text = "Grid: " + TemporaryGrid.GridSquares[CurrentGrid].Designation;
        }

        if (Input.IsActionJustPressed("mouse_left_click") && CurrentlyHeldStar == null)
        {
            
            RayCast.ForceRaycastUpdate();
            if (RayCast.IsColliding())
            {
                GD.Print("picking up");
                Node3D Parent = ((RayCast.GetCollider() as Area3D).GetParent().GetParent() as Node3D);

                CurrentlyHeldStar = Parent;
            }
            
            
            
        }
        else if(Input.IsActionJustPressed("mouse_left_click") && CurrentlyHeldStar != null)
        {
            (CurrentlyHeldStar.GetParent() as Constellation).CheckCorrectPositions(CurrentlyHeldStar);
            CurrentlyHeldStar = null;

        }
        if(CurrentlyHeldStar != null)
        {
            Vector3 NewPosition = GetNode<Camera3D>("Camera3D").GlobalPosition;
            NewPosition.Z = (CurrentlyHeldStar.GetParent() as Constellation).StarDistanceFromCamera;
            CurrentlyHeldStar.GlobalPosition = NewPosition;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        
        base._PhysicsProcess(delta);

        var Pos1 = GetNode<Camera3D>("Camera3D").GetChild(0) as MeshInstance3D;
        var Pos2 = GetNode<Camera3D>("Camera3D").GetChild(1) as MeshInstance3D;
        RayCast.GlobalPosition = Pos1.GlobalPosition;
        RayCast.TargetPosition = new Vector3(0,0, Pos2.GlobalPosition.Z);
        RayCast.ExcludeParent = true;
        RayCast.CollideWithAreas = true;


    }

       
}