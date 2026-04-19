using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using TH_Sakuya.Scrpits.Relics;

namespace TH_Sakuya.Scripts.GameActions;

public sealed class ReverseWatchRewindAction : GameAction
{
	public Player Player { get; }

	public override ulong OwnerId => Player.NetId;

	public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

	public ReverseWatchRewindAction(Player player)
	{
		Player = player;
	}

	protected override async Task ExecuteAction()
	{
		ReverseWatch? relic = Player.GetRelic<ReverseWatch>();
		if (relic == null)
		{
			return;
		}
		await relic.TryRewind();
	}

	public override INetAction ToNetAction()
	{
		return new NetReverseWatchRewindAction();
	}
}

public struct NetReverseWatchRewindAction : INetAction, IPacketSerializable
{
	public GameAction ToGameAction(Player player)
	{
		return new ReverseWatchRewindAction(player);
	}

	public void Serialize(PacketWriter writer)
	{
	}

	public void Deserialize(PacketReader reader)
	{
	}
}

