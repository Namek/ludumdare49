using System;
using Godot;

public class Bullet : Area {
  [Export] public Vector3 Direction;
  public override void _Ready() {
    SetProcess(false);
  }
}
