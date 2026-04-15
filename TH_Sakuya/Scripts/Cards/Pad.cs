using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class Pad: SakuyaCardModel
{
	 public override bool GainsBlock => true;
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<StrengthPower>()
  });
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move),new CardsVar(2)];
	public Pad() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
		.WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
		.SpawningHitVfxOnEachCreature()
		.Execute(choiceContext);
		await PowerCmd.Apply<PadPower>(base.Owner.Creature, attackCommand.Results.Sum((DamageResult r) => r.TotalDamage + r.OverkillDamage),Owner.Creature,this);
		await PowerCmd.Apply<VulnerablePower>(Owner.Creature,this.DynamicVars.Cards.IntValue,Owner.Creature,this);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(2);
		DynamicVars.Cards.UpgradeValueBy(-1);
	}
}
}

