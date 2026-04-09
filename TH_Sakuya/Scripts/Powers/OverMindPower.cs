using System.Security.Cryptography;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
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

	public void ResetCounter()
	{
		cnt=0;
	}
	public async Task TriggerOverMind(bool isExit=false)
	{
		Flash();
		cnt++;
		if(!isExit)
		{
		await PlayerCmd.GainEnergy(Amount,Owner.Player);
		await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(),Amount,Owner.Player);
		}
		if(cnt>2)
		{
		CardModel card = base.CombatState.CreateCard<SelfDisolve>(Owner.Player);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, addedByPlayer: true,CardPilePosition.Random));
		}
		
	}
	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
			ResetCounter();
        }
}
}



