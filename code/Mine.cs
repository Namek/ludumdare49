using System;
using Godot;

public class Mine : KinematicBody {
  [Export] public float speed = 1.0f;
  [Export] public bool stopped = false;
  [Export] public bool highlighted = false;

  // range: [0,1]
  [Export] public float health = 1.0f;

  float frozenTint = 0f;

  Camera debugCamera;
  MeshInstance meshInstance;
  CPUParticles explosion;

  public override void _Ready() {
    var anim = GetNode<AnimationPlayer>("./mesh/AnimationPlayer");
    anim.PlaybackSpeed = (float)GD.RandRange(0.10f, 0.2f);
    anim.Advance((float)GD.RandRange(0.0f, 2.5f));

    speed = (float)GD.RandRange(C.MINE_SPEED_FACTOR_MIN, C.MINE_SPEED_FACTOR_MAX);

    debugCamera = GetNode<Camera>("./Camera");
    explosion = GetNode<CPUParticles>("./explosionParticle");
  }


  public override void _Process(float delta) {
    if (debugCamera.Current) {
      if (Input.IsActionPressed(C.Action.BottomAttack)) {
        AddFreeze(delta * 2);
      }
    }

    if (health <= 0) {
      Explode();
      return;
    }

    if (!stopped) {
      // get health back if not being frozen until destroyal
      Damage(-delta);
    }

    if (GlobalTransform.origin.x >= -2) {
      // it goes near to city but won't clash it anymore
      QueueFree();
    }


    meshInstance = GetNode<MeshInstance>("./mesh/mergedBlocks(Clone)");
    var shaderMaterial = meshInstance.GetActiveMaterial(0) as ShaderMaterial;
    shaderMaterial.SetShaderParam("tint_percent", frozenTint);
    shaderMaterial.SetShaderParam("highlight", highlighted);
  }

  public override void _PhysicsProcess(float delta) {
    if (!stopped) {
      MoveAndCollide(Vector3.Right * delta * C.MINE_BASE_SPEED * (!stopped ? speed : 0));
    }
  }

  public void Damage(float delta) {
    health = Mathf.Clamp(health - C.HELI_TO_MINE_DAMAGE_PER_SECOND * delta, 0f, 1f);
    AddFreeze(delta);
  }

  private void AddFreeze(float delta) {
    frozenTint = Mathf.Clamp(frozenTint + C.MINE_FROZEN_TINT_GROW_SPEED * delta, 0f, 1f);
  }

  public void Explode() {
    Game.Globals.mainCamera.Shake(0.2f, 0.3f, 2f);
    explosion.Emitting = true;
    var timer = new Timer();
    AddChild(timer);
    timer.WaitTime = 0.2f;
    timer.Connect("timeout", this, nameof(_on_Explode_timeout));
    timer.Start();
  }

  private void _on_Explode_timeout() {
    QueueFree();
  }
}
