namespace BreakableCharms;

internal static class Ref
{
    internal static PlayerData PD => PlayerData.instance;
    internal static SceneData SD => SceneData.instance;
    internal static GameManager GM => GameManager.instance;
    internal static InputHandler IH => InputHandler.Instance;
    internal static HeroController HC =>  HeroController.instance;
    internal static GameObject Knight => HC.gameObject;
}