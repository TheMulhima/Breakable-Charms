using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;

namespace BreakableCharms.Randomizer;

public static class RandoMenu
{
    public static void AddMenuPage()
    {
        RandomizerMenuAPI.AddMenuPage(CreateMenuChangerMenu, CreateButtonHandler);
    }

    public static SmallButton SmallButton;
    public static Color OffColor = Colors.DEFAULT_COLOR;
    public static Color OnColor = Colors.TRUE_COLOR;
    
    private static bool CreateButtonHandler(MenuPage previousMenuPage, out SmallButton smallButton)
    {
        SmallButton = smallButton = new SmallButton(previousMenuPage, "Breakable Charms");

        smallButton.Text.color = BreakableCharms.globalSettings.RandomizeCharmLocations ? OnColor : OffColor;

        smallButton.OnClick += CustomOnClick;
        
        return true;
    }

    private static void CustomOnClick()
    {
        BreakableCharms.globalSettings.RandomizeCharmLocations = !BreakableCharms.globalSettings.RandomizeCharmLocations;
        SetButtonColor(BreakableCharms.globalSettings.RandomizeCharmLocations);
    }

    public static void SetButtonColor(bool isActive)
    {
        if (SmallButton != null)
        {
            SmallButton.Text.color = isActive ? OnColor : OffColor;
        }
    }

    private static void CreateMenuChangerMenu(MenuPage _) { }
}