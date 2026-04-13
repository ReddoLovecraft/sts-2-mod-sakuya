using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace TH_Sakuya.Scripts.Patches
{
    [HarmonyPatch(typeof(TouchOfOrobas),"GetUpgradedStarterRelic",[typeof(RelicModel)])]
     public static class RelicUpgradePatch
     {
        static void Postfix(ref RelicModel __result,RelicModel starterRelic)
        {
            if(starterRelic ==null)
            return;
            if(starterRelic is SakuyaWatch)
            __result=ModelDb.Relic<SakuyaLunaDial>();
        }
     } 
}
