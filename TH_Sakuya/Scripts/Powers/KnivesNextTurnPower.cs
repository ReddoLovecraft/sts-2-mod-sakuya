using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class KnivesNextTurnPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/KNTP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/KNTP64.png";
	protected override IEnumerable<DynamicVar> CanonicalVars => [(new CardsVar(4))];
	public void SetKnifeCount(int count)
	{
		AssertMutable();
		base.DynamicVars.Cards.BaseValue=count;
	}
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("Knife")];
  		public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
            await PowerCmd.Apply<KnifePower>(Owner,this.DynamicVars.Cards.BaseValue,null,null);
			await PowerCmd.Decrement(this);
        }
}
}



