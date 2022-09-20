using TMPro;

namespace BreakableCharms;

public static class FSMEdits
{
    public static GameObject CharmUIGameObject => GameCameras.instance.hudCamera.transform.Find("Inventory").Find("Charms").gameObject;
    private static PlayMakerFSM charmFSM;
    
    public static void CharmFSMEdits()
    {
        charmFSM = CharmUIGameObject.LocateMyFSM("UI Charms");
        charmFSM.CreateEmptyState();
        
        var costgo = CharmUIGameObject.transform.Find("Details").Find("Cost");
        var costFSM = costgo.gameObject.LocateMyFSM("Charm Details Cost");
        costFSM.CreateEmptyState();

        charmFSM.Intercept(new TransitionInterceptor
        {
            fromState = "Deactivate UI",
            eventName = "FINISHED",
            toStateDefault = "Unequippable", //should intercept is true so it doesnt matter
            toStateCustom = "Unequippable", //i will handle the cases myself
            shouldIntercept = () => true,
            onIntercept = (_, _) =>
            {
                var charmNum = charmFSM.GetVariable<FsmInt>("Current Item Number").Value;

                if (BreakableCharms.localSettings.BrokenCharms.TryGetValue(charmNum, out var charmData) && charmData.isBroken)
                {
                    if (Ref.PD.GetInt(nameof(PlayerData.geo)) >= 200)
                    {
                        RepairCharm(costgo, costFSM, charmNum);
                        Ref.PD.IntAdd(nameof(PlayerData.geo), -200);
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
                onIntercept = (_, originalEventName) => DisplayRepairCost(originalEventName, costgo, costFSM)
            });
        }
        
        charmFSM.AddCustomAction("Idle Collection", () => BreakableCharms.SetAllCharmIcons());
    }
    
    private static void DisplayRepairCost(string originalEventName, Transform costgo, PlayMakerFSM costFSM)
    {
        var charmNum = charmFSM.GetVariable<FsmInt>("Current Item Number").Value;
        if (Ref.PD.GetBool($"gotCharm_{charmNum}") &&
            BreakableCharms.localSettings.BrokenCharms.TryGetValue(charmNum, out var charmData) && charmData.isBroken)
        {
            var costText = costgo.Find("Text Cost");
            costgo.localPosition = costgo.localPosition.X(costFSM.GetVariable<FsmFloat>("1 X").Value);
            costText.gameObject.SetActive(true);
            costText.GetComponent<MeshRenderer>().enabled = true;
            foreach (MeshRenderer meshRenderer in costText.GetComponentsInChildren<MeshRenderer>(true))
            {
                meshRenderer.enabled = true;
            }

            costText.GetComponent<TextMeshPro>().text = "Cost  200";
                
                
            var geoIcon = costgo.Find($"Cost 1");
            geoIcon.localPosition = geoIcon.localPosition.Y(costFSM.GetVariable<FsmFloat>("Present Y").Value + 0.05f);
            geoIcon.GetComponent<SpriteRenderer>().sprite = BreakableCharms.geo;
                
            for (int i = 1; i <= 6; i++)
            {
                if (i == 1) continue;
                var costx = costgo.Find($"Cost {i}");
                costx.localPosition = costx.localPosition.Y(costFSM.GetVariable<FsmFloat>("Absent Y").Value);
            }
        }

        else
        {
            costgo.Find("Cost 1").GetComponent<SpriteRenderer>().sprite = BreakableCharms.charmCostIndicator;
            costgo.Find("Text Cost").GetComponent<TextMeshPro>().text = "Cost";
            costFSM.SetState($"Cost {originalEventName}");
        }
    }

    private static void RepairCharm(Transform costgo, PlayMakerFSM costFSM ,int charmNum)
    {
        BreakableCharms.localSettings.BrokenCharms[charmNum].isBroken = false;
        BreakableCharms.SetAllCharmIcons(changeDetails:true, charmNumOfDetails:charmNum);
        
        var notchCost = Ref.PD.GetInt($"charmCost_{charmNum}");
        costgo.localPosition = costgo.localPosition.X(costFSM.GetVariable<FsmFloat>($"{notchCost} X").Value);
        costgo.Find("Text Cost").GetComponent<TextMeshPro>().text = "Cost";
        costgo.Find($"Cost 1").GetComponent<SpriteRenderer>().sprite = BreakableCharms.charmCostIndicator;
                        
        for (int i = 1; i <= 6; i++)
        {
            var costx = costgo.Find($"Cost {i}");
            costx.localPosition = costx.localPosition.Y(costFSM.GetVariable<FsmFloat>(i <= notchCost ? "Present Y" : "Absent Y").Value);
        }
    }
}