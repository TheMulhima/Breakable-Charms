﻿namespace BreakableCharms;

public static class Dictionaries
{
    public static readonly Dictionary<int, Sprite> UnbreakableCharmSpriteFromID = new();
    public static readonly Dictionary<int, Sprite> FragileCharmSpriteFromID = new();
    public static readonly Dictionary<int, Sprite> DelicateCharmSpriteFromID = new();
    public static readonly Dictionary<int, Sprite> BrokenCharmSpriteFromID = new();

    public static readonly Dictionary<int, string> CharmNameFromID = new()
    {
        {1, "Gathering_Swarm"},
        {2, "Wayward_Compass"},
        {3, "Grubsong"},
        {4, "Stalwart_Shell"},
        {5, "Baldur_Shell"},
        {6, "Fury_of_the_Fallen"},
        {7, "Quick_Focus"},
        {8, "Lifeblood_Heart"},
        {9, "Lifeblood_Core"},
        {10, "Defender's_Crest"},
        {11, "Flukenest"},
        {12, "Thorns_of_Agony"},
        {13, "Mark_of_Pride"},
        {14, "Steady_Body"},
        {15, "Heavy_Blow"},
        {16, "Sharp_Shadow"},
        {17, "Spore_Shroom"},
        {18, "Longnail"},
        {19, "Shaman_Stone"},
        {20, "Soul_Catcher"},
        {21, "Soul_Eater"},
        {22, "Glowing_Womb"},
        {23, "Fragile_Heart"},
        {24, "Fragile_Greed"},
        {25, "Fragile_Strength"},
        {26, "Nailmaster's_Glory"},
        {27, "Joni's_Blessing"},
        {28, "Shape_of_Unn"},
        {29, "Hiveblood"},
        {30, "Dream_Wielder"},
        {31, "Dashmaster"},
        {32, "Quick_Slash"},
        {33, "Spell_Twister"},
        {34, "Deep_Focus"},
        {35, "Grubberfly's_Elegy"},
        {36, "Void_Heart"},
        {37, "Sprintmaster"},
        {38, "Dreamshield"},
        {39, "Weaversong"},
        {40, "Grimmchild2"},
    };
    public static readonly Dictionary<int, string> CharmInGameSpriteNameFromID = new()
    {
        { 1, "charm_sprite_02" },
        { 2, "charm_sprite_03" },
        { 3, "charm_grub_mid" },
        { 4, "_0006_charm_stalwart_shell" },
        { 5, "charm_blocker" },
        { 6, "_0005_charm_fury" },
        { 7, "_0005_charm_fast_focus" },
        { 8, "_0010_charm_bluehealth" },
        { 9, "_0007_charm_blue_health_large" },
        { 10, "charm_dung_def" },
        { 11, "charm_fluke" },
        { 12, "_0000_charm_thorn_counter" },
        { 13, "char_mantis" },
        { 14, "_0006_charm_no_recoil" },
        { 15, "_0008_charm_nail_damage_up" },
        { 16, "charm_shade_impact" },
        { 17, "charm_fungus" },
        { 18, "_0007_charm_greed" },
        { 19, "_0002_charm_spell_damage_up" },
        { 20, "_0001_charm_more_soul" },
        { 21, "charm_soul_up_large" },
        { 22, "_0009_charm_Hatchling" },
        { 23, "_0002_charm_glass_heal_full" },
        { 24, "_0003_charm_glass_geo_full" },
        { 25, "_0002_charm_glass_attack_up_full" },
        { 26, "_0004_charm_charge_time_up" },
        { 27, "charm_blue_health_convert" },
        { 28, "charm_slug" },
        { 29, "charm_hive" },
        { 30, "inv_dream_charm" },
        { 31, "_0011_charm_generic_03" },
        { 32, "_0003_charm_nail_slash_speed_up" },
        { 33, "charm_magic_cost_down" },
        { 34, "charm_crystal" },
        { 35, "charm_grub_blade" },
        { 36, "charm_black" },
        { 37, "charm_grimm_sprint_master" },
        { 38, "charm_grimm_markoth_shield" },
        { 39, "charm_grimm_silkweaver" },
        { 40, "charm_grimmkin_05" },
    };
}