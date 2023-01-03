using Godot;
using System;
using System.Collections.Generic;

public partial class CameraBrain : Camera3D
{
	private static CameraBrain instance = null;
    public override void _Ready()
    {
		if(!GDAssertion.Assert(this, instance == null, "Multiple CameraBrain nodes were loaded into the scene tree at the same time! Only one should ever be present at any time!!!")) return;
		instance = this;
    }

    private static Stack<VirtualCamera> vcam_stack = new();
	private static VirtualCamera vcam_cache = null;

	public static void AddCamera(VirtualCamera camera)
	{
		vcam_stack.Push(camera);
	}

	public static void RemoveCamera()
	{
		vcam_stack.Pop();
	}

    public override void _Process(double delta)
    {
		if(vcam_stack.TryPeek(out vcam_cache))
		{
			GlobalTransform = vcam_cache.GlobalTransform;
		}
    }
    public override void _PhysicsProcess(double delta)
    {
		if(vcam_stack.TryPeek(out vcam_cache))
		{
			GlobalTransform = vcam_cache.GlobalTransform;
		}
    }





}
