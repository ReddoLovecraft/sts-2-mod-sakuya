using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class TimeFastPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/TFP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/TFP64.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("TimeStop")];
	  public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (dealer != base.Owner)
		{
			return amount;
		}
		return amount*Amount;
	}
	public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
            if (Owner == null || cardPlay.Card.Owner != Owner.Player)
            {
                return Task.CompletedTask;
            }
            if(Owner.Player.GetRelic<SakuyaWatch>()!=null)
            {
                Owner.Player.GetRelic<SakuyaWatch>().DecrementCounterAndMaybeEndTurn(Owner.Player);
            }
            else if(Owner.Player.GetRelic<SakuyaLunaDial>()!=null)
            {
                Owner.Player.GetRelic<SakuyaLunaDial>().DecrementCounterAndMaybeEndTurn(Owner.Player);
            }
            return Task.CompletedTask;
    }
	public override Task AfterModifyingHpLostBeforeOsty()
	{
		Flash();
		return Task.CompletedTask;
	}
}
}



