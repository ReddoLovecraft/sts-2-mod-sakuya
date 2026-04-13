using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class SquareRicochet: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
		 Tools.GetStaticKeyword("Knife")
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new DynamicVar("Power", 3),
        new CardsVar(3)
     ];
	  protected override bool IsPlayable => Owner.Creature.HasPower<KnifePower>();
	public SquareRicochet() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if(Owner.Creature.HasPower<KnifePower>())
		{
			await (Owner.Creature.GetPower<KnifePower>()).ThrowKnife(choiceContext,cardPlay.Target,KnifeType.AnyEnemy,1+base.DynamicVars.Cards.IntValue,1,1+base.DynamicVars["Power"].IntValue);
		}
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars["Power"].UpgradeValueBy(1);
	}
}

}
