using Osmi.Game;
using TMPro;
using HKMirror.Reflection.SingletonClasses;

namespace BreakableCharms;

public static class CharmUtils
{
    public static void RemoveCharmEffects(int charmNumBroken)
    {
        if (charmNumBroken == (int)Charm.NailmastersGlory)
        {
            HeroControllerR.nailChargeTime = 
                !PlayerDataAccess.equippedCharm_26 ?
                    HeroControllerR.NAIL_CHARGE_TIME_DEFAULT :
                    HeroControllerR.NAIL_CHARGE_TIME_CHARM;
        }
        
        if (charmNumBroken == (int) Charm.UnbreakableHeart)
        {
            PlayerDataAccess.maxHealth = PlayerDataAccess.maxHealthBase;
            
            if (PlayerDataAccess.health > PlayerDataAccess.maxHealth)
            {
                PlayerDataAccess.health = PlayerDataAccess.maxHealth;    
            }
        }

        if (charmNumBroken == (int)Charm.Grimmchild)
        {
            HeroControllerR.carefreeShieldEquipped = PlayerDataAccess.equippedCharm_40 && 
                                                             PlayerDataAccess.grimmChildLevel == 5;
        }

        int oldHealthBlue = PlayerDataAccess.healthBlue;
        
        if (charmNumBroken == (int)Charm.LifebloodHeart)
        {
            PlayerDataAccess.healthBlue = (oldHealthBlue - 2).EnsurePositive();
        }
        if (charmNumBroken == (int)Charm.LifebloodCore)
        {
            PlayerDataAccess.healthBlue = (oldHealthBlue - 4).EnsurePositive();
        }

        if (charmNumBroken == (int)Charm.JonisBlessing)
        {
            PlayerDataAccess.joniHealthBlue = 0;
            PlayerDataAccess.maxHealth = PlayerDataAccess.maxHealthBase;
            PlayerDataAccess.health = PlayerDataAccess.maxHealth;
            HeroControllerR.joniBeam = false;
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
        PlayerDataAccess.equippedCharms.ToList().ForEach(c =>
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
            //most likely always gonna run
            if (!PlayerDataAccess.atBench)
            {
                PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
            }
            PlayMakerFSM.BroadcastEvent("UPDATE BLUE HEALTH");
            CharmUtil.UpdateCharmUI();
            new BreakCharmMsgUIDef().SendMessage(MessageType.Corner, null);
            
            GlobalCoroutineExecutor.Start(FixHealth());

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
                        //get the health number and check if its not more than max health
                        if (int.Parse(healthGo.name.Split(' ')[1]) <=
                            PlayerDataAccess.maxHealth &&
                            
                            //what happens when mask is broken (if its already broken dont bother
                            !(healthGo.GetComponent<MeshRenderer>().enabled && !healthGo.Find("Idle").GetComponent<MeshRenderer>().enabled)
                            )
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