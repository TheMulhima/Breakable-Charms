using Osmi.Utils;
using TMPro;

namespace BreakableCharms;

public static class FSMEdits
{
    public static GameObject CharmUIGameObject => GameCameras.instance.hudCamera.transform.Find("Inventory").Find("Charms").gameObject;
    private static PlayMakerFSM charmFSM, costFSM;
    private static GameObject costGo;
    private static TransformDelegate costGoTranform;
    
    
    public static string CharmNumVariableName = "Current Item Number";

    public static void CharmMenuFSMEdits()
    { 
        costGo = CharmUIGameObject.Find("Details").Find("Cost");
        costGoTranform = costGo.GetTransformDelegate();
        
        charmFSM = CharmUIGameObject.LocateMyFSM("UI Charms");
        costFSM = costGo.LocateMyFSM("Charm Details Cost");
        
        charmFSM.CreateEmptyState();
        costFSM.CreateEmptyState();

        FSMEdits_BrokenCharmFunctionality();
        
        FSMEdit_RepairCostDisplay();
        
        charmFSM.AddCustomAction("Idle Collection", () => CharmUtils.SetAllCharmIcons());
    }

    private static void FSMEdit_RepairCostDisplay()
    {
        //there is a different event for each charm cost
        for (int i = 0; i <= 6; i++)
        {
            costFSM.Intercept(new TransitionInterceptor
            {
                fromState = "Check",
                eventName = i.ToString(),
                toStateDefault = "Empty", //should intercept is true so it doesnt matter
                toStateCustom = "Empty", //i will handle the cases myself
                shouldIntercept = () => true,
                onIntercept = (_, originalEventName) => DisplayRepairCost(originalEventName)
            });
        }

    }
    
    private static void DisplayRepairCost(string originalEventName)
    {
        var charmNum = charmFSM.GetVariable<FsmInt>(CharmNumVariableName).Value;
        
        if (PlayerData.instance.GetBool($"gotCharm_{charmNum}") &&
            BreakableCharms.localSettings.BrokenCharms.TryGetValue(charmNum, out var charmData) &&
            charmData.isBroken &&
            charmFSM.GetVariable<FsmBool>("Idle Collection").Value) //makes sure its not in equipped charms area
        {
            var costText = costGo.Find("Text Cost");
            costText.gameObject.SetActive(true);
            costText.GetComponent<MeshRenderer>().enabled = true;
            
            //set position to right most vanilla position 
            costGoTranform.LocalX = costFSM.GetVariable<FsmFloat>("1 X").Value;
            
            foreach (MeshRenderer meshRenderer in costText.GetComponentsInChildren<MeshRenderer>(includeInactive:true))
            {
                meshRenderer.enabled = true;
            }

            costText.GetComponent<TextMeshPro>().text = "Cost  200";
                
            //get the first notch cost icon    
            var geoIcon = costGo.Find($"Cost 1");
            //make geo icon go show up
            geoIcon.GetTransformDelegate().LocalY = costFSM.GetVariable<FsmFloat>("Present Y").Value + 0.05f;
            geoIcon.GetComponent<SpriteRenderer>().sprite = SpriteUtils.LoadSpriteFromResources("Misc.Geo");
            
            //make all other notch icons disappear
            for (int i = 1; i <= 6; i++)
            {
                if (i == 1) continue;
                var costx = costGo.Find($"Cost {i}");
                costx.GetTransformDelegate().LocalY = costFSM.GetVariable<FsmFloat>("Absent Y").Value;
            }
        }
        else
        {
            costGo.Find("Cost 1").GetComponent<SpriteRenderer>().sprite = BreakableCharms.charmCostIndicator;
            costGo.Find("Text Cost").GetComponent<TextMeshPro>().text = "Cost";
            costFSM.SetState($"Cost {originalEventName}"); //show the correct notches and stuff
        }
    }

    private static void FSMEdits_BrokenCharmFunctionality()
    {
        charmFSM.Intercept(new TransitionInterceptor
        {
            fromState = "Deactivate UI",
            eventName = "FINISHED",
            toStateDefault = "Unequippable", //should intercept is true so it doesnt matter
            toStateCustom = "Unequippable", //i will handle the cases myself
            shouldIntercept = () => true,
            onIntercept = (_, _) =>
            {
                var charmNum = charmFSM.GetVariable<FsmInt>(CharmNumVariableName).Value;

                if (BreakableCharms.localSettings.BrokenCharms.TryGetValue(charmNum, out var charmData) && 
                    charmData.isBroken &&
                    charmFSM.GetVariable<FsmBool>("Idle Collection").Value)
                {
                    if (PlayerData.instance.GetInt(nameof(PlayerData.geo)) >= 200)
                    {
                        RepairCharm(charmNum);
                        PlayerData.instance.IntAdd(nameof(PlayerData.geo), -200);
                        HeroController.instance.geoCounter.geoTextMesh.text = PlayerData.instance.GetInt(nameof(PlayerData.geo)).ToString();
                        BreakableCharms.AudioPlayer.pitch = 1f;
                        BreakableCharms.AudioPlayer.PlayOneShot(BreakableCharms.charmBuySuccess);
                    }
                    else
                    {
                        BreakableCharms.AudioPlayer.pitch = 1.25f;
                        BreakableCharms.AudioPlayer.PlayOneShot(BreakableCharms.charmBuyFail);
                        BreakableCharms.AudioPlayer.pitch = 0.85f;
                        BreakableCharms.AudioPlayer.PlayOneShot(BreakableCharms.charmBuyFail);
                    }
                    
                    charmFSM.SetState("Unequippable");
                    
                }
                else
                {
                    //next state in chain, skips break
                    charmFSM.SetState("Royal?");
                }
                    
            }
        });
    }

    private static void RepairCharm(int charmNum)
    {
        BreakableCharms.localSettings.BrokenCharms[charmNum].isBroken = false;
        CharmUtils.SetAllCharmIcons(changeDetails: true, charmNumOfDetails: charmNum);

        var notchCost = PlayerData.instance.GetInt($"charmCost_{charmNum}");
        if (notchCost == 0)//void heart
        {
            costGo.gameObject.SetActive(false);
        }
        else
        {
            costGoTranform.LocalX = costFSM.GetVariable<FsmFloat>($"{notchCost} X").Value;
        }
        costGo.Find("Text Cost").GetComponent<TextMeshPro>().text = "Cost";
        costGo.Find($"Cost 1").GetComponent<SpriteRenderer>().sprite = BreakableCharms.charmCostIndicator;


        for (int i = 1; i <= 6; i++)
        {
            var costx = costGo.Find($"Cost {i}");
            costx.GetTransformDelegate().LocalY = costFSM.GetVariable<FsmFloat>(i <= notchCost ? "Present Y" : "Absent Y").Value;
        }
    }
}