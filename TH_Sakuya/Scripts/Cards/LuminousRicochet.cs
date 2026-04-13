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
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class LuminousRicochet: SakuyaCardModel
{
	  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
       StunIntent.GetStaticHoverTip()
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(5, ValueProp.Move),
        new CardsVar(5)
     ];
	public LuminousRicochet() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).WithHitCount(this.DynamicVars.Cards.IntValue).TargetingAllOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
			.Execute(choiceContext);
		foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies)
		{
			if(mos.IsAlive)
			{
				await CreatureCmd.Stun(mos);
			}
		}
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Damage.UpgradeValueBy(1);
		base.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
