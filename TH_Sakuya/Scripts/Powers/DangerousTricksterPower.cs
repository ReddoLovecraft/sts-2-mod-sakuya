using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class DangerousTricksterPower : SakuyaPowerModel
{
	private readonly List<CardCostSnapshot> _pendingTurnEndDiscards = [];

	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/DTP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/DTP64.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("Knife")];
	public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
	{
		if (card.Owner == base.Owner.Player )
		{
			int cnt=card.EnergyCost.GetWithModifiers(CostModifiers.Local);
			if(cnt>0)
			{
				Flash();
				KnifePower kp= await PowerCmd.Apply<KnifePower>(Owner, Amount*cnt, null, null);
				if(kp!=null)
				{	
					await kp.ThrowKnife(choiceContext,null,KnifeType.RandomEnemy,1,Amount*cnt);
				}
			}
		}
	}

	public override Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side != base.Owner.Side)
		{
			return Task.CompletedTask;
		}

		if (base.Owner.Player is not Player player)
		{
			return Task.CompletedTask;
		}

		_pendingTurnEndDiscards.Clear();
		foreach (CardModel card in PileType.Hand.GetPile(player).Cards)
		{
			if (card == null || card.Owner != player)
			{
				continue;
			}
			int cnt = card.EnergyCost.GetWithModifiers(CostModifiers.Local);
			if (cnt > 0)
			{
				_pendingTurnEndDiscards.Add(new CardCostSnapshot(card, cnt));
			}
		}

		return Task.CompletedTask;
	}

	public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side != base.Owner.Side)
		{
			return;
		}

		int total = 0;
		foreach (CardCostSnapshot entry in _pendingTurnEndDiscards)
		{
			if (entry.Card?.Pile?.Type == PileType.Discard)
			{
				total += entry.Cost * Amount;
			}
		}
		_pendingTurnEndDiscards.Clear();

		if (total > 0)
		{
			Flash();
			KnifePower kp= await PowerCmd.Apply<KnifePower>(Owner, total, null, null);
			if(kp!=null)
			{
				await kp.ThrowKnife(choiceContext,null,KnifeType.RandomEnemy,1,total);
			}
		}
	}

	private readonly struct CardCostSnapshot(CardModel card, int cost)
	{
		public CardModel Card { get; } = card;
		public int Cost { get; } = cost;
	}

}
}

