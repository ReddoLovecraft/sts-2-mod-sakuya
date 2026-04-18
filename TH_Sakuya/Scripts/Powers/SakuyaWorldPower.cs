using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class SakuyaWorldPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/SWP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/SWP64.png";
	public override bool ShouldClearBlock(Creature creature)
	{
		if (base.Owner == null)
		{
			return true;
		}
		if (base.Owner != creature)
		{
			return true;
		}
		return !creature.HasPower<TimeStopPower>();
	}
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("TimeStop"),HoverTipFactory.Static(StaticHoverTip.Block)];
     public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
			await PowerCmd.Decrement(this);
        }
}
}


