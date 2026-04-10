using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class CannotTimeStopPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Debuff;
	public override PowerStackType StackType => PowerStackType.Single;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/CTSP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/CTSP64.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("TimeStop")];
	public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? _, out decimal modifiedAmount)
	{
		if (target != base.Owner)
		{
			modifiedAmount = amount;
			return false;
		}
		if (canonicalPower is not TimeStopPower)
		{
			modifiedAmount = amount;
			return false;
		}
		if (!canonicalPower.IsVisible)
		{
			modifiedAmount = amount;
			return false;
		}
		modifiedAmount = default(decimal);
		return true;
	}
	public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == base.Owner.Side)
		{
			await PowerCmd.Remove(this);
		}
	}
}
}



