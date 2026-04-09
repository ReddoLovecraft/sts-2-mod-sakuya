using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using System;
using System.Runtime.CompilerServices;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scripts.Main;

public static class TimeStopPointSystem
{
	public const int DefaultMax = 160;
	public const int PointsPerEnergy = 6;

	private static readonly ConditionalWeakTable<Player, State> _states = new ConditionalWeakTable<Player, State>();

	public static bool IsEnabledFor(Player player)
	{
		return player?.Character is SakuyaCharacter;
	}

	public static int Get(Player player)
	{
		return _states.TryGetValue(player, out State? state) ? state.Current : 0;
	}

	public static int GetMax(Player player)
	{
		return _states.TryGetValue(player, out State? state) ? state.Max : DefaultMax;
	}

	public static void InitForCombat(Player player, int? max = null, int initial = 0)
	{
		if (player == null)
		{
			return;
		}
		State state = _states.GetOrCreateValue(player);
		int old = state.Current;
		state.Max = Math.Max(0, max ?? DefaultMax);
		state.Current = Math.Clamp(initial, 0, state.Max);
		if (old != state.Current)
		{
			state.Changed?.Invoke(old, state.Current);
		}
	}

	public static void Gain(Player player, int amount)
	{
		if (player == null || amount <= 0)
		{
			return;
		}
		State state = _states.GetOrCreateValue(player);
		int old = state.Current;
        //能力逻辑
		if(state.Current+amount>state.Max&&player.Creature.HasPower<MoonNightPower>())
        {
            SakuyaWatch sw=player.GetRelic<SakuyaWatch>();
            if(sw!=null)
			{
				player.Creature.GetPower<MoonNightPower>().Trigger();
				sw.ResetCounter();
			}
        }
        //重置怀表计数
		state.Current = Math.Clamp(state.Current + amount, 0, state.Max);
		if (old != state.Current)
		{
			state.Changed?.Invoke(old, state.Current);
		}
	}

	public static void OnEnergySpent(Player player, int energySpent)
	{
		if (player == null || energySpent <= 0)
		{
			return;
		}
		if (player.Creature == null || !player.Creature.HasPower<InfinitePower>())
		{
			return;
		}
		Gain(player, energySpent * PointsPerEnergy);
	}

	public static bool TrySpend(Player player, int amount)
	{
		if (player == null)
		{
			return false;
		}
		if (amount <= 0)
		{
			return true;
		}
		State state = _states.GetOrCreateValue(player);
		if (state.Current < amount)
		{
			return false;
		}
        //能力逻辑👇
        if (player.Creature != null && player.Creature.HasPower<InfinitePower>())
		{
             int cnt = amount / PointsPerEnergy;
			 PlayerCmd.GainEnergy(cnt, player);
		}
        //能力逻辑👆
		int old = state.Current;
		state.Current = Math.Max(state.Current - amount, 0);
		if (old != state.Current)
		{
			state.Changed?.Invoke(old, state.Current);
		}
		return true;
	}

	public static void Subscribe(Player player, Action<int, int> onChanged)
	{
		if (player == null || onChanged == null)
		{
			return;
		}
		State state = _states.GetOrCreateValue(player);
		state.Changed += onChanged;
	}

	public static void Unsubscribe(Player player, Action<int, int> onChanged)
	{
		if (player == null || onChanged == null)
		{
			return;
		}
		if (_states.TryGetValue(player, out State? state))
		{
			state.Changed -= onChanged;
		}
	}

	private sealed class State
	{
		public int Current;
		public int Max = DefaultMax;
		public Action<int, int>? Changed;
	}
}
