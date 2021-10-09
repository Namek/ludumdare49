using System;
using Godot;

public class WaveIntroduceScreen : Panel {
  Tween tween;
  Timer disappearTimer;
  Label label;

  public override void _Ready() {
    tween = GetNode<Tween>("./Tween");
    disappearTimer = GetNode<Timer>("./disappearTimer");
    label = GetNode<Label>("./Label");
    Modulate = new Color(Modulate, 1f);
    Visible = false;
  }

  public void PresentWave(int num) {
    label.Text = $"Wave {num}";

    Visible = true;

    if (num > 1) {
      Modulate = new Color(Modulate, 0f);
      tween.InterpolateProperty(this, "modulate:a", 0f, 1.0f, 1f, Tween.TransitionType.Linear, Tween.EaseType.In);
      tween.Start();
    }

    disappearTimer.Start();
  }

  private void _on_disappearTimer_timeout() {
    tween.InterpolateProperty(this, "modulate:a", 1f, 0f, 1.0f, Tween.TransitionType.Linear, Tween.EaseType.Out);
    tween.Start();
  }
}
