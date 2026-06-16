using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Threading.Tasks;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scripts.Main;

public static class TimeStopPointSystem
{
	public const int DefaultMax = 160;
	public const int PointsPerEnergy = 6;

	public static bool IsEnabledFor(Player player)
	{
		if (player == null)
		{
			return false;
		}
		if (player.Character is SakuyaCharacter)
		{
			return true;
		}
		return player.GetRelic<SakuyaWatch>() != null || player.GetRelic<SakuyaLunaDial>() != null;
	}

	public static int Get(Player player)
	{
		return player?.Creature?.GetPowerAmount<TimeStopPointPower>() ?? 0;
	}

	public static int GetMax(Player player)
	{
		if (player == null)
		{
			return DefaultMax;
		}
		int result = DefaultMax;
		if (player.Creature != null && player.Creature.HasPower<ChangeMaxTimePower>())
		{
			result += player.Creature.GetPowerAmount<ChangeMaxTimePower>();
		}
		return Math.Max(0, result);
	}

	public static async Task InitForCombat(Player player, int? max = null, int initial = 0)
	{
		if (player?.Creature == null)
		{
			return;
		}
		await Set(player, initial);
		await PowerCmd.Remove<TimeStopFirstGrantPower>(player.Creature);
	}

	public static async Task Gain(Player player, int amount)
	{
		if (player == null || amount <= 0)
		{
			return;
		}
		int current = Get(player);
		int max = GetMax(player);
		if (player.Creature != null && current + amount > max && player.Creature.HasPower<MoonNightPower>())
		{
			SakuyaWatch? sw = player.GetRelic<SakuyaWatch>();
			if (sw != null)
			{
				player.Creature.GetPower<MoonNightPower>().Trigger();
				sw.ResetCounter();
			}
			SakuyaLunaDial? dial = player.GetRelic<SakuyaLunaDial>();
			if (dial != null)
			{
				player.Creature.GetPower<MoonNightPower>().Trigger();
				dial.ResetCounter();
			}
		}
		int next = Math.Clamp(current + amount, 0, max);
		await Set(player, next);
	}

	public static async Task OnEnergySpent(Player player, int energySpent)
	{
		if (player == null || energySpent <= 0)
		{
			return;
		}
		if (player.Creature == null || !player.Creature.HasPower<InfinitePower>())
		{
			return;
		}
		await Gain(player, energySpent * PointsPerEnergy);
		await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), player);
	}

	public static async Task<bool> TrySpend(Player player, int amount)
	{
		if (player == null)
		{
			return false;
		}
		if (amount <= 0)
		{
			return true;
		}
		int current = Get(player);
		if (current < amount)
		{
			return false;
		}
        //能力逻辑👇
        if (player.Creature != null && player.Creature.HasPower<InfinitePower>())
		{
             int cnt = amount / PointsPerEnergy;
			 await PlayerCmd.GainEnergy(cnt, player);
			 await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), player);
		}
        //能力逻辑👆
		await Set(player, Math.Max(current - amount, 0));
		return true;
	}

	private static async Task Set(Player player, int value)
	{
		if (player?.Creature == null)
		{
			return;
		}
		int max = GetMax(player);
		int clamped = Math.Clamp(value, 0, max);
		TimeStopPointPower? power = player.Creature.GetPower<TimeStopPointPower>();
		if (power == null)
		{
			if (clamped <= 0)
			{
				return;
			}
			await PowerCmd.Apply<TimeStopPointPower>(player.Creature, clamped, player.Creature, cardSource: null, silent: true);
			return;
		}
		int delta = clamped - power.Amount;
		if (delta != 0)
		{
			await PowerCmd.ModifyAmount(power, delta, applier: player.Creature, cardSource: null);
		}
	}
}
