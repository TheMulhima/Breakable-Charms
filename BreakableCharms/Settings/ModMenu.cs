namespace BreakableCharms.Settings;

public static class ModMenu
{
    private static Menu MenuRef;
    public static MenuScreen CreateModMenu(MenuScreen modlistmenu)
    {
        MenuRef ??= new Menu("Breakable Charms", new Element[]
        {
            Blueprints.HorizontalBoolOption("Break delicate charms on any damage", 
                "Should charms break on taking any type of damage",
                b =>
                {
                    BreakableCharms.globalSettings.Break_DelicateCharms_On_AllDamage = b;
                    MenuRef.Find("DoubleDamage").isVisible = !b;
                    MenuRef.Update();
                },
                () => BreakableCharms.globalSettings.Break_DelicateCharms_On_AllDamage),
            
            Blueprints.HorizontalBoolOption("Break delicate charms on double damage", 
                "Should charms break on taking double damage",
                b => BreakableCharms.globalSettings.Break_DelicateCharms_On_DoubleDamage = b,
                () => BreakableCharms.globalSettings.Break_DelicateCharms_On_DoubleDamage, 
                Id:"DoubleDamage"),


        });
        MenuRef.OnBuilt += (_, _) =>
        {
            MenuRef.Find("DoubleDamage").isVisible = BreakableCharms.globalSettings.Break_DelicateCharms_On_AllDamage;
            MenuRef.Update();
        };
        return MenuRef.GetMenuScreen(modlistmenu);
    }
}