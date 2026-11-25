using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Constellation : Node3D
{
    // Called when the node enters the scene tree for the first time.

    string StarPath = "res://Scenes/StarMapPrototype/Star.tscn";

    [Export]
    int StarsInConstellation;
    [Export]
    Vector2[] CorrectPositions;
    [Export]
    string[] GridPositions;
    [Export]
    string[] CorrectStars;
    [Export]
    Array<Node3D> Stars;
    public int StarDistanceFromCamera = 10;

    int RandomXFloor = 0;
    int RandomXCeiling = 88;

    int RandomYFloor = 0;
    int RandomYCeiling = 56;

    float StarPlacementLeniancy = 0.2f;

    float StarScale = 0.5f; 
    public override void _Ready()
	{
        //CorrectPositions = new Vector2[StarsInConstellation];
        //CorrectStars = new string[StarsInConstellation];
        CreateStars();
        GridPositions = new string[StarsInConstellation];
        var grid = (GetParent().GetNode<MeshInstance3D>("StarMapGrid") as Grid);
        GridPositions = (GetParent().GetNode<MeshInstance3D>("StarMapGrid") as Grid).PostionsToGrid(CorrectPositions);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    void CreateStars()
    {
        PackedScene StarScene = GD.Load<PackedScene>(StarPath);
        RandomNumberGenerator rand = new RandomNumberGenerator();
        Camera3D Cam = GetParent().GetNode<Camera3D>("Camera3D");
        for (int i = 0; i < StarsInConstellation; i++)
        {
            
            Stars.Add(StarScene.Instantiate() as Node3D);
            AddChild(Stars.Last<Node3D>());

            Godot.Vector3 Pos = new Godot.Vector3(rand.RandfRange(RandomXFloor, RandomXCeiling), rand.RandfRange(RandomYFloor, RandomYCeiling), Cam.GlobalPosition.Z - StarDistanceFromCamera);
            Stars.Last<Node3D>().GlobalPosition = Pos;
            Stars.Last<Node3D>().Scale *= StarScale;
            (Stars.Last<Node3D>() as StarPrototype).StarName = CorrectStars[i];
        }
    }


    public void CheckCorrectPositions(Node3D Star)
    {

        //constellation done lock the stars positions
        int StarsDone = 0;
        for (int i = 0; i < StarsInConstellation; i++)
        {

            if (Stars[i].GlobalPosition.DistanceTo(new Vector3(CorrectPositions[i].X, CorrectPositions[i].Y, Stars[i].GlobalPosition.Z)) < StarPlacementLeniancy)
            {
                StarsDone++;
            }
        }
        if(StarsDone == StarsInConstellation)
        {
            for (int i = 0; i < StarsInConstellation; i++)
            {

                if (Stars[i].GlobalPosition.DistanceTo(new Vector3(CorrectPositions[i].X, CorrectPositions[i].Y, Stars[i].GlobalPosition.Z)) < StarPlacementLeniancy)
                {
                    //GD.Print("Constellation is done");
                    Stars[i].GlobalPosition = new Vector3(CorrectPositions[i].X, CorrectPositions[i].Y, StarDistanceFromCamera);
                    (Stars[i].GetChild(0).GetChild(0).GetChild(0) as CollisionShape3D).Disabled = true;
                }
            }
        }
        
        
        
    }
}
