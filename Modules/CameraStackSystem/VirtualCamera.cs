using Godot;
using System;

public partial class VirtualCamera : Marker3D
{

    public override void _EnterTree()
    {
		CameraBrain.AddCamera(this);
    }

    public override void _ExitTree()
    {
		CameraBrain.RemoveCamera();
    }
}
