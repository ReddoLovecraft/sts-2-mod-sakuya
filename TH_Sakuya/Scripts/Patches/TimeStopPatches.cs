using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Threading.Tasks;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Nodes.Combat;
using TH_Sakuya.Scripts.Powers;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TH_Sakuya.Scrpits.Cards;
using TH_Sakuya.Scripts.GameActions;

namespace TH_Sakuya.Scripts.Patches;

[HarmonyPatch]
public static class TimeStopPatches
{
	private static readonly AccessTools.FieldRef<PlayerCombatState, Player> _pcsPlayerRef =
		AccessTools.FieldRefAccess<PlayerCombatState, Player>("_player");


	[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
	[HarmonyPrefix]
	private static bool PlayerCombatState_HasEnoughResourcesFor_Prefix(PlayerCombatState __instance, CardModel card, ref UnplayableReason reason, ref bool __result)
	{
		Player player = card?.Owner ?? _pcsPlayerRef(__instance);
		if (!TimeStopPointSystem.IsEnabledFor(player) || player.Creature == null || !player.Creature.HasPower<TimeStopPower>())
		{
			return true;
		}

		int tsp = TimeStopPointSystem.Get(player);
		int energyCost = Math.Max(0, card.EnergyCost.GetWithModifiers(CostModifiers.All));
		int tspCost = card.EnergyCost.CostsX ? 0 : energyCost * TimeStopPointSystem.PointsPerEnergy;

		reason = UnplayableReason.None;
		if (tspCost > tsp)
		{
			reason |= UnplayableReason.EnergyCostTooHigh;
		}

		int starsCost = Math.Max(0, card.GetStarCostWithModifiers());
		if (starsCost > __instance.Stars)
		{
			reason |= UnplayableReason.StarCostTooHigh;
		}

		__result = reason == UnplayableReason.None;
		return false;
	}

	[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
	[HarmonyPrefix]
	private static bool CardModel_SpendResources_Prefix(CardModel __instance, ref Task<(int, int)> __result)
	{
		if (__instance.CombatState == null || __instance.Owner?.Creature == null)
		{
			return true;
		}
		if (!TimeStopPointSystem.IsEnabledFor(__instance.Owner) || !__instance.Owner.Creature.HasPower<TimeStopPower>())
		{
			return true;
		}
		__result = SpendResourcesDuringTimeStop(__instance);
		return false;
	}

	private static async Task<(int, int)> SpendResourcesDuringTimeStop(CardModel card)
	{
		if (card.CombatState == null || card.Owner?.Creature?.CombatState == null || card.Owner.PlayerCombatState == null)
		{
			return (0, 0);
		}
		var combatState = card.CombatState;
		int tspToSpend = 0;
		if (card.EnergyCost.CostsX)
		{
			int availableTsp = TimeStopPointSystem.Get(card.Owner);
			int x = Math.Max(0, availableTsp / TimeStopPointSystem.PointsPerEnergy);
			tspToSpend = x * TimeStopPointSystem.PointsPerEnergy;
			if (!card.IsDupe)
			{
				card.EnergyCost.CapturedXValue = x;
			}
		}
		else
		{
			int energyCost = Math.Max(0, card.EnergyCost.GetWithModifiers(CostModifiers.All));
			tspToSpend = energyCost * TimeStopPointSystem.PointsPerEnergy;
		}

		if (!await TimeStopPointSystem.TrySpend(card.Owner, tspToSpend))
		{
			return (0, 0);
		}
		if (TimeStopPointSystem.Get(card.Owner) == 0 && card.Owner.Creature.HasPower<TimeStopPower>())
		{
			SfxCmd.Play(SakuyaInit.ToModSfxPath("TH_Sakuya/ArtWorks/SFX/tsprunout.wav"));
			 if(!card.Owner.Creature.HasPower<SakuyaWorldPower>())
			{
				await PowerCmd.Remove<TimeStopPower>(card.Owner.Creature);
				await TimeStopPointSystem.RestoreExitCards(card.Owner);
			}
			await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), card.Owner.Creature, 1m, card.Owner.Creature, cardSource: null, silent: true);
			await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), card.Owner.Creature, 1m, card.Owner.Creature, cardSource: null, silent: true);
		}

		int starsToSpend = Math.Max(0, card.GetStarCostWithModifiers());

		await Hook.AfterEnergySpent(combatState, card, 0);

		if (!card.IsDupe)
		{
			card.LastStarsSpent = starsToSpend;
		}

		if (starsToSpend > 0)
		{
			card.Owner.PlayerCombatState.LoseStars(starsToSpend);
			await Hook.AfterStarsSpent(card.Owner.Creature.CombatState!, starsToSpend, card.Owner);
		}

		return (0, starsToSpend);
	}

	[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
	[HarmonyPostfix]
	private static void NCombatUi_Activate_Postfix(NCombatUi __instance, MegaCrit.Sts2.Core.Combat.CombatState state)
	{
		Player me = MegaCrit.Sts2.Core.Context.LocalContext.GetMe(state);
		if (!TimeStopPointSystem.IsEnabledFor(me))
		{
			return;
		}

		NStarCounter starCounter = __instance.GetNode<NStarCounter>("%StarCounter");
		starCounter.Visible = false;

		if (__instance.EnergyCounterContainer == null)
		{
			return;
		}
		if (__instance.EnergyCounterContainer.GetNodeOrNull<Control>("TimeStopPointCounter") != null)
		{
			return;
		}

		PackedScene? scene = GD.Load<PackedScene>("res://TH_Sakuya/ArtWorks/Character/tsp_counter.tscn");
		if (scene == null)
		{
			return;
		}
		Node node = scene.Instantiate();
		if (node is not NTimeStopPointCounter counter)
		{
			return;
		}
		counter.Name = "TimeStopPointCounter";
		counter.Initialize(me);

		NEnergyCounter? energyCounter = __instance.EnergyCounterContainer.GetChildCount() > 0
			? __instance.EnergyCounterContainer.GetChild<NEnergyCounter>(0)
			: null;

		if (energyCounter != null)
		{
			energyCounter.AddChildSafely(counter);
		}
		else
		{
			__instance.EnergyCounterContainer.AddChildSafely(counter);
		}
	}

	[HarmonyPatch(typeof(Hook), nameof(Hook.AfterEnergySpent))]
	[HarmonyPostfix]
	private static void Hook_AfterEnergySpent_Postfix(CombatState combatState, CardModel card, int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		Player? player = card?.Owner;
		if (player == null || !TimeStopPointSystem.IsEnabledFor(player))
		{
			return;
		}
		RunManager.Instance?.ActionQueueSynchronizer?.RequestEnqueue(new InfiniteEnergySpentAction(player, amount));
	}

	[HarmonyPatch(typeof(NCombatCardPile), "OnPress")]
	[HarmonyFinalizer]
	private static Exception? NCombatCardPile_OnPress_Finalizer(Exception __exception)
	{
		if (__exception is ObjectDisposedException)
		{
			return null;
		}
		return __exception;
	}

	[HarmonyPatch(typeof(NCombatCardPile), "OnRelease")]
	[HarmonyFinalizer]
	private static Exception? NCombatCardPile_OnRelease_Finalizer(Exception __exception)
	{
		if (__exception is ObjectDisposedException)
		{
			return null;
		}
		return __exception;
	}

	[HarmonyPatch(typeof(ActionQueueSynchronizer), nameof(ActionQueueSynchronizer.RequestEnqueue))]
	[HarmonyPostfix]
	private static void ActionQueueSynchronizer_RequestEnqueue_Postfix(ActionQueueSynchronizer __instance, GameAction action)
	{
		if (action == null || action is SilverSpaceGrantTspAction)
		{
			return;
		}

		if (!RunManager.Instance.IsInProgress
			|| string.Equals(RunManager.Instance.NetService.Type.ToString(), "Singleplayer", StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		CardModel? card = TryGetCardFromPlayCardAction(action);
		if (card is not SilverSpace)
		{
			return;
		}

		Player? owner = card.Owner;
		if (owner?.Creature == null || !owner.Creature.HasPower<TimeStopPower>())
		{
			return;
		}

		int tsp = CalculateEnemyAttackIntentTotal(owner);
		__instance.RequestEnqueue(new SilverSpaceGrantTspAction(owner, tsp));
	}

	private static CardModel? TryGetCardFromPlayCardAction(GameAction action)
	{
		Type t = action.GetType();
		if (!t.Name.Contains("PlayCardAction", StringComparison.Ordinal))
		{
			return null;
		}

		PropertyInfo? prop = t.GetProperty("Card", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (prop == null || prop.GetIndexParameters().Length != 0)
		{
			return null;
		}

		return prop.GetValue(action) as CardModel;
	}

	private static int CalculateEnemyAttackIntentTotal(Player player)
	{
		if (player?.Creature?.CombatState == null)
		{
			return 0;
		}

		var combatState = player.Creature.CombatState;
		List<Creature> targets = [player.Creature];
		int sum = 0;
		foreach (Creature enemy in combatState.HittableEnemies)
		{
			if (enemy == null || !enemy.IsAlive || enemy.Monster == null)
			{
				continue;
			}
			foreach (AttackIntent intent in enemy.Monster.NextMove.Intents.OfType<AttackIntent>())
			{
				sum += intent.GetTotalDamage(targets, enemy);
			}
		}
		return sum;
	}
}
