namespace BreakableCharms.Settings;

public static class ModMenu
{
    private static Menu MenuRef;
    public static MenuScreen CreateModMenu(MenuScreen modlistmenu)
    {
        MenuRef ??= new Menu("Breakable Charms", new Element[]
        {
            BoolOption("Break delicate charms on any damage", 
                "Should charms break on taking any type of damage",
                b =>
                {
                    BreakableCharms.globalSettings.Break_DelicateCharms_On_AllDamage = b;
                    MenuRef.Find("DoubleDamage").isVisible = !b;
                    MenuRef.Update();

                },
                () => BreakableCharms.globalSettings.Break_DelicateCharms_On_AllDamage),
            
            BoolOption("Break delicate charms on double damage", 
                "Should charms break on taking double damage",
                b => BreakableCharms.globalSettings.Break_DelicateCharms_On_DoubleDamage = b,
                () => BreakableCharms.globalSettings.Break_DelicateCharms_On_DoubleDamage, 
                Id:"DoubleDamage", isVisible:BreakableCharms.globalSettings.Break_DelicateCharms_On_AllDamage),
        });
        return MenuRef.GetMenuScreen(modlistmenu);
    }

    private static HorizontalOption BoolOption(string name,
        string description,
        Action<bool> applySetting,
        Func<bool> loadSetting,
        string _true = "True",
        string _false = "False",
        string Id = "__UseName",
        bool isVisible = true)

    {
        if (Id == "__UseName")
        {
            Id = name;
        }

        return new HorizontalOption(name,
            description,
            new[] { _true, _false },
            (i) => applySetting(i == 0),
            () => loadSetting() ? 0 : 1,
            Id
        ) { isVisible = isVisible };
    }
}