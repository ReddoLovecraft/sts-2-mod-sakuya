using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class DanceStarSwordPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/DSSP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/DSSP64.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("Knife")];
	public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
             if (cardPlay.Card.Owner == base.Owner.Player&&cardPlay.Card.Type==CardType.Attack)
			{
			   if(Owner.HasPower<KnifePower>())
			   {
				  Creature target=cardPlay.Target;
				  if(target!=null)
				  {
					await Owner.GetPower<KnifePower>().ThrowKnife(context,target,KnifeType.AnyEnemy,1,Amount);
				  }
				  else
				  {
					await Owner.GetPower<KnifePower>().ThrowKnife(context,null,KnifeType.RandomEnemy,1,Amount);
				  }
			   }
			   
			}
        }

}
}



