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
    
    private static MenuPage MenuPage;
    private static bool CreateButtonHandler(MenuPage previousMenuPage, out SmallButton smallButton)
    {
        smallButton = new SmallButton(previousMenuPage, "Breakable Charms");
        smallButton.AddHideAndShowEvent(previousMenuPage, MenuPage);
        return true;
    }

    private static void CreateMenuChangerMenu(MenuPage previousMenuPage)
    {
        MenuPage = new MenuPage("Breakable Charms", previousMenuPage);
        new VerticalItemPanel(MenuPage, 
            localTopCenter: new Vector2(0f, 300f),
            vspace: 75f,
            rootLevel:true,
            children: new MenuElementFactory<GlobalSettings>(MenuPage, BreakableCharms.globalSettings).Elements);
    }
}