using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
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
public class RedThanBlood: SakuyaCardModel
{
	public override bool CanBeGeneratedInCombat => true;
	public override bool GainsBlock => true;
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(10, ValueProp.Move)
     ];
	public RedThanBlood() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
			.Targeting(cardPlay.Target)
			.WithHitVfxNode((Creature t) => NScratchVfx.Create(t, goingRight: true))
			.Execute(choiceContext);
		await CreatureCmd.Heal(base.Owner.Creature, attackCommand.Results.Sum((DamageResult r) => r.TotalDamage + r.OverkillDamage));
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Damage.UpgradeValueBy(5);
	}
}

}
