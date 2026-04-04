using MegaCrit.Sts2.Core.Models;
using System.Runtime.CompilerServices;

namespace TH_Sakuya.Scripts.Main;

public static class TimeStopCostCache
{
	private static readonly ConditionalWeakTable<CardModel, CostInfo> _costs = new ConditionalWeakTable<CardModel, CostInfo>();

	public static void RecordEnergyCost(CardModel card, int computedCost)
	{
		CostInfo info = _costs.GetOrCreateValue(card);
		info.LastComputedEnergyCost = computedCost;
	}

	public static int GetLastComputedEnergyCost(CardModel card)
	{
		if (_costs.TryGetValue(card, out CostInfo? info))
		{
			return info.LastComputedEnergyCost;
		}
		return 0;
	}

	private sealed class CostInfo
	{
		public int LastComputedEnergyCost;
	}
}
