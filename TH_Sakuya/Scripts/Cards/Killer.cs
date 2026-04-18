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
public class Killer: SakuyaCardModel
{
	 public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Ethereal];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
		 Tools.GetStaticKeyword("Knife")
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(8, ValueProp.Move)
     ];
	public Killer() : base(4, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (base.Owner.Character is SakuyaCharacter)
		{
			 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
		}
		int addtion=0;
		if(Owner.Creature.HasPower<KnifePower>())addtion+=Owner.Creature.GetPowerAmount<KnifePower>();
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
			.WithHitCount(1+addtion)
			.WithHitFx("vfx/vfx_starry_impact")
			.SpawningHitVfxOnEachCreature()
			.Execute(choiceContext);

		if(Owner.Creature.HasPower<KnifePower>())
		await (Owner.Creature.GetPower<KnifePower>()).ThrowKnife(choiceContext,null,KnifeType.RandomEnemy,1,addtion);	
	}
	protected override void OnUpgrade()
	{
		this.RemoveKeyword(CardKeyword.Ethereal);
	}
}

}
