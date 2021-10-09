using System;
using Godot;

abstract class Wave {

  protected float DESTRUCTIBLE_SPAWN_EVERY = 3.2f;

  protected float destructibleSpawnCooldown = 0;
  protected int currentMinesAmount = 0;
  protected int leftMines = 0;


  internal Game game;

  public Wave() {
    destructibleSpawnCooldown = 0f;
  }


  // returns true if wave is finished
  public bool process(float delta) {
    destructibleSpawnCooldown -= delta;

    if (leftMines > 0 && destructibleSpawnCooldown <= 0) {
      destructibleSpawnCooldown = DESTRUCTIBLE_SPAWN_EVERY;

      game.SpawnMine(new Vector3(
        (float)GD.RandRange(-15, -12),
        0,
        (float)GD.RandRange(-10, 10))
      );
      leftMines -= 1;
    }


    var mines = game.GetTree().GetNodesInGroup(C.Groups.Mines);
    return mines.Count == 0 && leftMines == 0;
  }

}


class Wave0 : Wave {
  public Wave0() {
    leftMines = 2;
    DESTRUCTIBLE_SPAWN_EVERY = 2f;
  }
}

class Wave1 : Wave {
  public Wave1() {
    leftMines = 40;
    DESTRUCTIBLE_SPAWN_EVERY = 0.7f;
  }
}


class Wave2 : Wave {
  public Wave2() {
    leftMines = 70;
    DESTRUCTIBLE_SPAWN_EVERY = 0.66f;
  }
}


class Wave3 : Wave {
  public Wave3() {
    leftMines = 70;
    DESTRUCTIBLE_SPAWN_EVERY = 0.6f;
  }
}


class Wave4 : Wave {
  public Wave4() {
    leftMines = 100;
    DESTRUCTIBLE_SPAWN_EVERY = 0.69f;
  }
}