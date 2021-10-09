using Godot;

public class Globals : Node {
  internal static Globals Instance;

  // it contains the `mainCamera`. used for shaking
  public Spatial mainCameraOffset;
  public MainCamera mainCamera;
  public Game game;
}

public static class GlobalsExt {
  public static Globals G(this Node node) {
    if (Globals.Instance == null) {
      Globals.Instance = (Globals)node.GetNode("/root/Globals");
    }

    return Globals.Instance;
  }
}