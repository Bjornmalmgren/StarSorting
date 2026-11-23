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

public struct Constellation
{
    public int StarsInConstellation;
    public Vec2[] CorrectPositions = new Vec2[5];

    public Constellation()
    {

    }
}

public partial class StarMapPrototype1 : Node3D
{

    public Constellation Constelaltion1;
    [Export]
    PackedScene StarScene;
    [Export]
    Array<Node3D> Stars;
    public int Index = 0;
    int CurrentHoveringStar = -1;
    bool CurrentlyHoveringStar = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        //instatiate the needed stars
        //connect their mouse entered and exited to a function to check if hovering and if it can be moved
        Constelaltion1.StarsInConstellation = 5;
        Constelaltion1.CorrectPositions = new Vec2[Constelaltion1.StarsInConstellation];
        Constelaltion1.CorrectPositions[0] = new Vec2(5, -3);
        Constelaltion1.CorrectPositions[1] = new Vec2(-5, 3);
        Constelaltion1.CorrectPositions[2] = new Vec2(2, 1);
        Constelaltion1.CorrectPositions[3] = new Vec2(-2, 1);
        Constelaltion1.CorrectPositions[4] = new Vec2(5, 1);

       


        CreateStars();
        
        
        (Stars[0] as StarPrototype).MouseEntered += MouseEnteredStar;
        (Stars[0] as StarPrototype).MouseExited += MouseExitedStar;
        
    }

    void CreateStars()
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Seed = 12441512;
        for (int i = 0; i < Constelaltion1.StarsInConstellation; i++) 
        {
            Stars.Add(StarScene.Instantiate() as Node3D);
            AddChild(Stars.Last<Node3D>());
            Index++;
            


            Godot.Vector3 Pos = new Godot.Vector3(rand.RandfRange(7,-7), rand.RandfRange(4, -4),-6);
            Stars.Last<Node3D>().Position = Pos;
            Stars.Last<Node3D>().Scale *= 0.5f;
            (Stars.Last<Node3D>() as StarPrototype).MouseEntered += MouseEnteredStar;
            (Stars.Last<Node3D>() as StarPrototype).MouseExited += MouseExitedStar;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
        
        base._PhysicsProcess(delta);
        if (Input.IsActionPressed("mouse_left_click") && CurrentlyHoveringStar)
        {

            var spaceState = GetWorld3D().DirectSpaceState;
            var cam = GetNode<Camera3D>("Camera3D");
            var mousePos = GetViewport().GetMousePosition();

            var origin = cam.ProjectRayOrigin(mousePos);
            var end = origin + cam.ProjectRayNormal(mousePos) * 1000;
            var query = PhysicsRayQueryParameters3D.Create(origin, end);
            query.CollideWithAreas = true;

            var result = spaceState.IntersectRay(query);
            var WorldPosition = result["position"].AsVector3();
            WorldPosition.Z = -6;
            Stars[CurrentHoveringStar].Position = WorldPosition;
        }
    }

    public void MouseEnteredStar(int index)
    {
        CurrentlyHoveringStar = true;
        CurrentHoveringStar = index;

       
    }

    public void MouseExitedStar(int index)
    {
        if(!Input.IsActionPressed("mouse_left_click"))
        {
            CurrentlyHoveringStar = false;
            CurrentHoveringStar = -1;
            CheckCorrectPositions(Constelaltion1);
        }
        //check if the stars are in the correct position
        
    }

    public void CheckCorrectPositions(Constellation Const)
    {
        int CorrectPositions = 0;
        foreach (var star in Stars)
        {
            
            foreach (var pos in Const.CorrectPositions)
            {
                if(star.Position.DistanceTo(new Vector3(pos.x, pos.y, -6)) < 0.2f)
                {
                    
                    CorrectPositions++;
                }
                
            }
            
        }
        if(CorrectPositions == Const.StarsInConstellation)
        {
            //constellation done lock the stars positions
            GD.Print("Constellation is done");
            foreach (var star in Stars)
            {

                foreach (var pos in Const.CorrectPositions)
                {
                    if (star.Position.DistanceTo(new Vector3(pos.x, pos.y, -6)) < 0.2f)
                    {
                        star.Position = new Vector3(pos.x, pos.y, -6);
                        star.SetMeta("_edit_lock_", true);
                    }
                    

                }

            }
        }
    }
}