static class C {
  public const float MINE_DAMAGE = 0.1f;
  public const float MINE_BASE_SPEED = 0.3f;
  public const float MINE_SPEED_FACTOR_MIN = 1.0f;
  public const float MINE_SPEED_FACTOR_MAX = 2.2f;

  public const float HELI_TO_MINE_DAMAGE_PER_SECOND = 1.5f;
  public const float MINE_FROZEN_TINT_GROW_SPEED = HELI_TO_MINE_DAMAGE_PER_SECOND * 1.5f;
  public const float BULLET_SPEED = 30f;
  public const float BULLET_LIFE_BOUNDARY_SIZE = 30f;

  public static class Groups {
    public static string Mines = "mines";
    public static string Bullets = "bullets";
  }

  public static class Action {
    public static string Left = "ui_left", Right = "ui_right", Up = "ui_up", Down = "ui_down";
    public static string FrontAttack = "game_front_attack";
    public static string BottomAttack = "game_bottom_attack";
  }
}

public static class Res {
  public static string Uri(string path) => "res://" + path;


  public static class Prefab {
    public static string Uri(string name) => Res.Uri("prefabs/" + name + ".tscn");

    public static string Mine = Uri("Mine");
    public static string Bullet = Uri("Bullet");
  }

}

