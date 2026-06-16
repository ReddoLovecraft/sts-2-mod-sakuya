using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using System.Threading.Tasks;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.GameActions;

public sealed class InfiniteEnergySpentAction : GameAction
{
	public Player Player { get; }
	public int EnergySpent { get; }

	public override ulong OwnerId => Player.NetId;

	public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

	public InfiniteEnergySpentAction(Player player, int energySpent)
	{
		Player = player;
		EnergySpent = energySpent;
	}

	protected override async Task ExecuteAction()
	{
		await TimeStopPointSystem.OnEnergySpent(Player, EnergySpent);
	}

	public override INetAction ToNetAction()
	{
		return new NetInfiniteEnergySpentAction(EnergySpent);
	}
}

public struct NetInfiniteEnergySpentAction : INetAction, IPacketSerializable
{
	private int _energySpent;

	public NetInfiniteEnergySpentAction(int energySpent)
	{
		_energySpent = energySpent;
	}

	public GameAction ToGameAction(Player player)
	{
		return new InfiniteEnergySpentAction(player, _energySpent);
	}

	public void Serialize(PacketWriter writer)
	{
		writer.WriteInt(_energySpent);
	}

	public void Deserialize(PacketReader reader)
	{
		_energySpent = reader.ReadInt();
	}

	public override readonly string ToString()
	{
		return $"NetInfiniteEnergySpentAction EnergySpent: {_energySpent}";
	}
}
