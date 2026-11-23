using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export]
    public float Speed { get; set; }  = 5.0f;
    [Export]
    public float JumpVelocity { get; set; } = 4.5f;

	[Export]
	public  float mouseSensitivity { get; set; } = 0.002f;
	[Export]
	public Camera3D playerCamera;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}
	public override void _Input(InputEvent @event)
	{
		 if (@event is InputEventMouseMotion eventMouseMotion)
		{
			RotateY(-eventMouseMotion.Relative.X*mouseSensitivity);
			playerCamera.RotateX(-eventMouseMotion.Relative.Y * mouseSensitivity);
			Vector3 vec = new Vector3(Mathf.Clamp(playerCamera.Rotation.X, -Mathf.DegToRad(80), Mathf.DegToRad(80)),playerCamera.Rotation.Y,playerCamera.Rotation.Z);
			playerCamera.Rotation=vec;
		}
		/*if(@event.IsActionPressed("open_menu"))
		{
			if(Input.MouseMode==Input.MouseModeEnum.Captured)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
			else
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}

		}*/
	}
	
}
