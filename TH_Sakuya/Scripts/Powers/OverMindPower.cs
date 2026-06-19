using System.Security.Cryptography;
using System;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scrpits.Cards;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class OverMindPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/OMP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/OMP64.png";
	private int cnt=0;
	private bool _hasEndedTurnDueToFullHand;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("TimeStop"),HoverTipFactory.ForEnergy(this),HoverTipFactory.FromCard<SelfDisolve>()];

	public void ResetCounter()
	{
		cnt=0;
	}

	private static int GetMaxHandSizeOrDefault(Player player)
	{
		object? pcs = player.PlayerCombatState;
		if (pcs == null)
		{
			return 10;
		}

		Type t = pcs.GetType();
		string[] propNames =
		[
			"MaxHandSize",
			"MaxCardsInHand",
			"HandLimit",
			"HandSizeLimit",
			"MaxHandCount",
		];

		foreach (string name in propNames)
		{
			var prop = t.GetProperty(name);
			if (prop?.PropertyType == typeof(int))
			{
				object? v = prop.GetValue(pcs);
				if (v is int i && i > 0)
				{
					return i;
				}
			}
		}

		return 10;
	}

	private void ResetTimeStopEndCountTo12()
	{
		Player player = Owner.Player;
		SakuyaWatch? watch = player.GetRelic<SakuyaWatch>();
		if (watch != null)
		{
			watch.SetCounter(12);
			return;
		}
		SakuyaLunaDial? dial = player.GetRelic<SakuyaLunaDial>();
		if (dial != null)
		{
			dial.SetCounter(12);
		}
	}

	public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
	{
		if (_hasEndedTurnDueToFullHand || Owner?.Player == null)
		{
			return;
		}
		if (card.Owner != Owner.Player)
		{
			return;
		}

		int maxHand = GetMaxHandSizeOrDefault(Owner.Player);
		int handCount = PileType.Hand.GetPile(Owner.Player).Cards.Count(c => c != null);
		if (handCount < maxHand)
		{
			return;
		}

		_hasEndedTurnDueToFullHand = true;
		Flash();
		ResetTimeStopEndCountTo12();
		PlayerCmd.EndTurn(Owner.Player, canBackOut: false);
	}

	public async Task TriggerOverMind(bool isExit=false)
	{
		Flash();
		cnt++;
		if(!isExit)
		{
		await PlayerCmd.GainEnergy(Amount,Owner.Player);
		await CardPileCmd.Draw(new BlockingPlayerChoiceContext(),Amount,Owner.Player);
		}
		if(cnt>2)
		{
		CardModel card = base.CombatState.CreateCard<SelfDisolve>(Owner.Player);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, creator: Owner.Player,CardPilePosition.Random));
		}
		
	}
	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
			_hasEndedTurnDueToFullHand = false;
			ResetCounter();
        }
}
}

