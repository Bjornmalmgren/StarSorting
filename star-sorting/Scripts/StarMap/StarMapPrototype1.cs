using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Linq;
using static Godot.BaseButton;
//using System.Numerics;


public partial class StarMapPrototype1 : Node3D
{
    [ExportCategory("Refrences")]
    [Export]
    Grid StarMapGrid;
    [Export]
    Camera3D Camera;
    [Export]
    Label3D GridLabel;
    [Export]
    Label3D HoldingStarLabel;
    [ExportCategory("Variables")]
   
    int CurrentHoveringStar = -1;
    bool CurrentlyHoveringStar = false;
    [Export]
    Node3D CurrentlyHeldStar = null;
    RayCast3D RayCast;
    //
    public bool OperateStarMap = false;
    bool CurrentlyDrawing = false;
    int CurrentGrid = 44+7;
    float CameraSpeed = 0.03f;
    float CameraSpeedFaster = 0.03f;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        RayCast = new RayCast3D();
        AddChild(RayCast);


        
        Input.MouseMode = Input.MouseModeEnum.Captured;

        //checks if this is the correct start position
 
        CurrentGrid = (int)(StarMapGrid.GridSquares.Count() - 1) / 2 + (int)(StarMapGrid.scale.X)/2;
        //CurrentGrid = TemporaryGrid.GridPosition(CurrentGrid, new Vector2(GetNode<Camera3D>("Camera3D").GlobalPosition.X, GetNode<Camera3D>("Camera3D").GlobalPosition.Y));
        
        //sets the labels to show the current grid and star thats held
        GridLabel.Text = "Grid: " + StarMapGrid.GridSquares[CurrentGrid].Designation;
        HoldingStarLabel.Text = "";
       
        //sets camera position to middle of the starting grid
       Camera.GlobalPosition = new Vector3(StarMapGrid.GridSquares[CurrentGrid].BLCorner.X, StarMapGrid.GridSquares[CurrentGrid].BLCorner.Y, 0) + new Vector3(0.5f, 0.5f, 0);
    }

    

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        //move cam according to where mouse is might rework into a diffrent type of mouse movement
        if (!OperateStarMap) return;
        

        if (OperateStarMap)
        {
            if (Input.IsActionPressed("move_faster"))
            {
                CameraSpeedFaster = 0.03f;
            }

            if (Input.IsActionPressed("move_left"))
            {
                Camera.Position -= new Vector3(CameraSpeed + CameraSpeedFaster, 0, 0);
            }
            if (Input.IsActionPressed("move_forward"))
            {
                Camera.Position += new Vector3(0, CameraSpeed + CameraSpeedFaster, 0);
            }
            if (Input.IsActionPressed("move_right"))
            {
                Camera.Position += new Vector3(CameraSpeed + CameraSpeedFaster, 0, 0);
            }
            if (Input.IsActionPressed("move_backward"))
            {
                Camera.Position -= new Vector3(0, CameraSpeed + CameraSpeedFaster, 0);
            }

            CurrentGrid = StarMapGrid.GridPosition(CurrentGrid, new Vector2(Camera.GlobalPosition.X, Camera.GlobalPosition.Y));
            GridLabel.Text = "Grid: " + StarMapGrid.GridSquares[CurrentGrid].Designation;
        }
        CameraSpeedFaster = 0;

        if (Input.IsActionJustPressed("change_to_drawing_mode") && CurrentlyHeldStar == null)
        {
            CurrentlyDrawing = !CurrentlyDrawing;
            if(Input.MouseMode == Input.MouseModeEnum.Captured)
            {
                Input.MouseMode = Input.MouseModeEnum.Confined;
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }

        if (Input.IsActionJustPressed("mouse_left_click") && CurrentlyHeldStar == null && !CurrentlyDrawing)
        {
            
            RayCast.ForceRaycastUpdate();
            if (RayCast.IsColliding())
            {
                //GD.Print("picking up");
                Node3D Parent = ((RayCast.GetCollider() as Area3D).GetParent().GetParent() as Node3D);

                CurrentlyHeldStar = Parent;
                HoldingStarLabel.Text = "Held star: " + (Parent as StarPrototype).StarName;
            }
            
            
            
        }
        else if(Input.IsActionJustPressed("mouse_left_click") && CurrentlyHeldStar != null && !CurrentlyDrawing)
        {
            (CurrentlyHeldStar.GetParent() as Constellation).CheckCorrectPositions();
            CurrentlyHeldStar = null;
            HoldingStarLabel.Text = "";

        }
        if(CurrentlyHeldStar != null && !CurrentlyDrawing)
        {
            Vector3 NewPosition = Camera.GlobalPosition;
            NewPosition.Z = CurrentlyHeldStar.GlobalPosition.Z;
            CurrentlyHeldStar.GlobalPosition = NewPosition;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        
        base._PhysicsProcess(delta);

        var Pos1 = Camera.GetChild(0) as MeshInstance3D;
        var Pos2 = Camera.GetChild(1) as MeshInstance3D;
        RayCast.GlobalPosition = Pos1.GlobalPosition;
        RayCast.TargetPosition = new Vector3(0,0, Pos2.GlobalPosition.Z);
        RayCast.ExcludeParent = true;
        RayCast.CollideWithAreas = true;


    }

       
}