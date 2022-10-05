using Osmi.Game;
using TMPro;

namespace BreakableCharms;

public static class CharmUtils
{
    public static void RemoveCharmEffects(int charmNumBroken)
    {
        if (charmNumBroken == (int)Charm.NailmastersGlory)
        {
            ReflectionHelper.SetField(HeroController.instance, "nailChargeTime",
            !PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_26))
                ? HeroController.instance.NAIL_CHARGE_TIME_DEFAULT
                : HeroController.instance.NAIL_CHARGE_TIME_CHARM);
        }
        
        if (charmNumBroken == (int) Charm.UnbreakableHeart)
        {
            PlayerData.instance.SetInt(nameof(PlayerData.maxHealth), PlayerData.instance.GetInt(nameof(PlayerData.maxHealthBase)));
            
            if (PlayerData.instance.GetInt(nameof(PlayerData.health)) > PlayerData.instance.GetInt(nameof(PlayerData.maxHealth)))
            {
                PlayerData.instance.SetInt(nameof(PlayerData.health), PlayerData.instance.GetInt(nameof(PlayerData.maxHealth)));    
            }
        }

        if (charmNumBroken == (int)Charm.Grimmchild)
        {
            HeroController.instance.carefreeShieldEquipped = PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_40)) && 
                                                             PlayerData.instance.GetInt(nameof(PlayerData.grimmChildLevel)) == 5;
        }

        int oldHealthBlue = PlayerData.instance.GetInt(nameof(PlayerData.healthBlue));
        
        if (charmNumBroken == (int)Charm.LifebloodHeart)
        {
            PlayerData.instance.SetInt(nameof(PlayerData.healthBlue), (oldHealthBlue - 2).SetPositive());
        }
        if (charmNumBroken == (int)Charm.LifebloodCore)
        {
            PlayerData.instance.SetInt(nameof(PlayerData.healthBlue), (oldHealthBlue - 4).SetPositive());
        }

        if (charmNumBroken == (int)Charm.JonisBlessing)
        {
            PlayerData.instance.SetInt(nameof(PlayerData.joniHealthBlue), 0);
            PlayerData.instance.SetInt(nameof(PlayerData.maxHealth), PlayerData.instance.GetInt(nameof(PlayerData.maxHealthBase)));
            PlayerData.instance.SetInt(nameof(PlayerData.health), PlayerData.instance.GetInt(nameof(PlayerData.maxHealth)));  
            PlayerData.instance.SetBool("joniBeam", false);
        }
    }
    
    public static void BreakCharm(int charmNum)
    {
        BreakableCharms.localSettings.BrokenCharms[charmNum].isBroken = true;
        CharmUtil.UnequipCharm(charmNum);
        
        if(charmNum == (int)Charm.Grimmchild)
        {
            var gc = GameObject.FindWithTag("Grimmchild");
            if (gc != null)
            {
                UnityEngine.Object.Destroy(gc);
            }
            
        }
    }
    
     public static void SetAllCharmIcons(bool changeDetails = false, int charmNumOfDetails = 0)
    {
        if (CharmIconList.Instance != null)
        {
            CharmIconList.Instance.grimmchildLevel1 = BreakableCharms.localSettings.BrokenCharms[(int)Charm.Grimmchild].GetSprite();
            CharmIconList.Instance.grimmchildLevel2 = BreakableCharms.localSettings.BrokenCharms[(int)Charm.Grimmchild].GetSprite();
            CharmIconList.Instance.grimmchildLevel3 = BreakableCharms.localSettings.BrokenCharms[(int)Charm.Grimmchild].GetSprite();
            CharmIconList.Instance.grimmchildLevel4 = BreakableCharms.localSettings.BrokenCharms[(int)Charm.Grimmchild].GetSprite();
            CharmIconList.Instance.nymmCharm = BreakableCharms.localSettings.BrokenCharms[(int)Charm.Grimmchild].GetSprite();

            CharmIconList.Instance.unbreakableGreed = BreakableCharms.localSettings.BrokenCharms[(int)Charm.UnbreakableGreed].GetSprite();
            CharmIconList.Instance.unbreakableHeart = BreakableCharms.localSettings.BrokenCharms[(int)Charm.UnbreakableHeart].GetSprite();
            CharmIconList.Instance.unbreakableStrength = BreakableCharms.localSettings.BrokenCharms[(int)Charm.UnbreakableStrength].GetSprite();
        }
        
        foreach (var (charmNum, charmData) in BreakableCharms.localSettings.BrokenCharms)
        {
            if (CharmIconList.Instance != null)
            {
                CharmIconList.Instance.spriteList[charmNum] = charmData.GetSprite();
            }
            if (FSMEdits.CharmUIGameObject != null) 
            {
                FSMEdits.CharmUIGameObject.transform.Find("Collected Charms").Find(charmNum.ToString()).Find("Sprite").ChangeSpriteRenderer(charmData.GetSprite());
                if (changeDetails && charmNum == charmNumOfDetails)
                {
                    FSMEdits.CharmUIGameObject.transform.Find("Details").Find("Detail Sprite").ChangeSpriteRenderer(charmData.GetSprite());
                    FSMEdits.CharmUIGameObject.transform.Find("Text Desc").GetComponent<TextMeshPro>().text = Language.Language.Get($"CHARM_DESC_{charmNum}", "UI"); 
                    FSMEdits.CharmUIGameObject.transform.Find("Text Name").GetComponent<TextMeshPro>().text = Language.Language.Get($"CHARM_NAME_{charmNum}", "UI");
                }
            }
        }
    }

    public static void BreakEquippedCharms(Func<CharmState, bool> hasCorrectCharmState)
    {
        bool anyBroken = false;
        PlayerData.instance.GetVariable<List<int>>("equippedCharms").ToList().ForEach(c =>
        {
            if (BreakableCharms.localSettings.BrokenCharms.ContainsKey(c) && 
                hasCorrectCharmState(BreakableCharms.localSettings.BrokenCharms[c].charmState))
            {
                BreakCharm(c);
                RemoveCharmEffects(c);
                anyBroken = true;
            }
        });

        if (anyBroken)
        {
            PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
            PlayMakerFSM.BroadcastEvent("UPDATE BLUE HEALTH");
            CharmUtil.UpdateCharmUI();
            new BreakCharmUIDef().SendMessage(MessageType.Corner, null);

            GameManager.instance.StartCoroutine(FixHealth());

            IEnumerator FixHealth()
            {
                yield return null;
                yield return null;
                
                //get the parent for all health gos
                var healthParent = GameCameras.instance.hudCanvas.transform.Find("Health");
                
                for(int i = 0; i < healthParent.childCount; i++)
                {
                    var healthGo = healthParent.GetChild(i);
                    //get all the Health n gos
                    if (healthGo.name.StartsWith("Health"))
                    {
                        //get the health number
                        if (int.Parse(healthGo.name.Split(' ')[1]) <=
                            PlayerData.instance.GetInt(nameof(PlayerData.maxHealth)))
                        {
                            //set state to the state before it checks whether or not to break mask or not after take damage
                            healthGo.gameObject.LocateMyFSM("health_display").SetState("Pause Frame");
                        }
                    }
                }
            }
        }
        SetAllCharmIcons();
        
    }
}