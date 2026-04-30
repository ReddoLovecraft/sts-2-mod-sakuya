using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using System.Threading.Tasks;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.GameActions;

public sealed class SilverSpaceGrantTspAction : GameAction
{
	public Player Player { get; }
	public int Amount { get; }

	public override ulong OwnerId => Player.NetId;

	public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

	public SilverSpaceGrantTspAction(Player player, int amount)
	{
		Player = player;
		Amount = amount;
	}

	protected override async Task ExecuteAction()
	{
		if (Amount > 0)
		{
			await TimeStopPointSystem.Gain(Player, Amount);
		}
	}

	public override INetAction ToNetAction()
	{
		return new NetSilverSpaceGrantTspAction(Amount);
	}
}

public struct NetSilverSpaceGrantTspAction : INetAction, IPacketSerializable
{
	private int _amount;

	public NetSilverSpaceGrantTspAction(int amount)
	{
		_amount = amount;
	}

	public GameAction ToGameAction(Player player)
	{
		return new SilverSpaceGrantTspAction(player, _amount);
	}

	public void Serialize(PacketWriter writer)
	{
		writer.WriteInt(_amount);
	}

	public void Deserialize(PacketReader reader)
	{
		_amount = reader.ReadInt();
	}

	public override readonly string ToString()
	{
		return $"NetSilverSpaceGrantTspAction Amount: {_amount}";
	}
}

