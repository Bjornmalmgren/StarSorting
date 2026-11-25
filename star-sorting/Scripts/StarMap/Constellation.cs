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
    public int StarDistanceFromCamera = -10;
    public override void _Ready()
	{
        //CorrectPositions = new Vector2[StarsInConstellation];
        //CorrectStars = new string[StarsInConstellation];
        CreateStars();
        GridPositions = new string[StarsInConstellation];
        var grid = (GetNode<MeshInstance3D>("/root/StarMapPrototype1/StarMapGrid") as Grid);
        GridPositions = (GetNode<MeshInstance3D>("/root/StarMapPrototype1/StarMapGrid") as Grid).PostionsToGrid(CorrectPositions);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    void CreateStars()
    {
        PackedScene StarScene = GD.Load<PackedScene>(StarPath);
        RandomNumberGenerator rand = new RandomNumberGenerator();
        for (int i = 0; i < StarsInConstellation; i++)
        {
            
            Stars.Add(StarScene.Instantiate() as Node3D);
            AddChild(Stars.Last<Node3D>());

            Godot.Vector3 Pos = new Godot.Vector3(rand.RandfRange(0, 22), rand.RandfRange(0, 14), StarDistanceFromCamera);
            Stars.Last<Node3D>().GlobalPosition = Pos;
            Stars.Last<Node3D>().Scale *= 0.5f;
            (Stars.Last<Node3D>() as StarPrototype).StarName = CorrectStars[i];
        }
    }


    public void CheckCorrectPositions(Node3D Star)
    {

        //constellation done lock the stars positions
        int StarsDone = 0;
        for (int i = 0; i < StarsInConstellation; i++)
        {

            if (Stars[i].GlobalPosition.DistanceTo(new Vector3(CorrectPositions[i].X, CorrectPositions[i].Y, -10)) < 0.2f)
            {
                StarsDone++;
            }
        }
        if(StarsDone == StarsInConstellation)
        {
            for (int i = 0; i < StarsInConstellation; i++)
            {

                if (Stars[i].GlobalPosition.DistanceTo(new Vector3(CorrectPositions[i].X, CorrectPositions[i].Y, -10)) < 0.2f)
                {
                    //GD.Print("Constellation is done");
                    Stars[i].GlobalPosition = new Vector3(CorrectPositions[i].X, CorrectPositions[i].Y, StarDistanceFromCamera);
                    (Stars[i].GetChild(0).GetChild(0).GetChild(0) as CollisionShape3D).Disabled = true;
                }
            }
        }
        
        
        
    }
}
