using System;
using System.Collections.Generic;
using Godot;

public class Heli : KinematicBody {
  const float GRAVITY = -9.8f;
  const float SPEED = 8;
  const float ACCELERATION = 6;
  const float DE_ACCELERATION = 8;

  PackedScene bulletPrefab;
  Camera camera;
  Vector3 velocity;

  List<Mine> collidingMines = new List<Mine>();
  Mine currentlyFoughtMine = null;

  Spatial laser;

  [Export] public bool SpawnBullets = false;

  private float spawnBulletCooldown = 0;

  Vector3 leftOuterBulletOrigin, leftInnerBulletOrigin, rightOuterBulletOrigin, rightInnerBulletOrigin;
  Spatial heliFrontPoint, heliBackPoint;

  public override void _Ready() {

    var bulletOrigins = GetNode<Spatial>("./bulletOrigins");

    leftOuterBulletOrigin = bulletOrigins.GetNode<Spatial>("./leftOuter").Translation;
    leftInnerBulletOrigin = bulletOrigins.GetNode<Spatial>("./leftInner").Translation;
    rightOuterBulletOrigin = bulletOrigins.GetNode<Spatial>("./rightOuter").Translation;
    rightInnerBulletOrigin = bulletOrigins.GetNode<Spatial>("./rightInner").Translation;
    RemoveChild(bulletOrigins);

    heliFrontPoint = GetNode<Spatial>("./markingPoints/front");
    heliBackPoint = GetNode<Spatial>("./markingPoints/back");

    laser = GetNode<Spatial>("./laser");
    laser.Visible = false;
  }

  public override void _Process(float delta) {
    bool letMineGo = false, laserWorking = false, allowBullets = false;

    if (this.G().game.GameState == GameState.GoingOn) {

      if (Input.IsActionPressed(C.Action.BottomAttack)) {
        if (collidingMines.Count > 0) {
          currentlyFoughtMine = collidingMines[0];
          currentlyFoughtMine.stopped = true;
          laserWorking = true;
        }

        if (currentlyFoughtMine != null) {
          if (currentlyFoughtMine.health > 0) {
            laserWorking = true;
            currentlyFoughtMine.Damage(delta);
          } else {
            letMineGo = true;
          }
        }
      } else {
        letMineGo = true;

        if (Input.IsActionPressed(C.Action.FrontAttack)) {
          // TODO this is not useful for now, no enemies to shoot at
          // allowBullets = true;
        }
      }
    } else {
      letMineGo = true;
    }

    laser.Visible = laserWorking;

    SpawnBullets = allowBullets;

    // if (currentlyFoughtMine != null && Godot.Object.IsInstanceValid(currentlyFoughtMine)) {
    // neither works ;[]
    // laser.LookAt(currentlyFoughtMine.GlobalTransform.origin, new Vector3(0, 0, 1));

    // laser.LookAt(
    //   (laser.GlobalTransform.origin - currentlyFoughtMine.GlobalTransform.origin).Normalized(),
    //   Vector3.Up);
    // }

    if (letMineGo) {
      if (currentlyFoughtMine != null) {
        currentlyFoughtMine.stopped = false;
        // collidingMines.Remove(currentlyFoughtMine);
        currentlyFoughtMine = null;
      }
    }

    if (SpawnBullets) {
      spawnBulletCooldown -= delta;

      if (spawnBulletCooldown <= 0) {
        spawnBulletCooldown += 0.1f;

        var pos = this.GlobalTransform.origin;
        var dir = GetDirection();


        this.G().game.SpawnBullet(pos + rightOuterBulletOrigin, dir);
        this.G().game.SpawnBullet(pos + rightInnerBulletOrigin, dir);
        this.G().game.SpawnBullet(pos + leftOuterBulletOrigin, dir);
        this.G().game.SpawnBullet(pos + leftInnerBulletOrigin, dir);
      }
    }
  }

  public Vector3 GetDirection() {
    var dir = heliFrontPoint.GlobalTransform.origin - heliBackPoint.GlobalTransform.origin;
    return dir.Normalized();
  }

  public override void _PhysicsProcess(float delta) {
    var dir = new Vector3();
    bool vertChanged = true, horzChanged = true;

    if (Input.IsActionPressed(C.Action.Left)) {
      // dir += -camera.Transform.basis[0];
      dir += Vector3.Forward;
    } else if (Input.IsActionPressed(C.Action.Right)) {
      // dir += camera.Transform.basis[0];
      dir += Vector3.Back;
    } else {
      horzChanged = false;
    }
    if (Input.IsActionPressed(C.Action.Up)) {
      // dir += -camera.Transform.basis[2];
      dir += Vector3.Right;
    } else if (Input.IsActionPressed(C.Action.Down)) {
      // dir += camera.Transform.basis[2];
      dir += Vector3.Left;
    } else { vertChanged = false; }

    dir.y = 0;
    dir = dir.Normalized();
    //velocity.y = +delta * GRAVITY;

    var hVel = velocity;
    hVel.y = 0;
    var directedTranslation = dir * SPEED;
    var accel = DE_ACCELERATION;
    if (dir.Dot(hVel) > 0) {
      accel = ACCELERATION;
    }
    hVel = hVel.LinearInterpolate(directedTranslation, accel * delta);
    velocity.x = hVel.x;
    velocity.z = hVel.z;
    velocity = MoveAndSlide(velocity, new Vector3(0, 1, 0));
    velocity.y = 0;

    // var interpolatedTargetDir = this.Rotation;
    // float angle = Mathf.Rad2Deg(Mathf.Atan2(Rotation.y, Vector3.Right.y));
    // GD.Print("angle " + angle);
    // interpolatedTargetDir.

    if (horzChanged || vertChanged) {
      LookAt(this.Translation + dir, Vector3.Up);
    }
  }

  private void _on_Area_body_entered(Node body) {
    GD.Print("player <--> " + body.Name);

    if (body is Mine mine && mine.health > 0) {
      mine.highlighted = true;
      collidingMines.Add(mine);
    }
  }

  private void _on_Area_body_exited(Node body) {
    if (body is Mine mine) {
      mine.highlighted = false;
      mine.stopped = false;
      collidingMines.Remove(mine);
    }
  }

}
