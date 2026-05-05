using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TH_Sakuya.Scrpits.Relics;
using static HarmonyLib.AccessTools;

namespace TH_Sakuya.Scripts.Patches;

[HarmonyPatch]
public static class CardPileOrderPatches
{
	private static readonly ConditionalWeakTable<NSimpleCardSelectScreen, List<CardModel>> _selectionOrder = new();

	private static readonly FieldRef<NSimpleCardSelectScreen, HashSet<CardModel>> _simpleSelectedCardsRef =
		FieldRefAccess<NSimpleCardSelectScreen, HashSet<CardModel>>("_selectedCards");

	private static readonly FieldRef<NSimpleCardSelectScreen, CardSelectorPrefs> _simplePrefsRef =
		FieldRefAccess<NSimpleCardSelectScreen, CardSelectorPrefs>("_prefs");

	private static readonly FieldRef<NCardPileScreen, NCardGrid> _pileScreenGridRef =
		FieldRefAccess<NCardPileScreen, NCardGrid>("_grid");

	private static readonly FieldRef<NCardGridSelectionScreen, TaskCompletionSource<IEnumerable<CardModel>>> _gridSelectionCompletionSourceRef =
		FieldRefAccess<NCardGridSelectionScreen, TaskCompletionSource<IEnumerable<CardModel>>>("_completionSource");

	[HarmonyPatch(typeof(NSimpleCardSelectScreen), "OnCardClicked")]
	[HarmonyPrefix]
	private static void NSimpleCardSelectScreen_OnCardClicked_Prefix(NSimpleCardSelectScreen __instance, CardModel card)
	{
		HashSet<CardModel> selected = _simpleSelectedCardsRef(__instance);
		CardSelectorPrefs prefs = _simplePrefsRef(__instance);
		List<CardModel> order = _selectionOrder.GetOrCreateValue(__instance);
		if (selected.Contains(card))
		{
			order.Remove(card);
			return;
		}
		if (selected.Count < prefs.MaxSelect)
		{
			order.Remove(card);
			order.Add(card);
		}
	}

	[HarmonyPatch(typeof(NSimpleCardSelectScreen), "CompleteSelection")]
	[HarmonyPrefix]
	private static bool NSimpleCardSelectScreen_CompleteSelection_Prefix(NSimpleCardSelectScreen __instance)
	{
		HashSet<CardModel> selected = _simpleSelectedCardsRef(__instance);
		List<CardModel> result;
		if (_selectionOrder.TryGetValue(__instance, out List<CardModel>? order))
		{
			result = order.Where(selected.Contains).ToList();
			foreach (CardModel c in selected)
			{
				if (!result.Contains(c))
				{
					result.Add(c);
				}
			}
			_selectionOrder.Remove(__instance);
		}
		else
		{
			result = selected.ToList();
		}

		TaskCompletionSource<IEnumerable<CardModel>> completionSource = _gridSelectionCompletionSourceRef(__instance);
		completionSource.SetResult(result);
		NOverlayStack.Instance.Remove(__instance);
		return false;
	}

	[HarmonyPatch(typeof(NCardPileScreen), "OnPileContentsChanged")]
	[HarmonyPrefix]
	private static bool NCardPileScreen_OnPileContentsChanged_Prefix(NCardPileScreen __instance)
	{
		if (__instance.Pile.Type != PileType.Draw)
		{
			return true;
		}

		Player? owner = __instance.Pile.Cards.FirstOrDefault()?.Owner;
		if (owner?.GetRelic<Dice>() == null)
		{
			return true;
		}

		List<CardModel> list = __instance.Pile.Cards.ToList();
		NCardGrid grid = _pileScreenGridRef(__instance);
		grid.SetCards(list, __instance.Pile.Type, new List<SortingOrders> { SortingOrders.Ascending });
		return false;
	}
}
