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
public class Ricochet: SakuyaCardModel
{
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(8, ValueProp.Move),
        new CardsVar(1)
     ];
	public Ricochet() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		int cnt=0;
        cnt+=Owner.Creature.CombatState.HittableEnemies.Count*base.DynamicVars.Cards.IntValue;
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(1+cnt).FromCard(this)
			.TargetingRandomOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
