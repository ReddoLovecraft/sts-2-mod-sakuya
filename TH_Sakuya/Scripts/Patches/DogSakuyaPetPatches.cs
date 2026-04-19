using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using TH_Sakuya.Scrpits.Relics;

namespace TH_Sakuya.Scripts.Patches;

[HarmonyPatch]
public static class DogSakuyaPetPatches
{
	[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
	[HarmonyPostfix]
	private static void NCombatUi_Activate_Postfix(NCombatUi __instance, CombatState state)
	{
		Player me = LocalContext.GetMe(state);
		if (me.GetRelic<DogSakuya>() == null)
		{
			return;
		}
		if (__instance.GetNodeOrNull<Node>("DogSakuyaPet") != null)
		{
			return;
		}
		PackedScene? scene = GD.Load<PackedScene>("res://TH_Sakuya/ArtWorks/UI/dog_sakuya_pet.tscn");
		if (scene == null)
		{
			return;
		}
		Node node = scene.Instantiate();
		node.Name = "DogSakuyaPet";
		if (node is Control c)
		{
			c.Position = new Vector2(220f, 730f);
		}
		__instance.AddChildSafely(node);
	}
}
