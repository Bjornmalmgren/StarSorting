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
	[Export]
	public InteractionRay playerInteractor;

	[Export]
	public Marker3D carryObjectMarker;

	Node3D currentHeldObject=null;
	

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
	public void PickUpObject(Node3D item)
	{
		if(item!=null)
		{
            playerInteractor.Disable();
            item.Reparent(carryObjectMarker);
            item.GlobalPosition = carryObjectMarker.GlobalPosition;
            item.GlobalRotation = carryObjectMarker.GlobalRotation;
            currentHeldObject = item;
            var book = (Book)item;
            book.isHeld = true;
        }
		
		
	}
	public void DropHeldObject()
	{
		
		currentHeldObject.Reparent(GetTree().CurrentScene);
        var book = (Book)currentHeldObject;
        book.isHeld = false;
		currentHeldObject = null;

        playerInteractor.Enable();
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
		if(@event.IsActionPressed("Cycle"))
		{
			/*if(Input.MouseMode==Input.MouseModeEnum.Captured)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
			else
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}*/
			if (currentHeldObject == null)
			{
				PickUpObject(playerInteractor.PickUp());
			}
			else
			{
				DropHeldObject();
			}

		}

	}
	
}
