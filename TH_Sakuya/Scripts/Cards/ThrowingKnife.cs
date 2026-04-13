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
public class ThrowingKnife: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
		 Tools.GetStaticKeyword("Knife")
  });
    protected override bool IsPlayable => Owner.Creature.HasPower<KnifePower>();
 public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Retain];
	public ThrowingKnife() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if(Owner.Creature.HasPower<KnifePower>())
		{
			KnifePower kp=Owner.Creature.GetPower<KnifePower>();
			int cnt=1;
			cnt+=kp.Amount;
			await kp.ThrowKnife(choiceContext,null,KnifeType.RandomEnemy,1,kp.Amount,cnt);
		}
	}
	protected override void OnUpgrade()
	{
		this.EnergyCost.UpgradeBy(-1);
	}
}

}
