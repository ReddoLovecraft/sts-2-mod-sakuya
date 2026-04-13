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
public class BounceKnife: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
		 Tools.GetStaticKeyword("Knife")
  });
   protected override bool IsPlayable => Owner.Creature.HasPower<KnifePower>();
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		 new DynamicVar("Power", 8),
        new CardsVar(3)
     ];
	public BounceKnife() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		 int amount = base.DynamicVars["Power"].IntValue;
		 if(Owner.Creature.HasPower<KnifePower>())
		 {
			 await (Owner.Creature.GetPower<KnifePower>()).ThrowKnife(choiceContext,null,KnifeType.RandomEnemy,1,amount,1+base.DynamicVars.Cards.IntValue);
		 }
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars["Power"].UpgradeValueBy(8);
	}
}

}
