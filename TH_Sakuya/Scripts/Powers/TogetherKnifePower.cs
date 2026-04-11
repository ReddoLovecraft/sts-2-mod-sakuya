using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class TogetherKnifePower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/TKP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/TKP64.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("Knife")];
		public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner != base.Owner.Player && CombatManager.Instance.IsInProgress && cardPlay.Card.Type == CardType.Attack)
		{
			int intValue = cardPlay.Card.EnergyCost.GetWithModifiers(CostModifiers.Local);
			if(intValue < 0)intValue=0;
			if(Owner.HasPower<KnifePower>())
			{
				if(cardPlay.Card.TargetType == TargetType.AnyEnemy&&cardPlay.Target.IsAlive&&cardPlay.Target.IsHittable)
				{
					await Owner.GetPower<KnifePower>().ThrowKnife(context,cardPlay.Target,KnifeType.AnyEnemy,intValue+1,Amount);
				}
				else
				{
					await Owner.GetPower<KnifePower>().ThrowKnife(context,null,KnifeType.RandomEnemy,intValue+1,Amount);
				}
			}
		}
	}

}
}



