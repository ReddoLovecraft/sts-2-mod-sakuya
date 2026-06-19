using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scrpits.Cards
{
	[Pool(typeof(SakuyaCardPool))]
public class FinishHomework : SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[4]
  {
         Tools.GetStaticKeyword("TimeStop"),
		 HoverTipFactory.Static(StaticHoverTip.Transform),
         HoverTipFactory.FromCard<Sweep>(base.IsUpgraded),
		 base.EnergyHoverTip
  });
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move),new EnergyVar(1)];
	public FinishHomework() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.Damage(choiceContext, Owner.Creature.CombatState.HittableEnemies, base.DynamicVars.Damage, base.Owner.Creature);
		await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext,  base.Owner.Creature, base.DynamicVars.Energy.IntValue,Owner.Creature,this);

	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(2); 
		base.DynamicVars.Energy.UpgradeValueBy(1);
	}
}

}
