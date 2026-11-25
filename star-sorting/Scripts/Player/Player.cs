using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[ExportCategory("References")]

    [Export]
    public Camera3D playerCamera;
    [Export]
    public InteractionRay playerInteractor;

    [Export]
    public Marker3D carryObjectMarker;
    

    [ExportCategory("Variables")]
    [Export]
    public float Speed { get; set; } = 3.0f;
    [Export]
    public float JumpVelocity { get; set; } = 4.5f;
    [Export]
	public  float mouseSensitivity { get; set; } = 0.002f;
	
	private IPickable heldItem = null;
	private IReadable activeReadableItem = null;
	public bool isInteracting = false;


    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
    public override void _PhysicsProcess(double delta)
    {
        if (isInteracting)
        {
            return;
        }
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

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            if (isInteracting)
            {
                return;
            }
            RotateY(-eventMouseMotion.Relative.X * mouseSensitivity);
            playerCamera.RotateX(-eventMouseMotion.Relative.Y * mouseSensitivity);
            Vector3 vec = new Vector3(Mathf.Clamp(playerCamera.Rotation.X, -Mathf.DegToRad(80), Mathf.DegToRad(80)), playerCamera.Rotation.Y, playerCamera.Rotation.Z);
            playerCamera.Rotation = vec;
        }
        if (@event.IsActionPressed("mouse_left_click"))
        {
            if (isInteracting)
            {
                return;

            }
            if (heldItem == null)
            {
                PickUpObject(playerInteractor.GetPickable());
            }
            else
            {
                DropHeldObject();
            }

        }
        if (@event.IsActionPressed("interact"))
        {
            if(isInteracting)
            {
              
                StopReading();
                 return;
                
            }
            if (heldItem == null)
            {
                playerInteractor.Interact(this);
               // GD.Print("tried to interact");
            }
            else
            {
                if (heldItem is IReadable readable)
                {
                    StartReading(readable);
                    GD.Print("tried to read what is in my hands");
                }
                else if (heldItem is IInteractable interactable)
                {
                    interactable.Interact(this);
                }

            }

        }
        if (@event.IsActionPressed("open_menu"))
        {
            if(isInteracting)
            {
                StopReading();
                return;
            }
        }

    }


    //-----------------------------
    //Starmap code
    public void StartOperatingStarmap()
    {
        isInteracting = true;

    }
    public void StopOperatingStarmap()
    {
        isInteracting = false;

    }


    //---------------------------
    //Reading code
    public void StartReading(IReadable readable)
    {
        activeReadableItem = readable;
        isInteracting = true;
        

        activeReadableItem.OpenReadingUI();
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public void StopReading()
    {
        activeReadableItem?.CloseReadingUI();
        activeReadableItem = null;
        isInteracting = false;

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
	//-------------------------
	//pickup code
    public void PickUpObject(IPickable item)
	{
		if (item == null)
		{
			return;
		}

        playerInteractor.Disable();
        item.OnPickUp(carryObjectMarker);
        heldItem = item;


    }
    public void DropHeldObject()
    {
        if (heldItem == null) return;

        heldItem.OnDrop();
        heldItem = null;
        playerInteractor.Enable();
    }
    //---------------
    



}
