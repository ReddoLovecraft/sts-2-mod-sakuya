using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class TimeWarpPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
	public override bool IsInstanced => true;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/TWP232.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/TWP264.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [ HoverTipFactory.FromPower<StrengthPower>(),
	 HoverTipFactory.FromPower<DexterityPower>()];
	 protected override IEnumerable<DynamicVar> CanonicalVars => [(new CardsVar(0))];
	public void SetPowerCount(int count)
	{
		AssertMutable();
		base.DynamicVars.Cards.BaseValue=count;
	}
 		public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
             if (cardPlay.Card.Owner == base.Owner.Player )
			{
			   this.Amount--;
			   if(Amount<=0)
			   {
				this.Amount=12;
				Flash();
				await PowerCmd.Apply<StrengthPower>(Owner,this.DynamicVars.Cards.BaseValue,null,null);
				await PowerCmd.Apply<DexterityPower>(Owner,this.DynamicVars.Cards.BaseValue,null,null);
			   }
			}
        }
}
}



