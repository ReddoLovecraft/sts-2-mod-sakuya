using Godot;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class RunKnifePower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/RKP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/RKP64.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("Knife")];
	public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
	{
		if (!fromHandDraw && card.Owner.Creature == base.Owner && card.Owner.Creature.CombatState.CurrentSide == card.Owner.Creature.Side)
		{
			if(Owner.HasPower<KnifePower>())
			{
				await Owner.GetPower<KnifePower>().ThrowKnife(choiceContext,null,KnifeType.AllEnemies,base.Amount,1);
			}
		}
	}

}
}



