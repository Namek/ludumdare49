using System;
using Godot;

public class GameOverScreen : Panel {
  public override void _Ready() {

  }

  public new void Show() {
    base.Show();
    this.Modulate = new Color(Modulate, 0f);
    var tween = GetNode<Tween>("./Tween");
    tween.InterpolateProperty(this, "modulate:a", 0f, 1f, 1f);
    tween.Start();
  }
}