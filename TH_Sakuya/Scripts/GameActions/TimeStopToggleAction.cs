using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using TH_Sakuya.Scrpits.Relics;

namespace TH_Sakuya.Scripts.GameActions;

public sealed class TimeStopToggleAction : GameAction
{
	public Player Player { get; }

	public override ulong OwnerId => Player.NetId;

	public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

	public TimeStopToggleAction(Player player)
	{
		Player = player;
	}

	protected override async Task ExecuteAction()
	{
		SakuyaWatch? watch = Player.GetRelic<SakuyaWatch>();
		if (watch != null)
		{
			await watch.ToggleTimeStop(Player);
			return;
		}

		SakuyaLunaDial? dial = Player.GetRelic<SakuyaLunaDial>();
		if (dial != null)
		{
			await dial.ToggleTimeStop(Player);
		}
	}

	public override INetAction ToNetAction()
	{
		return new NetTimeStopToggleAction();
	}
}

public struct NetTimeStopToggleAction : INetAction, IPacketSerializable
{
	public GameAction ToGameAction(Player player)
	{
		return new TimeStopToggleAction(player);
	}

	public void Serialize(PacketWriter writer)
	{
	}

	public void Deserialize(PacketReader reader)
	{
	}

	public override readonly string ToString()
	{
		return "NetTimeStopToggleAction";
	}
}
