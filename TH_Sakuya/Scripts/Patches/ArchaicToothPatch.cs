using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scrpits.Cards;

namespace TH_Sakuya.Scripts.Patches
{
     [HarmonyPatch(typeof(ArchaicTooth),"GetTranscendenceStarterCard",[typeof(Player)])]
    public static class GetStarterCardTransformPatch
    {
        static void Postfix(ref CardModel __result,Player player)
        {
            if (player == null)
            {
                return;
            }

            if(player.Character is SakuyaCharacter)
            {
                CardModel? cm = player.Deck?.Cards?.FirstOrDefault((CardModel c) => c is MaidSecret);
                if(cm!=null)
                __result=cm;
                else
                 Log.Debug("MaidSecret not found in deck");
            }
           
        }
    }
    [HarmonyPatch(typeof(ArchaicTooth),"GetTranscendenceTransformedCard",[typeof(CardModel)])]
    [HarmonyPriority(Priority.First)]
    public static class GetStarterCardTransformedPatch
    {
        static bool _logged;

        static bool Prefix(ArchaicTooth __instance, ref CardModel __result, CardModel starterCard, ref CardModel? __state)
        {
            try
            {
                if (starterCard == null)
                {
                    return true;
                }

                if (starterCard is not MaidSecret)
                {
                    return true;
                }

                if (!_logged)
                {
                    _logged = true;
                    Log.Debug("GetTranscendenceTransformedCard Prefix: handling MaidSecret");
                }

                var template = ModelDb.Card<MaidSecretAll>();
                if (template == null)
                {
                    Log.Error("MaidSecretAll card template not found in ModelDb");
                    return true;
                }

                CardModel? cardModel = template.MutableClone() as CardModel;
                if (cardModel == null)
                {
                    Log.Error("Failed to clone MaidSecret card template");
                    return true;
                }

                if (starterCard.IsUpgraded)
                {
                    CardCmd.Upgrade(cardModel);
                }

                if (starterCard.Enchantment is EnchantmentModel enchantment)
                {
                    if (enchantment.MutableClone() is EnchantmentModel enchantmentModel)
                    {
                        CardCmd.Enchant(enchantmentModel, cardModel, enchantmentModel.Amount);
                    }
                }

                __state = cardModel;
                __result = cardModel;
                __result.Owner = starterCard.Owner;
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"GetTranscendenceTransformedCard patch failed: {e}");
                return true;
            }
        }

        [HarmonyPriority(Priority.Last)]
        static void Postfix(ref CardModel __result, CardModel starterCard, CardModel? __state)
        {
            if (starterCard is MaidSecret && __state != null)
            {
                __result = __state;
            }
        }
    }
}