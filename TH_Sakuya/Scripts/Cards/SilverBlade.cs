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
public class SilverBlade: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
         HoverTipFactory.FromPower<StrengthPower>()
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(12, ValueProp.Move),
        new DynamicVar("Power", 1),
        new CardsVar(3)
     ];
	public SilverBlade() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_bloody_impact");
		await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars.Cards.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered , this);
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
			.Targeting(cardPlay.Target)
			.WithHitVfxNode((Creature t) => NScratchVfx.Create(t, goingRight: true))
			.Execute(choiceContext);
		await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars["Power"].IntValue,base.Owner.Creature,this);
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Damage.UpgradeValueBy(3);
		base.DynamicVars["Power"].UpgradeValueBy(1);
	}
}

}
