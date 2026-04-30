using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using System.Collections.Generic;
using System.Linq;
using TH_Sakuya.Scrpits.Relics;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scripts.GameActions;

public sealed class TimeStopToggleAction : GameAction
{
	public Player Player { get; }
	public int? FirstGrantTspOverride { get; }

	public override ulong OwnerId => Player.NetId;

	public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

	public TimeStopToggleAction(Player player, int? firstGrantTspOverride = null)
	{
		Player = player;
		FirstGrantTspOverride = firstGrantTspOverride;
	}

	protected override async Task ExecuteAction()
	{
		SakuyaWatch? watch = Player.GetRelic<SakuyaWatch>();
		if (watch != null)
		{
			await watch.ToggleTimeStop(Player, FirstGrantTspOverride);
			return;
		}

		SakuyaLunaDial? dial = Player.GetRelic<SakuyaLunaDial>();
		if (dial != null)
		{
			await dial.ToggleTimeStop(Player, FirstGrantTspOverride);
		}
	}

	public override INetAction ToNetAction()
	{
		if (FirstGrantTspOverride.HasValue)
		{
			return new NetTimeStopToggleAction(FirstGrantTspOverride.Value);
		}

		int firstGrantTsp = -1;
		if (Player?.Creature?.CombatState != null
			&& !Player.Creature.HasPower<TimeStopPower>()
			&& !Player.Creature.HasPower<TimeStopFirstGrantPower>())
		{
			firstGrantTsp = CalculateEnemyAttackIntentTotalForPlayer(Player);
		}
		return new NetTimeStopToggleAction(firstGrantTsp);
	}

	private static int CalculateEnemyAttackIntentTotalForPlayer(Player player)
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

public struct NetTimeStopToggleAction : INetAction, IPacketSerializable
{
	private int _firstGrantTsp;

	public NetTimeStopToggleAction(int firstGrantTsp)
	{
		_firstGrantTsp = firstGrantTsp;
	}

	public GameAction ToGameAction(Player player)
	{
		int? overrideTsp = _firstGrantTsp >= 0 ? _firstGrantTsp : null;
		return new TimeStopToggleAction(player, overrideTsp);
	}

	public void Serialize(PacketWriter writer)
	{
		writer.WriteInt(_firstGrantTsp);
	}

	public void Deserialize(PacketReader reader)
	{
		_firstGrantTsp = reader.ReadInt();
	}

	public override readonly string ToString()
	{
		return $"NetTimeStopToggleAction FirstGrantTsp: {_firstGrantTsp}";
	}
}
