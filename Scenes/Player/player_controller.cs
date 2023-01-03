using Godot;


public partial class player_controller : CharacterBody3D
{

	[ExportCategory("Player Controller")]
	[ExportGroup("Movement Settings")]
	[Export] private float speed = 5.0f;
	[Export] private float acceleration = 10.0f;

	[ExportGroup("Camera Control Settings")]

	[Export] private PackedScene virtual_camera_scene;
	[Export] private float camera_max_angle_degrees = 70.0f;
	


	[ExportSubgroup("Mouse")]
	[Export] private float camera_sensitivity = 0.003f;
	[ExportSubgroup("Gamepad")]
	[Export] private float gamepad_look_speed = 10.0f; // degrees per second
	[Export] private float gamepad_min_look_strength = 0.25f; // degrees per second
	

	[ExportGroup("NodePaths")]
	[Export] private NodePath camera_pivot_path;
	private Node3D camera_pivot;

	[Export] private NodePath third_person_cam_root_path;
	private Node3D third_person_cam_root;

    public override void _Ready()
    {
		camera_pivot = GetNodeOrNull<Node3D>(camera_pivot_path);
		// Helper class to allow for GDScript style assertions in C#
		GDAssertion.Assert(this, camera_pivot != null, $"Failed to load camera pivot node from node path {camera_pivot_path}");

		third_person_cam_root = GetNodeOrNull<Node3D>(third_person_cam_root_path);
    }

    public override void _PhysicsProcess(double delta)
    {
		// Apply Look
		var look_vec = Input.GetVector("look_right", "look_left", "look_down", "look_up", gamepad_min_look_strength);
		_DoLookMotion(look_vec * gamepad_look_speed * (float)delta);

		// Apply motion
		var input_vec = Input.GetVector("move_left", "move_right", "move_back", "move_forwards");
		Vector3 direction = new();

		// move in relative direction to the camera look position (we move the pivot as if it were the camera)
		direction += camera_pivot.GlobalTransform.basis.z * input_vec.y * -1.0f;
		direction += camera_pivot.GlobalTransform.basis.x * input_vec.x;
		direction.y = 0;
		direction = direction.Normalized();

		Velocity = Velocity.Lerp(direction * speed, acceleration * (float)delta);

		MoveAndSlide();
    }

	private static Vector2 MOUSE_LOOK_VEC_FLIP = new Vector2(-1.0f, -1.0f);
    public override void _UnhandledInput(InputEvent e)
    {
		if (Input.MouseMode == Input.MouseModeEnum.Captured && e is InputEventMouseMotion)
		{
			_DoLookMotion((e as InputEventMouseMotion).Relative * camera_sensitivity * MOUSE_LOOK_VEC_FLIP);
		}
		if (e.IsActionPressed("toggle_mouse_lock"))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		}
		if (e.IsActionPressed("toggle_third_person"))
		{
			if (third_person_cam_root.GetChildCount() > 0)
			{
				foreach(Node n in third_person_cam_root.GetChildren())
				{
					n.QueueFree();
				}
			} else {
				var vcam = virtual_camera_scene.Instantiate<VirtualCamera>();
				third_person_cam_root.AddChild(vcam);
			}
		}
    }

	private void _DoLookMotion(Vector2 delta)
	{
		RotateY(delta.x);

		var temp_rot = camera_pivot.Rotation;
		temp_rot.x += delta.y;
		float max_radians = Mathf.DegToRad(camera_max_angle_degrees);
		temp_rot.x = Mathf.Clamp(temp_rot.x, -max_radians, max_radians);
		camera_pivot.Rotation = temp_rot;
	}

}
