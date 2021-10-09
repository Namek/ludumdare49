using System;
using Godot;

public class CameraFollow : ClippedCamera {
  [Export] public float Distance = 6.3f;
  [Export] public float Height = 6f;

  public override void _Ready() {
    SetPhysicsProcess(true);
    SetAsToplevel(true);
  }

  public override void _PhysicsProcess(float delta) {
    // var target = GetNode<Spatial>("../camera_target").GlobalTransform.origin;
    // var pos = GlobalTransform.origin;
    // var up = new Vector3(0, 1, 0);

    // var offset = pos - target;

    // offset = offset.Normalized() * Distance;
    // offset.y = Height;

    // pos = target + offset;
    // LookAtFromPosition(pos, target, up);
  }

  //  // Called every frame. 'delta' is the elapsed time since the previous frame.
  //  public override void _Process(float delta)
  //  {
  //      
  //  }
}
