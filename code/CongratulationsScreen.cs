using System;
using Godot;

public class CongratulationsScreen : Panel {
  Tween tween;
  public override void _Ready() {
    Visible = false;

    tween = GetNode<Tween>("./Tween");
  }

  public new void Show() {
    base.Show();

    Modulate = new Color(Modulate, 0f);
    tween.InterpolateProperty(this, "modulate:a", 0f, 1.0f, 1.0f, Tween.TransitionType.Linear, Tween.EaseType.In);
    tween.Start();
  }

}