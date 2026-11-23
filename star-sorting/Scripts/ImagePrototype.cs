using Godot;
using Godot.Collections;
using System;

public partial class ImagePrototype : Node2D
{
	Sprite2D Sprite;
	[Export]
	Array<CompressedTexture2D> Textures;
	int CurrentImage = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Sprite = GetNode<Sprite2D>("Sprite2D");
		Sprite.Texture = Textures[CurrentImage];

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("Cycle Images"))
		{
			CurrentImage++; 
            Sprite.Texture = Textures[CurrentImage];
            if (CurrentImage == Textures.Count-1)
            {
                CurrentImage = -1;
            }
        }
	}
}
