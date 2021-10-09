using System;
using Godot;

public class Dome : KinematicBody {
  Camera camera;

  // range: [0, 1]
  [Export] public float health = 1.0f;

  ShaderMaterial material;

  [Export] public float targetDissolve = 0f;

  public override void _Ready() {

    var domeArea = GetNode<Area>("Area");
    domeArea.Connect("body_entered", this, nameof(_onDomeArea_BodyEntered));

    material = GetNode<MeshInstance>("./mesh").GetActiveMaterial(0) as ShaderMaterial;
    material.SetShaderParam("dissolve", true);
    material.SetShaderParam("dissolve_level", 0f);
  }

  public override void _Process(float delta) {
    float newDissolveLevel = Mathf.Lerp((float)material.GetShaderParam("dissolve_level"), targetDissolve, delta);
    material.SetShaderParam("dissolve_level", newDissolveLevel);
  }

  private void _onDomeArea_BodyEntered(Node body) {
    GD.Print($"{body.Name} entered dome");

    if (body is Mine mine) {
      health -= C.MINE_DAMAGE;

      if (this.G().game.GameState == GameState.GoingOn) {
        targetDissolve = Mathf.Clamp(1.0f - Mathf.Clamp(health, 0, 1), 0, 1) / 2;
      }

      mine.Explode();

      Game.Globals.mainCamera.Shake(5f, 0.6f, 2f);
    }
  }

  public void Explode() {
    // TODO
  }
}