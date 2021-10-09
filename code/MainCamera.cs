using System;
using Godot;

public class MainCamera : Camera {
  Tween shakeTween;
  Timer shakeTimer;

  private float shakeAmount;
  private Vector3 shakeOffset = Vector3.Zero;

  public override void _Ready() {
    shakeTween = GetNode<Tween>("./shakeTween");
    shakeTimer = GetNode<Timer>("./shakeTimer");

    // SetProcess(false);

    shakeTimer.Connect("timeout", this, nameof(_on_shakeTimer_timeout));
  }


  public override void _Process(float delta) {
    shakeOffset = new Vector3(
      (float)GD.RandRange(-shakeAmount, shakeAmount),
      (float)GD.RandRange(-shakeAmount, shakeAmount),
      (float)GD.RandRange(-shakeAmount, shakeAmount)) * delta;

    Game.Globals.mainCameraOffset.Translation = shakeOffset;
  }

  public void Shake(float addAmount, float time, float limit) {
    shakeAmount += addAmount;
    if (shakeAmount > limit) {
      shakeAmount = limit;
    }
    shakeTimer.WaitTime = time;
    shakeTween.StopAll();
    // SetProcess(true);
    shakeTimer.Start();
  }


  private void _on_shakeTimer_timeout() {
    shakeAmount = 0;
    // SetProcess(false);

    shakeTween.InterpolateProperty(this, nameof(shakeOffset), shakeOffset, Vector3.Zero, 0.1f, Tween.TransitionType.Quad, Tween.EaseType.InOut);
    shakeTween.Start();
  }
}
