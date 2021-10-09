using System;
using System.Collections.Generic;
using Godot;

public enum GameState {
  BeforeStart,
  GoingOn,
  WonWave,
  Lost,
  WonAllWaves
}

public class Game : Node {
  public static Globals Globals;

  PackedScene bulletPrefab, minePrefab;


  Node minesParent;

  Spatial cityWithDome;
  AnimationPlayer cityWithDomeSinkAnimation;
  Dome dome;


  [Export] public GameState GameState = GameState.BeforeStart;
  WaveIntroduceScreen waveIntroduceScreen;
  CongratulationsScreen congratulationsScreen;
  GameOverScreen gameOverScreen;

  List<Wave> wavesLeft = new List<Wave>();
  int waveNumber = 0;

  Wave currentWave = null;


  public override void _Ready() {

    Globals = this.G();
    Globals.game = this;
    Globals.mainCameraOffset = (Spatial)FindNode("#CameraOffset");
    Globals.mainCamera = (MainCamera)FindNode("#Camera");

    wavesLeft.AddRange(new Wave[] {
      // new Wave0(),
      new Wave1(),
      new Wave2(),
      new Wave3(),
      new Wave4()
    });
    foreach (var wave in wavesLeft) {
      wave.game = this;
    }

    bulletPrefab = GD.Load<PackedScene>(Res.Prefab.Bullet);
    minePrefab = GD.Load<PackedScene>(Res.Prefab.Mine);
    minesParent = FindNode("mines");
    foreach (var child in minesParent.GetChildren()) {
      (child as Node).QueueFree();
    }

    cityWithDome = (Spatial)FindNode("#city_with_dome");
    cityWithDomeSinkAnimation = cityWithDome.GetNode<AnimationPlayer>("./sink");
    dome = (Dome)FindNode("#dome");

    waveIntroduceScreen = (WaveIntroduceScreen)FindNode("#WaveIntroduceScreen");
    congratulationsScreen = (CongratulationsScreen)FindNode("#CongratulationsScreen");
    gameOverScreen = (GameOverScreen)FindNode("#GameOverScreen");

    GoNextWave();
    // ShowGameOver();//debug only
  }

  public void SpawnMine(Vector3 pos) {
    var mine = minePrefab.Instance<Mine>();
    mine.Translation = pos;
    minesParent.AddChild(mine);
  }

  public void SpawnBullet(Vector3 origin, Vector3 dir) {
    var b = bulletPrefab.Instance<Bullet>();
    AddChild(b);
    b.LookAt(dir, Vector3.Up);
    b.Translation = origin;
    b.Direction = dir;
  }

  public override void _Input(InputEvent evt) {
    if (evt.IsActionPressed("ui_cancel")) {
      GetTree().Quit(0);
      return;
    }
  }

  public override void _Process(float delta) {
    if (GameState == GameState.GoingOn) {
      if (currentWave != null) {

        bool isWaveFinished = currentWave.process(delta);

        if (isWaveFinished) {
          GoNextWave();
        }
      }

      if (dome.health <= 0) {
        dome.Explode();
        ShowGameOver();
      }
    }
  }

  public override void _PhysicsProcess(float delta) {
    float boundarySize = C.BULLET_LIFE_BOUNDARY_SIZE;
    Rect2 bulletBoundary = new Rect2(-boundarySize / 2, -boundarySize / 2, boundarySize, boundarySize);

    foreach (Bullet b in GetTree().GetNodesInGroup(C.Groups.Bullets)) {
      if (!Godot.Object.IsInstanceValid(b)) {
        continue;
      }

      b.Translation += delta * C.BULLET_SPEED * b.Direction;
      var bulletPoint2d = new Vector2(b.GlobalTransform.origin.x, b.GlobalTransform.origin.z);
      if (!bulletBoundary.HasPoint(bulletPoint2d)) {
        // kill bullet if it's too far away
        b.QueueFree();
      }
    }
  }


  private void GoNextWave() {
    if (wavesLeft.Count == 0) {
      FinishGame();
      return;
    }
    GameState = GameState.GoingOn;

    waveNumber += 1;
    currentWave = wavesLeft[0];
    wavesLeft.RemoveAt(0);
    waveIntroduceScreen.PresentWave(waveNumber);
  }

  private void FinishGame() {
    GameState = GameState.WonAllWaves;
    dome.targetDissolve = 0f;
    congratulationsScreen.Show();
  }

  private void ShowGameOver() {
    GameState = GameState.Lost;
    dome.targetDissolve = 1f;
    gameOverScreen.Show();
    cityWithDomeSinkAnimation.Play("sink");
  }
}
