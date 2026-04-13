using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scrpits.Cards;

namespace TH_Sakuya.Scripts.Patches
{
    [HarmonyPatch(typeof(DustyTome), "SetupForPlayer", [typeof(Player)])]
    public static class DustyTomeSetupForPlayerPatch
    {
        static bool Prefix(DustyTome __instance, Player player)
        {
            if (player?.Character is not SakuyaCharacter)
            {
                return true;
            }

            try
            {
                __instance.AncientCard = ModelDb.Card<Infinite>().Id;
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"Failed to set DustyTome.AncientCard to Infinite: {e}");
                return true;
            }
        }
    }
}