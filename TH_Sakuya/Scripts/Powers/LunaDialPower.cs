using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class LunaDialPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Single;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/LDP232.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/LDP264.png";
	 public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == CombatSide.Player&&Owner.HasPower<TimeStopPower>() )
		{
			Flash();
			List<CardModel> list =  (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 0, 999999999), 
            context: choiceContext, player: base.Owner.Player, filter: RetainFilter, source: this)).ToList();
			if (list.Count != 0)
			{
			foreach (CardModel item in list)
		{
			item.GiveSingleTurnRetain();
		}
			}
		}
	}
    private bool RetainFilter(CardModel card)
	{
		return !card.ShouldRetainThisTurn;
	}
}
}



