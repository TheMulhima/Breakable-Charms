namespace BreakableCharms;

public static class ModMenu
{
    private static Menu MenuRef;
    public static MenuScreen CreateModMenu(MenuScreen modlistmenu)
    {
        MenuRef ??= new Menu("Breakable Charms", new Element[]
        {
            BoolOption("Break on any damage", 
                "Should charms break on taking any type of damage",
                b =>
                {
                    BreakableCharms.globalSettings.BreakOnAllDamage = b;
                    MenuRef.Find("DoubleDamage").isVisible = !b;
                    MenuRef.Update();

                },
                () => BreakableCharms.globalSettings.BreakOnAllDamage),
            
            BoolOption("Break on double damage", 
                "Should charms break on taking double damage",
                b => BreakableCharms.globalSettings.BreakOnDoubleDamage = b,
                () => BreakableCharms.globalSettings.BreakOnDoubleDamage, 
                Id:"DoubleDamage"),
            
            BoolOption("Break on hazard respawn", 
                "Should charms break on hazard respawning.",
                b => BreakableCharms.globalSettings.BreakOnHazardRespawn = b,
                () => BreakableCharms.globalSettings.BreakOnHazardRespawn),
        });

        return MenuRef.GetMenuScreen(modlistmenu);
    }
    
    public static HorizontalOption BoolOption(string name, 
        string description, 
        Action<bool> applySetting, 
        Func<bool> loadSetting, 
        string _true = "True",
        string _false = "False",
        string Id = "__UseName")
    {
        if (Id == "__UseName")
        {
            Id = name;
        }

        return new HorizontalOption(name,
            description,
            new []{_true, _false},
            (i) => applySetting(i == 0),
            () => loadSetting() ? 0 : 1,
            Id
        );
    }
}